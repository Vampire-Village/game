using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace VampireVillage.Network
{
    public class Room
    {
#region Properties
        private static readonly Random rng = new Random();

#region Client & Server Properties
        /// <summary>
        /// The room code that players can use to join.
        /// </summary>
        public string code;
#endregion

#region Server-only Properties
#if UNITY_SERVER || UNITY_EDITOR
        /// <summary>
        /// List of players in the room.
        /// </summary>
        [NonSerialized]
        public readonly List<ServerPlayer> players = new List<ServerPlayer>();

        /// <summary>
        /// The host player.
        /// </summary>
        [NonSerialized]
        public ServerPlayer host;

        /// <summary>
        /// The current room scene.
        /// </summary>
        [NonSerialized]
        public Scene scene;

        /// <summary>
        /// Whether the room is currently in the lobby or in the game.
        /// </summary>
        [NonSerialized]
        public RoomState state = RoomState.Lobby;

        /// <summary>
        /// The lobby manager that belongs to this room.
        /// </summary>
        [NonSerialized]
        public LobbyManager lobbyManager;
        
        /// <summary>
        /// Whether the lobby has been initialized.
        /// </summary>
        [NonSerialized]
        public bool isLobbyInitialized = false;

        /// <summary>
        /// The game manager that belongs to this room.
        /// </summary>
        [NonSerialized]
        public GameManager gameManager;

        /// <summary>
        /// Whether the game has been initialized.
        /// </summary>
        [NonSerialized]
        public bool isGameInitialized = false;
#endif
#endregion
#endregion

#region Methods
        public Room() {}
        
        public Room(string code)
        {
            this.code = code;
        }

        /// <summary>
        /// Generate an alphabetic room code with the specified length.
        /// </summary>
        /// <param name="length">Length of the code.</param>
        /// <returns>Room code.</returns>
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
#endregion
}
