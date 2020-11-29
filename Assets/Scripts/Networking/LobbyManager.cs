using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;

namespace VampireVillage.Network
{
    public class LobbyManager : NetworkBehaviour
    {
        [SyncVar(hook = nameof(UpdateRoom))]
        public Room room;

        public readonly SyncList<ServerPlayer> players = new SyncList<ServerPlayer>();

        public TMP_Text roomCodeText;
        public Button leaveLobbyButton;

        private readonly Dictionary<ServerPlayer, GameObject> lobbyPlayers = new Dictionary<ServerPlayer, GameObject>();

        private VampireVillageNetwork network;

#if UNITY_EDITOR
        [NonSerialized]
        public NetworkMode mode = NetworkMode.Offline;
#endif

        public void Awake()
        {
            network = VampireVillageNetwork.singleton as VampireVillageNetwork;
        }

        public override void OnStartServer()
        {
#if UNITY_EDITOR
            mode = NetworkMode.Server;
#endif

            // Register the Lobby Manager to the server's Room Manager.
            network.roomManager.RegisterLobbyManager(this, gameObject.scene);
        }

        public override void OnStartClient()
        {
#if UNITY_EDITOR
            mode = NetworkMode.Client;
#endif

            leaveLobbyButton.onClick.AddListener(Client.local.CmdLeaveRoom);
        }

        public void RegisterRoom(Room room)
        {
            this.room = room;
        }

        public void AddPlayer(ServerPlayer player)
        {
            players.Add(player);
            GameObject lobbyPlayerInstance = network.InstantiateLobbyPlayer(gameObject, player);
            lobbyPlayers.Add(player, lobbyPlayerInstance);
        }

        public void RemovePlayer(ServerPlayer player)
        {
            players.Remove(player);
            GameObject lobbyPlayerInstance = lobbyPlayers[player];
            lobbyPlayers.Remove(player);
            network.DestroyLobbyPlayer(lobbyPlayerInstance);
        }

        public void UpdateRoom(Room oldRoom, Room newRoom)
        {
            roomCodeText.text = newRoom.code;
        }
    }
}
