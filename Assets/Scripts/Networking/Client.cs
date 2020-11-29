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

        [TargetRpc]
        public void TargetHostRoom(Room room)
        {
            GameLogger.LogClient($"Created new room!\nCode: {room.code}");
            CmdJoinRoom(room.code);
        }

        [TargetRpc]
        public void TargetJoinRoom(Room room)
        {
            if (room == null)
            {
                GameLogger.LogClient($"Failed to join the room.");
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
