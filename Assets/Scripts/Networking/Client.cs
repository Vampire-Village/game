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

        private VampireVillageNetwork network;

        private void Start()
        {
            network = VampireVillageNetwork.singleton as VampireVillageNetwork;

            if (isLocalPlayer)
            {
                // Set this client as the local client.
                instance = this;

                // Set player gameobject name.
                name = $"Client [LOCAL]";

                GameLogger.LogClient("Client connected to the server.", this);
            }
            else
            {
                name = $"Client ({playerId})";
            }
        }

        [Command]
        public void CmdHostRoom()
        {
            GameLogger.LogServer("A client requested a new room.", this);
            network.CreateRoom(connectionToClient);
        }

        [TargetRpc]
        public void TargetHostRoom(Room room)
        {
            GameLogger.LogClient($"Created new room!\nCode: {room.code}");
            CmdJoinRoom(room.code);
        }

        [Command]
        public void CmdJoinRoom(string roomCode)
        {
            GameLogger.LogServer("A client requested to join a room.\nCode: {roomCode}", this);
            network.JoinRoom(connectionToClient, roomCode);
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
    }
}
