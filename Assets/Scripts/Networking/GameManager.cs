using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;

namespace VampireVillage.Network
{
    /// <summary>
    /// Handles the game logics.
    /// On the server: Adding/removing player, checking win condition.
    /// On the client: Leaving room, UI logics.
    /// </summary>
    public class GameManager : NetworkBehaviour
    {
#region Properties
        public static GameManager local;
        private static readonly System.Random rng = new System.Random();

#region Client & Server Properties
#region UIs
        public Button leaveGameButton;
        public GameObject gameOverCanvas;
        public TMP_Text winText;
        public TMP_Text announcementText;
#endregion

        private MeetingManager meetingManager;
        private VampireVillageNetwork network;
#endregion

#region Server-only Properties
#if UNITY_SERVER || UNITY_EDITOR
        /// <summary>
        /// List of spawn points.
        /// </summary>
        public GameObject spawnPointGroup;
        private readonly List<GameObject> spawnPoints = new List<GameObject>();

        /// <summary>
        /// How long the night time should be.
        /// </summary>
        public float nightLength = 300.0f;
        private Coroutine nightCoroutine;

        private Room room;
        private bool isGameOver = false;
        private readonly Dictionary<ServerPlayer, GamePlayer> gamePlayers = new Dictionary<ServerPlayer, GamePlayer>();

        private GamePlayer vampireLord;
        private readonly List<GamePlayer> vampires = new List<GamePlayer>();
        private readonly List<GamePlayer> villagers = new List<GamePlayer>();
#endif
#endregion
#endregion

#region Unity Methods
        private void Awake()
        {
            network = VampireVillageNetwork.singleton as VampireVillageNetwork;
            meetingManager = GetComponent<MeetingManager>();
            announcementText.gameObject.SetActive(false);
        }
#endregion

#region Server Methods
#if UNITY_SERVER || UNITY_EDITOR
        public override void OnStartServer()
        {
            network.roomManager.RegisterGameManager(this, gameObject.scene);
            foreach (Transform spawnPoint in spawnPointGroup.transform)
                spawnPoints.Add(spawnPoint.gameObject);
            StartNight();
        }
        
        /// <summary>
        /// Registers the room that this game manager belongs to.
        /// </summary>
        /// <param name="room">A room.</param>
        public void RegisterRoom(Room room)
        {
            this.room = room;
            GameLogger.LogServer("Game initialized!");
        }

        /// <summary>
        /// Initializes the game.
        /// </summary>
        public void StartGame()
        {
            // Randomly choose a vampire lord.
            int vampireLordIndex = rng.Next(0, room.players.Count);

            // Spawn the players.
            for (int i = 0; i < room.players.Count; i++)
            {
                ServerPlayer player = room.players[i];

                // Instantiate the game player and assign role.
                GameObject gamePlayerInstance = network.InstantiateGamePlayer(gameObject, player);
                GamePlayer gamePlayer = gamePlayerInstance.GetComponent<GamePlayer>();
                gamePlayers.Add(player, gamePlayer);
                gamePlayer.RegisterGameManager(this);
                gamePlayer.role = i == vampireLordIndex ? Role.VampireLord : Role.Villager;

                // Add game player to their team list.
                if (gamePlayer.role == Role.VampireLord)
                {
                    vampireLord = gamePlayer;
                    vampires.Add(gamePlayer);
                }
                else
                    villagers.Add(gamePlayer);

                // Move player to spawn points.
                // TODO: Make it random.
                gamePlayer.TargetSetPosition(spawnPoints[i % spawnPoints.Count].transform.position);
            }
        }

        /// <summary>
        /// Removes a player from the game.
        /// </summary>
        /// <param name="player">A player.</param>
        public void RemovePlayer(ServerPlayer player)
        {
            // Get the game player and remove it from the game players list.
            GamePlayer gamePlayer = gamePlayers[player];
            gamePlayers.Remove(player);

            // Also remove the player from its role list.
            if (gamePlayer.role == Role.Villager)
            {
                villagers.Remove(gamePlayer);
            }
            else if (gamePlayer.role == Role.Infected || gamePlayer.role == Role.VampireLord)
            {
                vampires.Remove(gamePlayer);
            }

            // Check the winning condition if game is not over yet.
            if (!isGameOver)
            {
                if (gamePlayer == vampireLord)
                    GameOver(Team.Villagers);
                else if (villagers.Count <= 1)
                    GameOver(Team.Vampires);
            }

            // Destroy the game object for this player.
            network.DestroyGamePlayer(gamePlayer.gameObject);
        }

        /// <summary>
        /// Updates a player team.
        /// </summary>
        /// <param name="player">A player.</param>
        /// <param name="oldRole">The player's old role.</param>
        /// <param name="newRole">The player's new role.</param>
        public void UpdatePlayerTeam(GamePlayer player, Role oldRole, Role newRole)
        {
            // Update the player team.
            if (oldRole == Role.Villager && newRole == Role.Infected)
            {
                villagers.Remove(player);
                vampires.Add(player);
            }

            // Check for win condition.
            if (villagers.Count <= 1)
                GameOver(Team.Vampires);
        }

        /// <summary>
        /// Finishes the game.
        /// </summary>
        /// <param name="winningTeam">The winning team.</param>
        public void GameOver(Team winningTeam)
        {
            isGameOver = true;
            RpcOnGameOver(winningTeam);
            GameLogger.LogServer($"Game over triggered for room {room.code}.\nWinning Team: {winningTeam.ToString()}");
        }

        /// <summary>
        /// Starts the night cycle.
        /// </summary>
        public void StartNight()
        {
            if (nightCoroutine != null)
                StopNight();
            nightCoroutine = StartCoroutine(StartNight(nightLength));
        }

        private IEnumerator StartNight(float length)
        {
            // TODO: Make it look like night in the game.

            // Wait until cycle completes.
            yield return new WaitForSeconds(length);

            // Call meeting when the night ends.
            meetingManager.StartMeeting();
        }

        /// <summary>
        /// Stops the night cycle.
        /// </summary>
        public void StopNight()
        {
            if (nightCoroutine != null)
            {
                StopCoroutine(nightCoroutine);
                nightCoroutine = null;
            }

            // TODO: Make it look like day in the game.
        }
#endif
#endregion

#region Client Methods
        public override void OnStartClient()
        {
            local = this;

            leaveGameButton.onClick.AddListener(LeaveGame);

            // Announce player role at the start.
            GamePlayer.OnRoleUpdated.AddListener(AnnouncePlayerRole);
            AnnouncePlayerRole();
        }

        private void LeaveGame()
        {
            Client.local.LeaveRoom();
        }

        /// <summary>
        /// Called when the game is over and let all the players know.
        /// </summary>
        /// <param name="winningTeam">The winning team.</param>
        [ClientRpc]
        public void RpcOnGameOver(Team winningTeam)
        {
            GameLogger.LogClient($"Game over! {winningTeam.ToString()} win!");
            gameOverCanvas.SetActive(true);
            winText.text = $"{winningTeam.ToString()} win!";
        }

        public void Announce(string message, float time = 5.0f)
        {
            StartCoroutine(AnnounceAsync(message, time));
        }

        private IEnumerator AnnounceAsync(string message, float time)
        {
            announcementText.text = message;
            announcementText.gameObject.SetActive(true);
            yield return new WaitForSeconds(time);
            announcementText.gameObject.SetActive(false);
        }

        public void AnnouncePlayerRole()
        {
            Role role = GamePlayer.local.role;
            string message = "";
            switch (role)
            {
                case Role.Villager:
                    message = "You are a villager!";
                    break;
                case Role.VampireLord:
                    message = "You are the Vampire Lord!";
                    break;
                case Role.Infected:
                    message = "You got infected!";
                    break;
            }
            Announce(message);
        }

        public override void OnStopClient()
        {
            GamePlayer.OnRoleUpdated.RemoveListener(AnnouncePlayerRole);
        }
#endregion
    }
}
