using System;
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
        public SceneAsset menuScene;
        public SceneAsset lobbyScene;
        public SceneAsset gameScene;

        public event Action OnNetworkStart;
        public event Action OnNetworkOnline;
        public event Action OnNetworkOffline;

        private readonly HashSet<ServerPlayer> players = new HashSet<ServerPlayer>();
        private readonly Dictionary<string, Room> rooms = new Dictionary<string, Room>();

        public override void Start()
        {
#if UNITY_EDITOR
            if (ClonesManager.IsClone())
                StartClient();
            else
                StartServer();
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
            players.Add(player);

            // Create the client.
            GameObject clientObject = Instantiate(playerPrefab);
            Client client = clientObject.GetComponent<Client>();
            client.playerId = player.id;

            // Add client to the server connection.
            NetworkServer.AddPlayerForConnection(conn, clientObject);
            GameLogger.LogServer("New client connected!", player);
        }

        public Room CreateRoom(NetworkConnection conn)
        {
            // Generate a unique room code.
            string roomCode;
            do
            {
                roomCode = Room.GenerateCode();
            } while (rooms.ContainsKey(roomCode));

            // Create the room and store it.
            Room room = new Room(roomCode);
            rooms.Add(roomCode, room);

            // Create the lobby scene for the room in the server.
            SceneManager.LoadScene(lobbyScene.name, LoadSceneMode.Additive);
            room.lobbyScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);

            // Set the client's match ID to room ID.
            Client client = conn.identity.GetComponent<Client>();
            client.SetMatchID(room.id);

            // Change the client's scene to lobby.
            conn.Send(new SceneMessage{ sceneName = lobbyScene.name, sceneOperation = SceneOperation.Normal });
            SceneManager.MoveGameObjectToScene(conn.identity.gameObject, room.lobbyScene);

            GameLogger.LogServer($"New room created.\nCode: {room.code}", GetPlayer(conn));
            return room;
        }

        public Room JoinRoom(NetworkConnection conn, string roomCode)
        {
            GameLogger.LogServer($"A client is trying to join room {roomCode}.", GetPlayer(conn));

            // Return null if room doesn't exist.
            // TODO: Proper error handling.
            if (!rooms.ContainsKey(roomCode))
                return null;

            // Get the room information.
            // TODO: Check if room hasn't started game.
            Room room = rooms[roomCode];

            // Set the client's match ID to room ID.
            Client client = conn.identity.GetComponent<Client>();
            client.SetMatchID(room.id);

            // Change the client's scene to the lobby.
            conn.Send(new SceneMessage{ sceneName = lobbyScene.name, sceneOperation = SceneOperation.Normal });
            SceneManager.MoveGameObjectToScene(conn.identity.gameObject, room.lobbyScene);

            return room;
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
            GameLogger.LogClient("Client started!");
            OnNetworkStart?.Invoke();
        }

        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);
            
            GameLogger.LogClient("Client connected to the server!");
            OnNetworkOnline?.Invoke();
        }

        public override void OnClientDisconnect(NetworkConnection conn)
        {
            // TODO: Enable reconnect.
            StopClient();
            SceneManager.LoadScene(menuScene.name);

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
#endregion
    }
}
