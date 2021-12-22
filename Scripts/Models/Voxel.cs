using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MilkSpun.CubeWorld.Models
{
    public struct Voxel
    {
        public readonly int X;
        public readonly int Y;
        public readonly int Z;
        public VoxelType VoxelType;
        public ChunkCoord ChunkCoord { get; }
        public Vector3 LocalPos => new (X, Y, Z);
        
        public Voxel(
            in ChunkCoord chunkCoord,
            int x,
            int y,
            int z,
            VoxelType voxelType = VoxelType.Air)
        {
            X = x;
            Y = y;
            Z = z;
            VoxelType = voxelType;
            ChunkCoord = chunkCoord;
        }

    }
}
