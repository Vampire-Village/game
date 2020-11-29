using System;
using Mirror;

namespace VampireVillage.Network
{
    /// <summary>
    /// Represents the player on the server.
    /// </summary>
    public class ServerPlayer : IEquatable<ServerPlayer>
    {
#region Client & Server Properties
        /// <summary>
        /// The player ID.
        /// </summary>
        public Guid id;

        /// <summary>
        /// The network connection ID.
        /// </summary>
        public int connectionId;
#endregion

#region Server-only Properties
        [NonSerialized]
        public NetworkConnection clientConnection;

        [NonSerialized]
        public Client client;

        [NonSerialized]
        public Room room;
#endregion

        public ServerPlayer()
        {
            id = Guid.NewGuid();
        }

        public override int GetHashCode()
        {
            return id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is ServerPlayer && Equals(obj);
        }

        public bool Equals(ServerPlayer p)
        {
            return id == p.id;
        }
    }
}
