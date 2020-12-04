using System;
using Mirror;

namespace VampireVillage.Network
{
    /// <summary>
    /// Represents the player on the server.
    /// </summary>
    public class ServerPlayer : IEquatable<ServerPlayer>
    {
#region Properties
#region Client & Server Properties
        /// <summary>
        /// The player ID.
        /// </summary>
        public Guid id;
#endregion

#region Server-only Properties
#if UNITY_SERVER || UNITY_EDITOR
        /// <summary>
        /// The network connection ID.
        /// </summary>
        [NonSerialized]
        public int connectionId;

        /// <summary>
        /// The connection to client.
        /// </summary>
        [NonSerialized]
        public NetworkConnection clientConnection;

        /// <summary>
        /// The client that belongs to this player.
        /// </summary>
        [NonSerialized]
        public Client client;

        /// <summary>
        /// The room that the player is currently in.
        /// </summary>
        [NonSerialized]
        public Room room;
#endif
#endregion
#endregion

#region Methods
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
#endregion
    }
}
