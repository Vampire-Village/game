using System;
using Mirror;

namespace VampireVillage.Network
{
    /// <summary>
    /// Holds the client identity across scenes and handles client requests to the server.
    /// </summary>
    public class Client : NetworkBehaviour
    {
#region Properties
        /// <summary>
        /// The local client.
        /// </summary>
        public static Client local { get; private set; }

        /// <summary>
        /// The server player ID.
        /// </summary>
        [SyncVar]
        public Guid playerId;

        /// <summary>
        /// The player name.
        /// </summary>
        [SyncVar(hook = nameof(UpdateName))]
        public string playerName;

#region Callbacks
        public delegate void HostRoomErrorCallback(string errorMessage);
        private HostRoomErrorCallback hostRoomErrorCallback;

        public delegate void JoinRoomErrorCallback(string errorMessage);
        private JoinRoomErrorCallback joinRoomErrorCallback;

        public delegate void LeaveRoomErrorCallback(string errorMessage);
        private LeaveRoomErrorCallback leaveRoomErrorCallback;

        public delegate void StartGameErrorCallback(string errorMessage);
        private StartGameErrorCallback startGameErrorCallback;
#endregion

        private VampireVillageNetwork network;
#endregion

#region Unity Methods
        private void Awake()
        {
            network = VampireVillageNetwork.singleton as VampireVillageNetwork;
            syncMode = SyncMode.Observers;
            syncInterval = 0.1f;
        }
#endregion

#region Server Methods
#if UNITY_SERVER || UNITY_EDITOR
        public override void OnStartServer()
        {
            name = $"Client ({playerId})";
        }
#endif    

        [Command]
        private void CmdSetName(string newName)
        {
#if UNITY_SERVER || UNITY_EDITOR
            if (newName.Length > 0)
                playerName = newName;
            else
                playerName = "Player";
            name = $"Client ({playerName})";
#endif
        }

        [Command]
        private void CmdHostRoom()
        {
#if UNITY_SERVER || UNITY_EDITOR
            GameLogger.LogServer("A client requested a new room.", this);
            network.CreateRoom(connectionToClient);
#endif
        }

        [Command]
        private void CmdJoinRoom(string roomCode)
        {
#if UNITY_SERVER || UNITY_EDITOR
            GameLogger.LogServer($"A client requested to join a room.\nCode: {roomCode}", this);
            network.JoinRoom(connectionToClient, roomCode);
#endif
        }

        [Command]
        private void CmdLeaveRoom()
        {
#if UNITY_SERVER || UNITY_EDITOR
            GameLogger.LogServer($"A client requested to leave a room.", this);
            network.LeaveRoom(connectionToClient);
#endif
        }

        [Command]
        private void CmdStartGame()
        {
#if UNITY_SERVER || UNITY_EDITOR
            GameLogger.LogServer($"A client requested to start a game.", this);
            network.StartGame(connectionToClient);
#endif
        }
#endregion

#region Client Methods
        public override void OnStartAuthority()
        {
            local = this;
            name = "Client";
            GameLogger.LogClient("Client connected to the server.", this);
        }

        /// <summary>
        /// Sets the player name.
        /// </summary>
        /// <param name="name">Player name.</param>
        public void SetName(string name)
        {
            CmdSetName(name);
        }

        private void UpdateName(string oldName, string newName)
        {
            name = $"Client ({newName})";
        }

        /// <summary>
        /// Sends a request to the server to host a room.
        /// </summary>
        /// <param name="hostRoomErrorCallback">Callback when an error occurs.</param>
        public void HostRoom(HostRoomErrorCallback hostRoomErrorCallback = null)
        {
            GameLogger.LogClient("Creating new room...");
            this.hostRoomErrorCallback = hostRoomErrorCallback;
            Client.local.CmdHostRoom();
        }

        /// <summary>
        /// Called when the server has processed the host room request.
        /// </summary>
        /// <param name="room">The room created.</param>
        /// <param name="status">Request status code.</param>
        [TargetRpc]
        public void TargetHostRoom(Room room, NetworkCode status)
        {
            // Return error.
            if (status != NetworkCode.Success)
            {
                string message;
                switch (status)
                {
                    case NetworkCode.HostFailedAlreadyInRoom:
                        message = "Already in a room.";
                        break;
                    default:
                        message = "Something is wrong.";
                        break;
                }

                GameLogger.LogClient($"Failed to create a room. {message}");
                if (hostRoomErrorCallback != null)
                {
                    hostRoomErrorCallback(message);
                    hostRoomErrorCallback = null;
                }
                return;
            }

            GameLogger.LogClient($"Created new room!\nCode: {room.code}");

            // Automatically join the room after creating.
            CmdJoinRoom(room.code);
        }

        /// <summary>
        /// Sends a request to the server to join a room.
        /// </summary>
        /// <param name="roomCode">The room code.</param>
        /// <param name="joinRoomErrorCallback">Callback when an error occurs.</param>
        public void JoinRoom(string roomCode, JoinRoomErrorCallback joinRoomErrorCallback = null)
        {
            GameLogger.LogClient($"Joining room {roomCode}...");
            this.joinRoomErrorCallback = joinRoomErrorCallback;
            CmdJoinRoom(roomCode);
        }

        /// <summary>
        /// Called when the server has processed the join room request.
        /// </summary>
        /// <param name="room">The room joined.</param>
        /// <param name="status">Request status code.</param>
        [TargetRpc]
        public void TargetJoinRoom(Room room, NetworkCode status)
        {
            // Return error.
            if (status != NetworkCode.Success)
            {
                string message;
                switch (status)
                {
                    case NetworkCode.JoinFailedAlreadyInRoom:
                        message = "Already in a room.";
                        break;
                    case NetworkCode.JoinFailedRoomDoesNotExist:
                        message = "Room does not exist.";
                        break;
                    case NetworkCode.JoinFailedRoomGameAlreadyStarted:
                        message = "Room already started the game.";
                        break;
                    default:
                        message = "Something is wrong.";
                        break;
                }

                GameLogger.LogClient($"Failed to join the room. {message}");
                if (joinRoomErrorCallback != null)
                {
                    joinRoomErrorCallback(message);
                    joinRoomErrorCallback = null;
                }
                return;
            }

            GameLogger.LogClient($"Joined a room!\nCode: {room.code}");
        }

        /// <summary>
        /// Sends a request to the server to leave the room.
        /// </summary>
        /// <param name="leaveRoomErrorCallback">Callback when an error occurs.</param>
        public void LeaveRoom(LeaveRoomErrorCallback leaveRoomErrorCallback = null)
        {
            GameLogger.LogClient("Leaving room...");
            this.leaveRoomErrorCallback = leaveRoomErrorCallback;
            CmdLeaveRoom();
        }

        /// <summary>
        /// Called when the server has processed the leave room request.
        /// </summary>
        /// <param name="status">Request status code.</param>
        [TargetRpc]
        public void TargetLeaveRoom(NetworkCode status)
        {
            // Return error.
            if (status != NetworkCode.Success)
            {
                string message;
                switch (status)
                {
                    default:
                        message = "Something is wrong.";
                        break;
                }

                GameLogger.LogClient($"Failed to leave the room. {message}");
                if (leaveRoomErrorCallback != null)
                {
                    leaveRoomErrorCallback(message);
                    leaveRoomErrorCallback = null;
                }
                return;
            }

            GameLogger.LogClient("Left room successfully!");
        }

        /// <summary>
        /// Sends a request to the server to start the game.
        /// </summary>
        /// <param name="startGameErrorCallback">Callback when an error occurs.</param>
        public void StartGame(StartGameErrorCallback startGameErrorCallback = null)
        {
            GameLogger.LogClient("Starting the game...");
            this.startGameErrorCallback = startGameErrorCallback;
            CmdStartGame();
        }

        /// <summary>
        /// Called when the server has processed the start game request.
        /// </summary>
        /// <param name="status">Request status code.</param>
        [TargetRpc]
        public void TargetStartGame(NetworkCode status)
        {
            // Return error.
            if (status != NetworkCode.Success)
            {
                string message;
                switch (status)
                {
                    case NetworkCode.StartFailedNotInARoom:
                        message = "You are not in a room.";
                        break;
                    case NetworkCode.StartFailedNotHost:
                        message = "You are not the host.";
                        break;
                    case NetworkCode.StartFailedNotEnoughPlayers:
                        message = "Not enough player to start the game.";
                        break;
                    default:
                        message = "Something is wrong.";
                        break;
                }

                GameLogger.LogClient($"Failed to start the game. {message}");
                if (startGameErrorCallback != null)
                {
                    startGameErrorCallback(message);
                    startGameErrorCallback = null;
                }
                return;
            }

            GameLogger.LogClient("Started the game!");
        }
#endregion
    }
}
