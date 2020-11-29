using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;

namespace VampireVillage.Network
{
    /// <summary>
    /// Handles the lobby logics.
    /// On the server: Adding/removing player.
    /// On the client: Starting the game for the host.
    /// </summary>
    public class LobbyManager : NetworkBehaviour
    {
        [SyncVar(hook = nameof(UpdateRoom))]
        public Room room;

        [SyncVar(hook = nameof(UpdateHost))]
        public ServerPlayer host;

        public readonly SyncList<ServerPlayer> players = new SyncList<ServerPlayer>();

        public TMP_Text roomCodeText;
        public Button startGameButton;
        public Button leaveLobbyButton;

        private readonly Dictionary<ServerPlayer, GameObject> lobbyPlayers = new Dictionary<ServerPlayer, GameObject>();

        private VampireVillageNetwork network;

#if UNITY_EDITOR
        [NonSerialized]
        public NetworkMode mode = NetworkMode.Offline;
#endif

        private void Awake()
        {
            network = VampireVillageNetwork.singleton as VampireVillageNetwork;

            startGameButton.gameObject.SetActive(false);
        }

#region Server Methods
        public override void OnStartServer()
        {
#if UNITY_EDITOR
            mode = NetworkMode.Server;
#endif

            // Register the lobby manager to the server's room manager.
            network.roomManager.RegisterLobbyManager(this, gameObject.scene);
        }

        public void RegisterRoom(Room room)
        {
            this.room = room;
            SetHost(room.host);
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

        public void SetHost(ServerPlayer player)
        {
            host = player;
        }

        public void OnStartGame()
        {
            // Clean up lobby players.
            foreach (var lobbyPlayer in lobbyPlayers.Values)
            {
                network.DestroyLobbyPlayer(lobbyPlayer);
            }
        }
#endregion

#region Client Methods
        public override void OnStartClient()
        {
#if UNITY_EDITOR
            mode = NetworkMode.Client;
#endif

            // Add buttons listeners.
            startGameButton.onClick.AddListener(StartGame);
            leaveLobbyButton.onClick.AddListener(LeaveRoom);
        }

        private void StartGame()
        {
            Client.local.StartGame();
        }

        private void LeaveRoom()
        {
            Client.local.LeaveRoom();
        }

        private void UpdateRoom(Room oldRoom, Room newRoom)
        {
            roomCodeText.text = newRoom.code;
        }

        private void UpdateHost(ServerPlayer oldHost, ServerPlayer newHost)
        {
            // Display the start game button if local client is the new host.
            if (newHost.id == Client.local.playerId)
                startGameButton.gameObject.SetActive(true);
        }
#endregion
    }
}
