using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace VampireVillage.Network
{
    public class Room
    {
        private static readonly Random rng = new Random();

        public string code;
        public Scene lobbyScene;
        public bool isRoomInitialized = false;
        public RoomState state = RoomState.Lobby;

        [System.NonSerialized]
        public readonly List<ServerPlayer> players = new List<ServerPlayer>();

        [System.NonSerialized]
        public LobbyManager lobbyManager;

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
