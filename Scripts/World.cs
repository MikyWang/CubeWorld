using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
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
        public Transform Transform => _worldRenderer.transform;
        public Vector3 Position => Transform.position;
        public Chunk[,] Chunks => _chunks;
        public int MiddleCoord { get; }

        private static ChunkConfig ChunkConfig => GameManager.Instance.chunkConfig;
        private static List<Biome> Biomes => GameManager.Instance.Biomes;
        private static int NoiseResolution => ChunkConfig.chunkWidth;
        private static WorldRenderer WorldPrefab => GameManager.Instance.WorldPrefab;

        private readonly WorldRenderer _worldRenderer;
        private readonly Chunk[,] _chunks;
        private readonly Queue<Chunk> _chunksToUpdate;
        public Vector3 Center { get; }

        public World() : this(Vector3.zero) { }
        public World(Vector3 center)
        {
            Center = center;
            MiddleCoord = Mathf.FloorToInt((float)ChunkConfig.chunkCoordSize / 2);
            _chunks = new Chunk[ChunkConfig.chunkCoordSize, ChunkConfig.chunkCoordSize];
            _chunksToUpdate = new Queue<Chunk>();
            _worldRenderer = Object.Instantiate(WorldPrefab);
            _worldRenderer.World = this;
        }

        public async Task RePopulateChunkFromCoord(ChunkCoord chunkCoord)
        {
            var x = chunkCoord.x;
            var z = chunkCoord.z;
            _chunks[x, z] ??= new Chunk(chunkCoord);
            await _chunks[x, z].CreateChunk();
        }

        public async Task GenerateWorld()
        {
            await RePopulateChunkFromCoord(new ChunkCoord(MiddleCoord, MiddleCoord));
            PopulateBottomLeftWorld();
            PopulateTopLeftWorld();
            PopulateBottomRightWorld();
            PopulateTopRightWorld();
            UpdateChunks();
        }

        public static bool IsPositionOnWorld(float x, float z)
        {
            var xChunk = Mathf.FloorToInt(x / ChunkConfig.chunkWidth);
            var zChunk = Mathf.FloorToInt(z / ChunkConfig.chunkWidth);

            return xChunk >= 0 &&
                   xChunk <= ChunkConfig.chunkCoordSize - 1 &&
                   zChunk >= 0 &&
                   zChunk <= ChunkConfig.chunkCoordSize - 1;
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

            ref var voxel = ref chunk.GetVoxelFromPosition(x, y, z);
            return voxel.GetVoxelConfig().isSolid;
        }

        public VoxelType GenerateVoxelType(Vector3 pos)
        {
            //默认行为
            var y = Mathf.FloorToInt(pos.y);
            if (y == 0) return VoxelType.BedRock;

            var noisePos = new Vector2(pos.x, pos.z);

            var biome = Biomes[0];
            var strongestWeight = 0f;
            var sumHeight = 0f;
            var count = 0;

            foreach (var bo in Biomes)
            {
                var weight = NoiseGenerator.Get2DPerlinNoise(noisePos, bo.offset, bo.scale, ChunkConfig.chunkWidth);
                if (weight > strongestWeight)
                {
                    strongestWeight = weight;
                    biome = bo;
                }
                var height = NoiseGenerator.Get2DPerlinNoise(noisePos, 0, bo.terrainScale, ChunkConfig.chunkWidth) * weight * bo.terrainHeight;
                if (height <= 0) continue;
                sumHeight += height;
                count++;
            }
            // 基础地形
            var terrainHeight = Mathf.FloorToInt(sumHeight / count) + biome.solidGroundHeight;

            if (y > terrainHeight)
            {
                return VoxelType.Air;
            }

            var voxelType = VoxelType.Air;

            if (y == terrainHeight)
            {
                voxelType = biome.surfaceVoxelType;
                //随机生成树木
                if (biome.hasTree)
                {
                    GenerateTree(pos, biome);
                }
            }
            if (y < terrainHeight)
            {
                voxelType = y > terrainHeight - 5 ? biome.subsurfaceVoxelType : VoxelType.Stone;
            }

            //生物多样性地形

            if (voxelType == VoxelType.Stone)
            {
                foreach (var lode in biome.lodes)
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

        public ref Chunk GetChunkFromPosition(float x, float z)
        {
            var xChunk = Mathf.FloorToInt(x / ChunkConfig.chunkWidth);
            var zChunk = Mathf.FloorToInt(z / ChunkConfig.chunkWidth);
            _chunks[xChunk, zChunk] ??= new Chunk(new ChunkCoord(xChunk, zChunk));
            return ref _chunks[xChunk, zChunk];
        }

        public async void UpdateChunks()
        {
            while (true)
            {
                if (_chunksToUpdate.Count > 0)
                {
                    var chunk = _chunksToUpdate.Dequeue();
                    await RePopulateChunkFromCoord(chunk.ChunkCoord);
                }
                await Task.Delay(100);
            }

        }

        private async void PopulateBottomLeftWorld()
        {
            for (var z = MiddleCoord; z >= 0; z--)
            {
                for (var x = MiddleCoord - 1; x >= 0; x--)
                {
                    await RePopulateChunkFromCoord(new ChunkCoord(x, z));
                }
            }
        }

        private async void PopulateTopLeftWorld()
        {
            for (var z = MiddleCoord + 1; z < ChunkConfig.chunkCoordSize; z++)
            {
                for (var x = MiddleCoord; x >= 0; x--)
                {
                    await RePopulateChunkFromCoord(new ChunkCoord(x, z));
                }
            }
        }
        private async void PopulateTopRightWorld()
        {
            for (var z = MiddleCoord; z < ChunkConfig.chunkCoordSize; z++)
            {
                for (var x = MiddleCoord + 1; x < ChunkConfig.chunkCoordSize; x++)
                {
                    await RePopulateChunkFromCoord(new ChunkCoord(x, z));
                }
            }
        }
        private async void PopulateBottomRightWorld()
        {
            for (var z = MiddleCoord - 1; z >= 0; z--)
            {
                for (var x = MiddleCoord; x < ChunkConfig.chunkCoordSize; x++)
                {
                    await RePopulateChunkFromCoord(new ChunkCoord(x, z));
                }
            }
        }

        private void GenerateTree(Vector3 pos, Biome biome)
        {
            var noisePos = new Vector2(pos.x, pos.z);
            var treeZoneNoise = NoiseGenerator.Get2DPerlinNoise(noisePos, 0f, biome.treeZoneScale,
                NoiseResolution);
            if (!(treeZoneNoise > biome.treeZoneThreshold)) return;

            var treeNoise = NoiseGenerator.Get2DPerlinNoise(noisePos, 0f, biome.treeScale, NoiseResolution);
            if (!(treeNoise > biome.treeThreshold)) return;

            var noiseHeight = biome.maxHeight *
                              NoiseGenerator.Get2DPerlinNoise(noisePos, 250f, 3f, NoiseResolution);
            var height = Mathf.FloorToInt(noiseHeight + biome.minHeight);
            for (var h = 0; h <= height; h++)
            {
                if (h + pos.y >= ChunkConfig.chunkHeight) break;

                var chunk = GetChunkFromPosition(pos.x, pos.z);
                ref var voxel = ref chunk.GetVoxelFromPosition(pos.x, h + pos.y, pos.z);
                voxel.VoxelType = VoxelType.Stump;
            }

            var leafHeight = pos.y + height;
            var chunksToUpdate = new HashSet<Chunk>();
            for (var y = 0; y < 3; y++)
            {
                for (var x = pos.x + y - 2; x <= pos.x - y + 2; x++)
                {
                    for (var z = pos.z + y - 2; z <= pos.z - y + 2; z++)
                    {
                        if (leafHeight + y >= ChunkConfig.chunkHeight) break;
                        if (!IsPositionOnWorld(x, z)) break;
                        ref var chunk = ref GetChunkFromPosition(x, z);
                        chunksToUpdate.Add(chunk);
                        ref var voxel = ref chunk.GetVoxelFromPosition(x, leafHeight + y, z);
                        if (Mathf.Abs(x - pos.x) < 0.001f && Mathf.Abs(z - pos.z) < 0.001f && y < 2)
                        {
                            voxel.VoxelType = VoxelType.Stump;
                        }
                        else
                        {
                            voxel.VoxelType = VoxelType.Leaf;
                        }
                    }
                }
            }
            foreach (var chunk in chunksToUpdate)
            {
                _chunksToUpdate.Enqueue(chunk);
            }
        }

        public override string ToString()
        {
            return $"World{Center}";
        }
    }
}
