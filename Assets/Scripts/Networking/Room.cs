using System;

namespace VampireVillage.Network
{
    public class Room
    {
        private static readonly Random rng = new Random();

        public Guid id { get; private set; }
        public string code;

        public Room() {}
        
        public Room(string code)
        {
            this.code = code;
            id = Guid.NewGuid();
        }

        public static string GenerateCode()
        {
            string code = "";
            for (int i = 0; i < 4; i++)
            {
                code += (char)(65 + rng.Next(0, 26));
            }
            return code;
        }
    }
}
