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
#region Client & Server Properties
        /// <summary>
        /// The room that this lobby belongs to.
        /// </summary>
        [SyncVar(hook = nameof(UpdateRoom))]
        public Room room;

        /// <summary>
        /// The current host of the lobby.
        /// </summary>
        [SyncVar(hook = nameof(UpdateHost))]
        public ServerPlayer host;

        /// <summary>
        /// List of players in the lobby.
        /// </summary>
        public readonly SyncList<ServerPlayer> players = new SyncList<ServerPlayer>();

#region UIs
        public TMP_Text roomCodeText;
        public TMP_Text playerCountText;
        public Button startGameButton;
        private TMP_Text startGameButtonText;
        public Button leaveLobbyButton;
        public GameObject popupPanel;
        public TMP_Text popupText;
        public Button popupButton;
#endregion

        public ChatSystem chatSystem;
        private VampireVillageNetwork network;
#endregion

#region Server-only Properties
#if UNITY_SERVER || UNITY_EDITOR
        private readonly Dictionary<ServerPlayer, GameObject> lobbyPlayers = new Dictionary<ServerPlayer, GameObject>();

#if UNITY_EDITOR
        [NonSerialized]
        public NetworkMode mode = NetworkMode.Offline;
#endif
#endif
#endregion
#endregion

#region Unity Methods
        private void Awake()
        {
            network = VampireVillageNetwork.singleton as VampireVillageNetwork;

            startGameButton.gameObject.SetActive(false);
        }
#endregion

#region Server Methods
#if UNITY_SERVER || UNITY_EDITOR
        public override void OnStartServer()
        {
#if UNITY_EDITOR
            mode = NetworkMode.Server;
#endif

            // Register the lobby manager to the server's room manager.
            network.roomManager.RegisterLobbyManager(this, gameObject.scene);

            // Stop rendering UI.
            roomCodeText.gameObject.SetActive(false);
            startGameButton.gameObject.SetActive(false);
            leaveLobbyButton.gameObject.SetActive(false);
        }

        /// <summary>
        /// Registers the room that this lobby manager belongs to.
        /// </summary>
        /// <param name="room">A room.</param>
        public void RegisterRoom(Room room)
        {
            this.room = room;
            SetHost(room.host);
        }

        /// <summary>
        /// Adds a player to the lobby.
        /// </summary>
        /// <param name="player">A player.</param>
        public void AddPlayer(ServerPlayer player)
        {
            players.Add(player);
            GameObject lobbyPlayerInstance = network.InstantiateLobbyPlayer(gameObject, player);
            chatSystem.AddPlayer(player);
            lobbyPlayers.Add(player, lobbyPlayerInstance);
        }

        /// <summary>
        /// Removes a player from the lobby.
        /// </summary>
        /// <param name="player">A player.</param>
        public void RemovePlayer(ServerPlayer player)
        {
            players.Remove(player);
            GameObject lobbyPlayerInstance = lobbyPlayers[player];
            chatSystem.RemovePlayer(player);
            lobbyPlayers.Remove(player);
            network.DestroyLobbyPlayer(lobbyPlayerInstance);
        }

        /// <summary>
        /// Sets the lobby host.
        /// </summary>
        /// <param name="player">A player.</param>
        public void SetHost(ServerPlayer player)
        {
            host = player;
        }

        /// <summary>
        /// Called when game is about to start to remove the lobby player objects.
        /// </summary>
        public void OnStartGame()
        {
            // Clean up lobby players.
            foreach (var lobbyPlayer in lobbyPlayers.Values)
            {
                network.DestroyLobbyPlayer(lobbyPlayer);
            }
        }
#endif
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
            startGameButtonText = startGameButton.GetComponentInChildren<TMP_Text>();

            // Set the player count text.
            playerCountText.text = $"{players.Count.ToString()}/10";
            players.Callback += UpdatePlayerCount;
        }

        private void StartGame()
        {
            Client.local.StartGame(OnStartGameError);
        }

        private void OnStartGameError(string errorMessage)
        {
            ShowPopup(errorMessage);
        }

        /// <summary>
        /// Called when game is about to start and let all the players know.
        /// </summary>
        [ClientRpc]
        public void RpcOnGameStarting()
        {
            GameLogger.LogClient("Game is starting soon!");
            startGameButtonText.text = "Game starting...";
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

        private void UpdatePlayerCount(SyncList<ServerPlayer>.Operation op, int index, ServerPlayer oldPlayer, ServerPlayer newPlayer)
        {
            playerCountText.text = $"{players.Count.ToString()}/10";
        }
#endregion
    }
}
