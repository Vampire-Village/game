using System;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace VampireVillage.Network
{
    public class GameManager : NetworkBehaviour
    {
        private static readonly System.Random rng = new System.Random();

        public List<GameObject> spawnPoints;

        private Room room;

        private VampireVillageNetwork network;

        private void Awake()
        {
            network = VampireVillageNetwork.singleton as VampireVillageNetwork;
        }

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
                gamePlayer.role = i == vampireLordIndex ? GamePlayer.Role.VampireLord : GamePlayer.Role.Villager;

                // Move player to spawn points.
                // TODO: Make it random.
                gamePlayerInstance.transform.position = spawnPoints[i % room.players.Count].transform.position;
            }
        }
    }
}
