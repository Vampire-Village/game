using System;
using Mirror;

namespace VampireVillage.Network
{
    /// <summary>
    /// Holds the client identity across scenes and handles client requests to the server.
    /// </summary>
    public class Client : NetworkBehaviour
    {
        /// <summary>
        /// The local client.
        /// </summary>
        public static Client local { get; private set; }

        /// <summary>
        /// The ServerPlayer ID that this client belongs to.
        /// </summary>
        [SyncVar]
        public Guid playerId;

        /// <summary>
        /// The player name.
        /// </summary>
        [SyncVar(hook = nameof(SetName))]
        public string playerName;

        public delegate void HostRoomErrorCallback(string errorMessage);
        private HostRoomErrorCallback hostRoomErrorCallback;

        public delegate void JoinRoomErrorCallback(string errorMessage);
        private JoinRoomErrorCallback joinRoomErrorCallback;

        private VampireVillageNetwork network;

        private void Awake()
        {
            network = VampireVillageNetwork.singleton as VampireVillageNetwork;
            syncMode = SyncMode.Observers;
            syncInterval = 0.1f;
        }

#region Server Methods
        public override void OnStartServer()
        {
#if UNITY_SERVER || UNITY_EDITOR
            name = $"Client ({playerId})";
#endif    
        }

        [Command]
        public void CmdSetName(string newName)
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
        public void CmdHostRoom()
        {
#if UNITY_SERVER || UNITY_EDITOR
            GameLogger.LogServer("A client requested a new room.", this);
            network.CreateRoom(connectionToClient);
#endif
        }

        [Command]
        public void CmdJoinRoom(string roomCode)
        {
#if UNITY_SERVER || UNITY_EDITOR
            GameLogger.LogServer($"A client requested to join a room.\nCode: {roomCode}", this);
            network.JoinRoom(connectionToClient, roomCode);
#endif
        }

        [Command]
        public void CmdLeaveRoom()
        {
#if UNITY_SERVER || UNITY_EDITOR
            GameLogger.LogServer($"A client requested to leave a room.", this);
            network.LeaveRoom(connectionToClient);
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

        public void HostRoom(HostRoomErrorCallback hostRoomErrorCallback)
        {
            this.hostRoomErrorCallback = hostRoomErrorCallback;
            Client.local.CmdHostRoom();
        }

        [TargetRpc]
        public void TargetHostRoom(Room room, NetworkCode code)
        {
            // Return error.
            if (room == null)
            {
                string message;
                switch (code)
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
            CmdJoinRoom(room.code);
        }

        public void JoinRoom(string roomCode, JoinRoomErrorCallback joinRoomErrorCallback)
        {
            this.joinRoomErrorCallback = joinRoomErrorCallback;
            CmdJoinRoom(roomCode);
        }

        [TargetRpc]
        public void TargetJoinRoom(Room room, NetworkCode code)
        {
            // Return error.
            if (room == null)
            {
                string message;
                switch (code)
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

        [TargetRpc]
        public void TargetLeaveRoom()
        {
            GameLogger.LogClient("Left room successfully!");
        }

        private void SetName(string oldName, string newName)
        {
            name = $"Client ({newName})";
        }
#endregion
    }
}
