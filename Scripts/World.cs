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
        private static Biome Biome => GameManager.Instance.Biome;
        private static int NoiseResolution => ChunkConfig.chunkWidth;

        private GameObject _worldObject;
        private readonly Chunk[,] _chunks;
        public Vector3 Center { get; }

        public World() : this(Vector3.zero) { }
        public World(Vector3 center)
        {
            Center = center;
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

        public static VoxelType GenerateVoxelType(Vector3 pos)
        {
            //默认行为
            var y = Mathf.FloorToInt(pos.y);
            if (y == 0) return VoxelType.BedRock;

            // 基础地形
            var noise =
                NoiseGenerator.Get2DPerlinNoise(new Vector2(pos.x, pos.z), 0, Biome.scale,
                    NoiseResolution) *
                Biome.terrainHeight;
            var terrainHeight = Mathf.FloorToInt(noise) + Biome.solidGroundHeight;
            if (y > terrainHeight)
            {
                return VoxelType.Air;
            }

            var voxelType = VoxelType.Air;

            if (y == terrainHeight)
            {
                voxelType = VoxelType.Grass;
            }
            if (y < terrainHeight)
            {
                voxelType = y > terrainHeight - 5 ? VoxelType.Dirt : VoxelType.Stone;
            }

            //生物多样性地形

            if (voxelType == VoxelType.Stone)
            {
                foreach (var lode in Biome.lodes)
                {
                    if (y < lode.minHeight || y > lode.maxHeight) continue;

                    if (NoiseGenerator.Get3DPerlinNoise(pos, lode.offset, lode.scale,
                            lode.threshold, 1))
                    {
                        voxelType = lode.voxelType;
                    }
                }
            }
            return voxelType;
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
            if (_chunks[xChunk, zChunk] is null)
            {
                _chunks[xChunk, zChunk] = new Chunk(new ChunkCoord(xChunk, zChunk), this);
            }
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
                    position = Center
                }
            };
        }
        private async Task PopulateBottomLeftWorld()
        {
            for (var z = MiddleCoord; z >= 0; z--)
            {
                for (var x = MiddleCoord - 1; x >= 0; x--)
                {
                    _chunks[x, z] = _chunks[x, z] is null
                        ? await new Chunk(new ChunkCoord(x, z), this).CreateChunk()
                        : await _chunks[x, z].CreateChunk();
                }
            }
        }

        private async Task PopulateTopLeftWorld()
        {
            for (var z = MiddleCoord + 1; z < ChunkConfig.chunkCoordSize; z++)
            {
                for (var x = MiddleCoord; x >= 0; x--)
                {
                    _chunks[x, z] = _chunks[x, z] is null
                        ? await new Chunk(new ChunkCoord(x, z), this).CreateChunk()
                        : await _chunks[x, z].CreateChunk();
                }
            }
        }
        private async Task PopulateTopRightWorld()
        {
            for (var z = MiddleCoord; z < ChunkConfig.chunkCoordSize; z++)
            {
                for (var x = MiddleCoord + 1; x < ChunkConfig.chunkCoordSize; x++)
                {
                    _chunks[x, z] = _chunks[x, z] is null
                        ? await new Chunk(new ChunkCoord(x, z), this).CreateChunk()
                        : await _chunks[x, z].CreateChunk();
                }
            }
        }
        private async Task PopulateBottomRightWorld()
        {
            for (var z = MiddleCoord - 1; z >= 0; z--)
            {
                for (var x = MiddleCoord; x < ChunkConfig.chunkCoordSize; x++)
                {
                    _chunks[x, z] = _chunks[x, z] is null
                        ? await new Chunk(new ChunkCoord(x, z), this).CreateChunk()
                        : await _chunks[x, z].CreateChunk();
                }
            }
        }

    }
}
