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
        [NonSerialized]
        public bool isNetworkConnected = false;
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
            // Start client/server based on ParrelSync settings.
            if (ClonesManager.IsClone() || forceClient)
                StartClient();
            else
                StartServer();

            // Set room manager mode editor.
            roomManager.mode = mode;
#endif
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
            GameObject clientInstance= Instantiate(playerPrefab);
            Client client = clientInstance.GetComponent<Client>();
            client.playerId = player.id;
            player.client = client;

            // Add client to the server connection.
            NetworkServer.AddPlayerForConnection(conn, clientInstance);
            GameLogger.LogServer("New client connected!", player);
        }

        public void CreateRoom(NetworkConnection conn)
        {
            StartCoroutine(CreateRoomAsync(conn));
        }

        private IEnumerator CreateRoomAsync(NetworkConnection conn)
        {
            // Get the player.
            ServerPlayer player = GetPlayer(conn);

            // Check if player can create room.
            if (player.room != null)
            {
                player.client.TargetHostRoom(null, NetworkCode.HostFailedAlreadyInRoom);
                yield break;
            }

            // Ask Room Manager to create a room.
            Room room = roomManager.CreateRoom();

            // Load the lobby scene on the server.
            yield return SceneManager.LoadSceneAsync(lobbyScene.name, LoadSceneMode.Additive);
            Scene loadedScene = GetLastScene();
            room.scene = loadedScene;
            room.host = player;
            additiveScenes.Add(loadedScene);
            GameLogger.LogServer($"New room created.\nCode: {room.code}", player);

            // Let the client know that the room has been created.
            player.client.TargetHostRoom(room, NetworkCode.Success);
        }

        public void JoinRoom(NetworkConnection conn, string roomCode)
        {
            StartCoroutine(JoinRoomAsync(conn, roomCode));
        }

        private IEnumerator JoinRoomAsync(NetworkConnection conn, string roomCode)
        {

            // Get the player.
            ServerPlayer player = GetPlayer(conn);
            GameLogger.LogServer($"A client is trying to join room {roomCode}.", player);

            // Check if room is joinable.
            if (player.room != null)
            {
                player.client.TargetJoinRoom(null, NetworkCode.JoinFailedAlreadyInRoom);
                yield break;
            }
            Room room = roomManager.GetRoom(roomCode);
            if (room == null)
            {
                player.client.TargetJoinRoom(null, NetworkCode.JoinFailedRoomDoesNotExist);
                yield break;
            }
            if (room.state == RoomState.Game)
            {
                player.client.TargetJoinRoom(null, NetworkCode.JoinFailedRoomGameAlreadyStarted);
                yield break;
            }

            // Wait for room to be initialized.
            while (!room.isRoomInitialized)
                yield return new WaitForSeconds(1);

            // Move the client to the lobby.
            SceneManager.MoveGameObjectToScene(conn.identity.gameObject, room.scene);
            conn.Send(new SceneMessage { sceneName = menuScene.name, sceneOperation = SceneOperation.UnloadAdditive });
            conn.Send(new SceneMessage { sceneName = lobbyScene.name, sceneOperation = SceneOperation.LoadAdditive });

            // Register player to the room manager.
            roomManager.JoinRoom(room, player);
            player.room = room;

            // Let the client know that the room has been joined.
            player.client.TargetJoinRoom(room, NetworkCode.Success);
            GameLogger.LogServer($"A client successfully joined room {room.code}.", player);
        }

        public GameObject InstantiateLobbyPlayer(GameObject lobbyManager, ServerPlayer player)
        {
            // Instantiate the lobby player in the lobby scene.
            GameObject lobbyPlayerInstance = Instantiate(lobbyPlayerPrefab, lobbyManager.transform);
            lobbyPlayerInstance.transform.parent = null;

            // Set the name of the lobby player according to client.
            LobbyPlayer lobbyPlayer = lobbyPlayerInstance.GetComponent<LobbyPlayer>();
            lobbyPlayer.name = $"Player ({player.client.playerName})";
            lobbyPlayer.playerName = player.client.playerName;

            // Spawn the lobby player in all connected clients in the lobby.
            NetworkServer.Spawn(lobbyPlayerInstance, player.clientConnection);
            return lobbyPlayerInstance;
        }

        public void DestroyLobbyPlayer(GameObject lobbyPlayerInstance)
        {
            NetworkServer.Destroy(lobbyPlayerInstance);
        }

        public void LeaveRoom(NetworkConnection conn)
        {
            // Remove player from the lobby through room manager.
            ServerPlayer player = GetPlayer(conn);
            GameLogger.LogServer($"A client is leaving room {player.room.code}", player);
            Room room = roomManager.LeaveRoom(player);

            // Move client out of the room.
            SceneManager.MoveGameObjectToScene(player.client.gameObject, gameObject.scene);
            conn.Send(new SceneMessage { sceneName = room.scene.name, sceneOperation = SceneOperation.UnloadAdditive });
            conn.Send(new SceneMessage { sceneName = menuScene.name, sceneOperation = SceneOperation.LoadAdditive });
            
            // Let the client know that they have left the room.
            player.client.TargetLeaveRoom();

            // Check if room is empty.
            if (room.players.Count == 0)
            {
                // Remove room.
                roomManager.RemoveRoom(room);

                // Unload the scene from the server.
                SceneManager.UnloadSceneAsync(room.scene);
                additiveScenes.Remove(room.scene);
                GameLogger.LogServer($"Room {room.code} was removed.");
            }
        }

        public void StartGame(NetworkConnection conn, Room room)
        {
            // Get player.
            ServerPlayer player = GetPlayer(conn);
            GameLogger.LogServer($"A player attempting to start the game on {room.code}", player);

            // Check if player is allowed to start the game.
            if (room.host != player)
            {
                // TODO: Return error.
            }
            if (room.players.Count < 4)
            {
                // TODO: Return error.
            }

            // TODO: Actually start the game.
        }

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            // Remove the player from players set.
            ServerPlayer player = players.SingleOrDefault(x => x.connectionId == conn.connectionId);
            if (player != null)
            {
                if (player.room != null)
                    LeaveRoom(conn);
                players.Remove(player);
                GameLogger.LogServer("A client disconnected.", player);
            }

            // Remove the objects for this connection.
            // TODO: Create a timeout.
            // TODO: Try not to destroy objects.
            NetworkServer.DestroyPlayerForConnection(conn);
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
            isNetworkConnected = true;
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
            isNetworkConnected = false;
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
