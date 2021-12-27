using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using MilkSpun.Common;
using MilkSpun.CubeWorld.Managers;
using MilkSpun.CubeWorld.Models;
using MilkSpun.CubeWorld.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MilkSpun.CubeWorld
{
    public class World : IWorld
    {
        public Transform Transform => _worldObject.transform;
        public Vector3 Position => Transform.position;
        public Chunk[,] Chunks => _chunks;
        private static ChunkConfig ChunkConfig => GameManager.Instance.chunkConfig;

        private GameObject _worldObject;
        private readonly Chunk[,] _chunks;
        private readonly Vector3 _center;

        public World() : this(Vector3.zero) { }
        public World(Vector3 center)
        {
            _center = center;
            _chunks = new Chunk[ChunkConfig.chunkCoordSize, ChunkConfig.chunkCoordSize];
            GenerateWorld();
        }
        public World GenerateWorld()
        {
            GenerateWorldObject();
            _ = PopulateBottomLeftWorld();
            _ = PopulateBottomRightWorld();
            _ = PopulateTopLeftWorld();
            _ = PopulateTopRightWorld();
            return this;
        }

        public bool CheckPositionOnVoxel(float x, float y, float z)
        {
            var xChunk = Mathf.FloorToInt(x) / (ChunkConfig.chunkWidth-1);
            var zChunk = Mathf.FloorToInt(z) / (ChunkConfig.chunkWidth-1);
            if (xChunk < 0 ||
                xChunk > ChunkConfig.WorldSizeInVoxels - 1 ||
                zChunk < 0 ||
                zChunk > ChunkConfig.WorldSizeInVoxels - 1)
                return false;

            var chunk = GetChunkFromPosition(x, z);
            if (!chunk.IsPositionInChunk(x, y, z)) return false;

            var voxel = chunk.GetVoxelFromPosition(x, y, z);
            return voxel.GetVoxelConfig().isSolid;
        }

        private ref Chunk GetChunkFromPosition(float x, float z)
        {
            var xChunk = Mathf.FloorToInt(x) / (ChunkConfig.chunkWidth-1);
            var zChunk = Mathf.FloorToInt(z) / (ChunkConfig.chunkWidth-1);
            return ref _chunks[xChunk, zChunk];
        }

        private void GenerateWorldObject()
        {
            if (_worldObject)
            {
                Object.DestroyImmediate(_worldObject);
            }
            _worldObject = new GameObject("World")
            {
                transform =
                {
                    position = _center
                }
            };
        }
        private async Task PopulateBottomLeftWorld()
        {
            for (var z = ChunkConfig.chunkCoordSize / 2 - 1; z >= 0; z--)
            {
                for (var x = ChunkConfig.chunkCoordSize / 2 - 1; x >= 0; x--)
                {
                    await Task.Yield();
                    _chunks[x, z] = new Chunk(new ChunkCoord(x, z));
                }
            }
        }

        private async Task PopulateTopLeftWorld()
        {
            for (var z = ChunkConfig.chunkCoordSize / 2; z < ChunkConfig.chunkCoordSize; z++)
            {
                for (var x = ChunkConfig.chunkCoordSize / 2 - 1; x >= 0; x--)
                {
                    await Task.Yield();
                    _chunks[x, z] = new Chunk(new ChunkCoord(x, z));
                }
            }
        }
        private async Task PopulateTopRightWorld()
        {
            for (var z = ChunkConfig.chunkCoordSize / 2; z < ChunkConfig.chunkCoordSize; z++)
            {
                for (var x = ChunkConfig.chunkCoordSize / 2; x < ChunkConfig.chunkCoordSize; x++)
                {
                    await Task.Yield();
                    _chunks[x, z] = new Chunk(new ChunkCoord(x, z));
                }
            }
        }
        private async Task PopulateBottomRightWorld()
        {
            for (var z = ChunkConfig.chunkCoordSize / 2 - 1; z >= 0; z--)
            {
                for (var x = ChunkConfig.chunkCoordSize / 2; x < ChunkConfig.chunkCoordSize; x++)
                {
                    await Task.Yield();
                    _chunks[x, z] = new Chunk(new ChunkCoord(x, z));
                }
            }
        }

    }
}
