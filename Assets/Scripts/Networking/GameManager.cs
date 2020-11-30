using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

namespace VampireVillage.Network
{
    public class GameManager : NetworkBehaviour
    {
        private static readonly System.Random rng = new System.Random();

        public List<GameObject> spawnPoints;

        public Button leaveGameButton;

        private Room room;

        private readonly Dictionary<ServerPlayer, GameObject> gamePlayers = new Dictionary<ServerPlayer, GameObject>();

        private VampireVillageNetwork network;

        private void Awake()
        {
            network = VampireVillageNetwork.singleton as VampireVillageNetwork;
        }

#region Server Methods
        public override void OnStartServer()
        {
            network.roomManager.RegisterGameManager(this, gameObject.scene);
        }
        
        public void RegisterRoom(Room room)
        {
            this.room = room;
            GameLogger.LogServer("Game initialized!");
        }

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
                gamePlayers.Add(player, gamePlayerInstance);
                gamePlayer.role = i == vampireLordIndex ? GamePlayer.Role.VampireLord : GamePlayer.Role.Villager;

                // Move player to spawn points.
                // TODO: Make it random.
                gamePlayer.TargetSetPosition(spawnPoints[i % room.players.Count].transform.position);
            }
        }

        public void RemovePlayer(ServerPlayer player)
        {
            GameObject gamePlayerInstance = gamePlayers[player];
            gamePlayers.Remove(player);
            network.DestroyGamePlayer(gamePlayerInstance);
        }
#endregion

#region Client Methods
        public override void OnStartClient()
        {
            leaveGameButton.onClick.AddListener(LeaveGame);
        }

        private void LeaveGame()
        {
            Client.local.LeaveRoom();
        }
#endregion
    }
}
