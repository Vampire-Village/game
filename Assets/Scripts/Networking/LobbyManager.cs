using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace VampireVillage.Network
{
    public class LobbyManager : NetworkBehaviour
    {
        private Room room;

        private VampireVillageNetwork network;

        public void Awake()
        {
            network = VampireVillageNetwork.singleton as VampireVillageNetwork;
        }

        public override void OnStartServer()
        {
            // Register the Lobby Manager to the server's Room Manager.
            network.roomManager.RegisterLobbyManager(this, gameObject.scene);
        }

        public void RegisterRoom(Room room)
        {
            this.room = room;
        }
    }
}
