using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace VampireVillage.Network
{
    public class Room
    {
        private static readonly Random rng = new Random();

#region Client & Server Properties
        /// <summary>
        /// The room code that players can use to join.
        /// </summary>
        public string code;

        /// <summary>
        /// The host player.
        /// </summary>
        public ServerPlayer host;

        /// <summary>
        /// Whether the room is currently in the lobby or in the game.
        /// </summary>
        public RoomState state = RoomState.Lobby;

        /// <summary>
        /// The current room scene.
        /// </summary>
        public Scene scene;
#endregion

#region Server-only Properties
        [NonSerialized]
        public bool isRoomInitialized = false;

        [NonSerialized]
        public readonly List<ServerPlayer> players = new List<ServerPlayer>();

        [NonSerialized]
        public LobbyManager lobbyManager;
#endregion

        public Room() {}
        
        public Room(string code)
        {
            this.code = code;
        }

        public static string GenerateCode(uint length)
        {
            string code = "";
            for (uint i = 0; i < length; i++)
            {
                code += (char)(65 + rng.Next(0, 26));
            }
            return code;
        }
    }
}
