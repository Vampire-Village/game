using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using Mirror;
#if UNITY_EDITOR
using ParrelSync;
#endif

namespace VampireVillage.Network
{
    public class VampireVillageNetwork : NetworkManager
    {
#region Network Settings
        public ushort networkPort = 7777;
#if UNITY_EDITOR
        public bool forceClient = false;
#endif
#endregion

#region Scene Settings
        public SceneAsset networkScene;
        public SceneAsset menuScene;
        public SceneAsset lobbyScene;
        public SceneAsset gameScene;
#endregion

#region Prefabs
        public GameObject lobbyManagerPrefab;
        public GameObject lobbyPlayerPrefab;
#endregion

#region Network Events
        public event Action OnNetworkStart;
        public event Action OnNetworkOnline;
        public event Action OnNetworkOffline;
#endregion

        private List<Scene> additiveScenes = new List<Scene>();
        private readonly HashSet<ServerPlayer> players = new HashSet<ServerPlayer>();

        public RoomManager roomManager;

        public override void Awake()
        {
            // Set default network settings.
            dontDestroyOnLoad = false;
            runInBackground = true;
            autoStartServerBuild = true;
            serverTickRate = 60;
            disconnectInactiveConnections = false;
            autoCreatePlayer = true;

            // Set transport port.
            transport = GetComponent<Transport>();
            ((TelepathyTransport)transport).port = networkPort;

            // Initialize reference.
            roomManager = GetComponent<RoomManager>();

            // Register custom prefabs.
            spawnPrefabs.Add(lobbyManagerPrefab);
            spawnPrefabs.Add(lobbyPlayerPrefab);

            base.Awake();
        }

        public override void Start()
        {
#if UNITY_EDITOR
            if (ClonesManager.IsClone() || forceClient)
                StartClient();
            else
                StartServer();
#endif

            roomManager.mode = mode;
        }

#region Server Methods
        public override void OnStartServer()
        {
            GameLogger.LogServer("Server started!");
            OnNetworkStart?.Invoke();
        }

        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            // Create new server player data.
            ServerPlayer player = new ServerPlayer();
            player.connectionId = conn.connectionId;
            player.clientConnection = conn;
            players.Add(player);

            // Create the client.
            GameObject clientObject = Instantiate(playerPrefab);
            Client client = clientObject.GetComponent<Client>();
            client.playerId = player.id;
            player.client = client;

            // Add client to the server connection.
            NetworkServer.AddPlayerForConnection(conn, clientObject);
            GameLogger.LogServer("New client connected!", player);
        }

        public void CreateRoom(NetworkConnection conn)
        {
            StartCoroutine(CreateRoomAsync(conn));
        }

        private IEnumerator CreateRoomAsync(NetworkConnection conn)
        {
            // Ask Room Manager to create a room.
            Room room = roomManager.CreateRoom();

            // Load the lobby scene on the server.
            yield return SceneManager.LoadSceneAsync(lobbyScene.name, LoadSceneMode.Additive);
            Scene loadedScene = GetLastScene();
            room.lobbyScene = loadedScene;
            additiveScenes.Add(loadedScene);
            GameLogger.LogServer($"New room created.\nCode: {room.code}", GetPlayer(conn));

            // Let the client know that the room has been created.
            conn.identity.GetComponent<Client>().TargetHostRoom(room);
        }

        public void JoinRoom(NetworkConnection conn, string roomCode)
        {
            StartCoroutine(JoinRoomAsync(conn, roomCode));
        }

        private IEnumerator JoinRoomAsync(NetworkConnection conn, string roomCode)
        {
            GameLogger.LogServer($"A client is trying to join room {roomCode}.", GetPlayer(conn));

            // Get client.
            Client client = conn.identity.GetComponent<Client>();

            // Check if room is joinable.
            // TODO: Check if room has started yet or not.
            Room room = roomManager.GetRoom(roomCode);
            if (room == null)
            {
                client.TargetJoinRoom(null);
                yield break;
            }
            while (!room.isRoomInitialized)
                yield return new WaitForSeconds(1);

            // Move the client to the lobby.
            SceneManager.MoveGameObjectToScene(conn.identity.gameObject, room.lobbyScene);
            conn.Send(new SceneMessage { sceneName = menuScene.name, sceneOperation = SceneOperation.UnloadAdditive });
            conn.Send(new SceneMessage { sceneName = lobbyScene.name, sceneOperation = SceneOperation.LoadAdditive });

            // Register player to the room manager.
            roomManager.JoinRoom(room, GetPlayer(conn));

            // Let the client know that the room has been joined.
            client.TargetJoinRoom(room);
        }

        public void InstantiateLobbyPlayer(GameObject lobbyManager, ServerPlayer player)
        {
            // Instantiate the lobby player in the lobby scene.
            GameObject lobbyPlayerInstance = Instantiate(lobbyPlayerPrefab, lobbyManager.transform);
            lobbyPlayerInstance.transform.parent = null;

            // Set the name of the lobby player according to client.
            LobbyPlayer lobbyPlayer = lobbyPlayerInstance.GetComponent<LobbyPlayer>();
            lobbyPlayer.playerName = player.client.playerName;

            // Spawn the lobby player in all connected clients in the lobby.
            NetworkServer.Spawn(lobbyPlayerInstance, player.clientConnection);
        }

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            // Remove the objects for this connection.
            // TODO: Create a timeout.
            // TODO: Try not to destroy objects.
            NetworkServer.DestroyPlayerForConnection(conn);
            
            // Remove the player from players set.
            ServerPlayer player = players.SingleOrDefault(x => x.connectionId == conn.connectionId);
            if (player != null)
            {
                players.Remove(player);
                GameLogger.LogServer("A client disconnected.", player);
            }
        }

        public override void OnStopServer()
        {
            GameLogger.LogServer("Server stopped.");
        }
#endregion

#region Client Methods
        public override void OnStartClient()
        {
            // Send client to start menu.
            if (GetLastScene().name != menuScene.name)
                SceneManager.LoadScene(menuScene.name, LoadSceneMode.Additive);

            GameLogger.LogClient("Client started!");
            OnNetworkStart?.Invoke();
        }

        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);
            
            GameLogger.LogClient("Client connected to the server!");
            OnNetworkOnline?.Invoke();
        }

        public override void OnClientSceneChanged(NetworkConnection conn)
        {
            base.OnClientSceneChanged(conn);

            // Add the last scene added.
            Scene lastScene = GetLastScene();
            if (lastScene.name != networkScene.name)
                additiveScenes.Add(lastScene);
        }

        public override void OnClientDisconnect(NetworkConnection conn)
        {
            StopClient();
            
            // TODO: Better async handling.
            // TODO: Enable reconnect.
            // TODO: Error message & loading scene.
            // Get the client back to the start menu.
            if (GetLastScene().name != menuScene.name)
            {
                foreach (Scene additiveScene in additiveScenes)
                    SceneManager.UnloadSceneAsync(additiveScene);
                SceneManager.LoadScene(menuScene.name, LoadSceneMode.Additive);
            }

            GameLogger.LogClient("Client disconnected.");
            OnNetworkOffline?.Invoke();
        }

        public override void OnStopClient()
        {
            GameLogger.LogClient("Client stopped.");
        }
#endregion

#region Helper Methods
        private ServerPlayer GetPlayer(NetworkConnection conn)
        {
            return players.SingleOrDefault(p => p.connectionId == conn.connectionId);
        }

        private static Scene GetLastScene()
        {
            return SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
        }
#endregion
    }
}
