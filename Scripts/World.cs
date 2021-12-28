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
        public int MiddleCoord { get; }

        private static ChunkConfig ChunkConfig => GameManager.Instance.chunkConfig;

        private GameObject _worldObject;
        private readonly Chunk[,] _chunks;
        private readonly Vector3 _center;

        public World() : this(Vector3.zero) { }
        public World(Vector3 center)
        {
            _center = center;
            MiddleCoord = Mathf.FloorToInt((float)ChunkConfig.chunkCoordSize / 2);
            _chunks = new Chunk[ChunkConfig.chunkCoordSize, ChunkConfig.chunkCoordSize];
        }
        public async Task<World> GenerateWorld()
        {
            GenerateWorldObject();
            await PopulateMiddleChunk();
            _ = PopulateBottomLeftWorld();
            _ = PopulateBottomRightWorld();
            _ = PopulateTopLeftWorld();
            _ = PopulateTopRightWorld();
            return this;
        }

        public bool CheckPositionOnGround(float x, float y, float z)
        {
            var xChunk = Mathf.FloorToInt(x / ChunkConfig.chunkWidth);
            var zChunk = Mathf.FloorToInt(z / ChunkConfig.chunkWidth);
            if (xChunk < 0 ||
                xChunk > ChunkConfig.chunkCoordSize - 1 ||
                zChunk < 0 ||
                zChunk > ChunkConfig.chunkCoordSize - 1)
                return false;

            var chunk = GetChunkFromPosition(x, z);
            if (!chunk.IsPositionInChunk(x, y, z)) return false;

            var voxel = chunk.GetVoxelFromPosition(x, y, z);
            return voxel.GetVoxelConfig().isSolid;
        }

        private async Task PopulateMiddleChunk()
        {
            _chunks[MiddleCoord, MiddleCoord] =
                await new Chunk(new ChunkCoord(MiddleCoord, MiddleCoord), this).CreateChunk();
        }

        private ref Chunk GetChunkFromPosition(float x, float z)
        {
            var xChunk = Mathf.FloorToInt(x / ChunkConfig.chunkWidth);
            var zChunk = Mathf.FloorToInt(z / ChunkConfig.chunkWidth);
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
            for (var z = MiddleCoord; z >= 0; z--)
            {
                for (var x = MiddleCoord - 1; x >= 0; x--)
                {
                    _chunks[x, z] = await new Chunk(new ChunkCoord(x, z), this).CreateChunk();
                }
            }
        }

        private async Task PopulateTopLeftWorld()
        {
            for (var z = MiddleCoord + 1; z < ChunkConfig.chunkCoordSize; z++)
            {
                for (var x = MiddleCoord; x >= 0; x--)
                {
                    _chunks[x, z] = await new Chunk(new ChunkCoord(x, z), this).CreateChunk();
                }
            }
        }
        private async Task PopulateTopRightWorld()
        {
            for (var z = MiddleCoord; z < ChunkConfig.chunkCoordSize; z++)
            {
                for (var x = MiddleCoord + 1; x < ChunkConfig.chunkCoordSize; x++)
                {
                    _chunks[x, z] = await new Chunk(new ChunkCoord(x, z), this).CreateChunk();
                }
            }
        }
        private async Task PopulateBottomRightWorld()
        {
            for (var z = MiddleCoord - 1; z >= 0; z--)
            {
                for (var x = MiddleCoord; x < ChunkConfig.chunkCoordSize; x++)
                {
                    _chunks[x, z] = await new Chunk(new ChunkCoord(x, z), this).CreateChunk();
                }
            }
        }

    }
}
