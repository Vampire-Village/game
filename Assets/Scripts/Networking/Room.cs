using System;
using UnityEngine.SceneManagement;

namespace VampireVillage.Network
{
    public class Room
    {
        private static readonly Random rng = new Random();

        public Guid id { get; private set; }
        public string code;
        public Scene lobbyScene;

        public Room() {}
        
        public Room(string code)
        {
            this.code = code;
            id = Guid.NewGuid();
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
