using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MilkSpun.CubeWorld.Models
{
    [System.Serializable]
    public struct ChunkCoord
    {
        public int x;
        public int z;

        public ChunkCoord(int x, int z)
        {
            this.x = x;
            this.z = z;
        }

        public override string ToString()
        {
            return $"ChunkCoord ({x},{z}) ";
        }

        public static bool operator ==(ChunkCoord a, ChunkCoord b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(ChunkCoord a, ChunkCoord b)
        {
            return !(a == b);
        }
        public bool Equals(ChunkCoord other)
        {
            return x == other.x && z == other.z;
        }
        public override bool Equals(object obj)
        {
            return obj is ChunkCoord other && Equals(other);
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(x, z);
        }
    }
}
