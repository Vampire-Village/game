﻿using System;

namespace VampireVillage.Network
{
    public class ServerPlayer : IEquatable<ServerPlayer>
    {
        public Guid id { get; private set; }
        public int connectionId { get; set; }

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