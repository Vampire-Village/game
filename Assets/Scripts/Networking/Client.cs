using System;
using Mirror;

namespace VampireVillage.Network
{
    public class Client : NetworkBehaviour
    {
        public static Client instance { get; private set; }

        [SyncVar]
        public Guid playerId;

        [SyncVar(hook = nameof(SetName))]
        public string playerName;

        private NetworkMatchChecker matchChecker;
        private VampireVillageNetwork network;

        private void Start()
        {
            network = VampireVillageNetwork.singleton as VampireVillageNetwork;
            matchChecker = GetComponent<NetworkMatchChecker>();

            if (isLocalPlayer)
            {
                // Set this client as the local client.
                instance = this;

                // Initialize the matchId as playerId (so the client is alone at the start).
                SetMatchID(playerId);
                name = $"Client ({playerId})";

                GameLogger.LogClient("Client connected to the server.", this);
            }

            if (network.mode == NetworkManagerMode.ClientOnly)
                DontDestroyOnLoad(gameObject);
        }

        [Command]
        public void CmdHostRoom()
        {
            GameLogger.LogServer("A client requested a new room.", this);
            Room room = network.CreateRoom(connectionToClient);
            TargetHostRoom(room);
        }

        [TargetRpc]
        public void TargetHostRoom(Room room)
        {
            GameLogger.LogClient($"Created new room!\nCode: {room.code}");
        }

        [Command]
        public void CmdJoinRoom(string roomCode)
        {
            GameLogger.LogServer("A client requested to join a room.\nCode: {roomCode}", this);
            Room room = network.JoinRoom(connectionToClient, roomCode);
            if (room != null)
                TargetJoinRoom(room);
        }

        [TargetRpc]
        public void TargetJoinRoom(Room room)
        {
            GameLogger.LogClient($"Joined a room!\nCode: {room.code}");
        }

        [Command]
        public void CmdSetName(string newName)
        {
            // TODO: Some sort of name validation.
            playerName = newName;
            name = $"Client ({newName})";
        }

        public void SetName(string oldName, string newName)
        {
            name = $"Client ({newName})";
            if (isLocalPlayer)
                name += " [LOCAL]";
        }

        public void SetMatchID(Guid id)
        {
            matchChecker.matchId = id;
        }
    }
}
