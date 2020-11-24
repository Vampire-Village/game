using System;
using Mirror;

namespace VampireVillage.Network
{
    public class ServerPlayer : IEquatable<ServerPlayer>
    {
        public Guid id;
        public int connectionId;

        [NonSerialized]
        public NetworkConnection clientConnection;

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
