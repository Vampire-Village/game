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
    /// On the client: Starting the game for the host, leaving room, UI logics.
    /// </summary>
    public class LobbyManager : NetworkBehaviour
    {
#region Properties
        [SyncVar(hook = nameof(UpdateRoom))]
        public Room room;

        [SyncVar(hook = nameof(UpdateHost))]
        public ServerPlayer host;

        public readonly SyncList<ServerPlayer> players = new SyncList<ServerPlayer>();

        public TMP_Text roomCodeText;
        public Button startGameButton;
        public Button leaveLobbyButton;
        public GameObject popupPanel;
        public TMP_Text popupText;
        public Button popupButton;

        private readonly Dictionary<ServerPlayer, GameObject> lobbyPlayers = new Dictionary<ServerPlayer, GameObject>();

        private VampireVillageNetwork network;

#if UNITY_EDITOR
        [NonSerialized]
        public NetworkMode mode = NetworkMode.Offline;
#endif
#endregion

#region Unity Methods
        private void Awake()
        {
            network = VampireVillageNetwork.singleton as VampireVillageNetwork;

            startGameButton.gameObject.SetActive(false);
        }
#endregion

#region Server Methods
        public override void OnStartServer()
        {
#if UNITY_EDITOR
            mode = NetworkMode.Server;
#endif

#if UNITY_SERVER || UNITY_EDITOR
            // Register the lobby manager to the server's room manager.
            network.roomManager.RegisterLobbyManager(this, gameObject.scene);

            // Stop rendering UI.
            roomCodeText.gameObject.SetActive(false);
            startGameButton.gameObject.SetActive(false);
            leaveLobbyButton.gameObject.SetActive(false);
#endif
        }

        public void RegisterRoom(Room room)
        {
#if UNITY_SERVER || UNITY_EDITOR
            this.room = room;
            SetHost(room.host);
#endif
        }

        public void AddPlayer(ServerPlayer player)
        {
#if UNITY_SERVER || UNITY_EDITOR
            players.Add(player);
            GameObject lobbyPlayerInstance = network.InstantiateLobbyPlayer(gameObject, player);
            lobbyPlayers.Add(player, lobbyPlayerInstance);
#endif
        }

        public void RemovePlayer(ServerPlayer player)
        {
#if UNITY_SERVER || UNITY_EDITOR
            players.Remove(player);
            GameObject lobbyPlayerInstance = lobbyPlayers[player];
            lobbyPlayers.Remove(player);
            network.DestroyLobbyPlayer(lobbyPlayerInstance);
#endif
        }

        public void SetHost(ServerPlayer player)
        {
#if UNITY_SERVER || UNITY_EDITOR
            host = player;
#endif
        }

        public void OnStartGame()
        {
#if UNITY_SERVER || UNITY_EDITOR
            // Clean up lobby players.
            foreach (var lobbyPlayer in lobbyPlayers.Values)
            {
                network.DestroyLobbyPlayer(lobbyPlayer);
            }
#endif
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
            popupButton.onClick.AddListener(() => popupPanel.SetActive(false));
        }

        private void StartGame()
        {
            Client.local.StartGame(OnStartGameError);
        }

        private void OnStartGameError(string errorMessage)
        {
            ShowPopup(errorMessage);
        }

        [ClientRpc]
        public void RpcOnGameStarting()
        {
            GameLogger.LogClient("Game is starting soon!");
            startGameButton.interactable = false;
            leaveLobbyButton.interactable = false;
        }

        private void LeaveRoom()
        {
            Client.local.LeaveRoom(OnLeaveRoomError);
        }

        private void OnLeaveRoomError(string errorMessage)
        {
            ShowPopup(errorMessage);
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

        private void ShowPopup(string message)
        {
            popupText.text = message;
            popupPanel.SetActive(true);
        }
#endregion
    }
}
