using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MilkSpun.Common;
using MilkSpun.CubeWorld.Managers;
using MilkSpun.CubeWorld.Models;
using MilkSpun.CubeWorld.Utils;
using UnityEngine;
using Object = UnityEngine.Object;
using Tree = MilkSpun.CubeWorld.Models.Tree;

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
        private readonly BlockingCollection<Tree> _trees;
        public Vector3 Center { get; }

        public World() : this(Vector3.zero) { }
        public World(Vector3 center)
        {
            Center = center;
            MiddleCoord = Mathf.FloorToInt((float)ChunkConfig.chunkCoordSize / 2);
            _chunks = new Chunk[ChunkConfig.chunkCoordSize, ChunkConfig.chunkCoordSize];
            _worldRenderer = Object.Instantiate(WorldPrefab);
            _worldRenderer.World = this;
            _trees = new BlockingCollection<Tree>(new ConcurrentQueue<Tree>());
        }

        /// <summary>
        /// 重新生成某一地块
        /// </summary>
        /// <param name="chunkCoord">地块坐标(x,z)</param>
        public void RePopulateChunkFromCoord(ChunkCoord chunkCoord)
        {
            var x = chunkCoord.x;
            var z = chunkCoord.z;

            if (x < 0 || x >= ChunkConfig.chunkCoordSize || z < 0 || z >= ChunkConfig.chunkCoordSize) return;

            _chunks[x, z] ??= new Chunk(chunkCoord);
            _chunks[x, z].CreateChunk();
        }

        /// <summary>
        /// 生成世界
        /// </summary>
        public void GenerateWorld()
        {
            RePopulateChunkFromCoord(new ChunkCoord(MiddleCoord, MiddleCoord));
            ConcurrentGenerateWorld();
            _ = Task.Factory.StartNew(GenerateTrees, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            _ = Task.Factory.StartNew(UpdateChunks, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        /// <summary>
        /// 判断坐标是否在世界上
        /// </summary>
        /// <param name="x">x</param>
        /// <param name="z">z</param>
        /// <returns>存在则返回true,否则false</returns>
        public static bool IsPositionOnWorld(float x, float z)
        {
            var xChunk = Mathf.FloorToInt(x / ChunkConfig.chunkWidth);
            var zChunk = Mathf.FloorToInt(z / ChunkConfig.chunkWidth);

            return xChunk >= 0 &&
                xChunk <= ChunkConfig.chunkCoordSize - 1 &&
                zChunk >= 0 &&
                zChunk <= ChunkConfig.chunkCoordSize - 1;
        }

        /// <summary>
        /// 判断是否能在该坐标生成物体
        /// </summary>
        /// <param name="pos">(x,y,z)</param>
        /// <param name="radius">半径</param>
        /// <returns><see cref="bool"/></returns>
        public static bool IsPositionCanBuild(Vector3 pos, int radius)
        {
            var x = Mathf.FloorToInt(pos.x);
            var z = Mathf.FloorToInt(pos.z);
            var width = ChunkConfig.WorldSizeInVoxels;

            return x >= radius && x <= width - radius && z >= radius && z <= width - radius;
        }

        /// <summary>
        /// 判断该世界坐标是否在地面上
        /// </summary>
        /// <param name="x">x</param>
        /// <param name="y">y</param>
        /// <param name="z">z</param>
        /// <returns>存在则返回true,否则false</returns>
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

        /// <summary>
        /// 重新生成该坐标的方块类型
        /// </summary>
        /// <param name="pos">(x,y,z)</param>
        /// <returns>方块类型</returns>
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
                var weight = NoiseGenerator.Get2DPerlinNoise(noisePos, ChunkConfig.seed, bo.scale, NoiseResolution);
                if (weight > strongestWeight)
                {
                    strongestWeight = weight;
                    biome = bo;
                }
                var height = NoiseGenerator.Get2DPerlinNoiseWithWaves(noisePos, bo.offset + ChunkConfig.seed, bo.terrainScale, bo.waves, NoiseResolution) * weight * bo.terrainHeight;
                if (height <= 0) continue;
                sumHeight += height;
                count++;
            }
            // 基础地形
            var terrainHeight = Mathf.FloorToInt(sumHeight / count) + biome.solidGroundHeight;

            if (y > terrainHeight) return VoxelType.Air;

            var voxelType = VoxelType.Air;

            if (y == terrainHeight)
            {
                voxelType = biome.surfaceVoxelType;
                //随机生成树木
                if (biome.hasTree && TreePass(pos, biome))
                {
                    var noiseHeight = biome.maxHeight * NoiseGenerator.Get2DPerlinNoise(noisePos, 250f + ChunkConfig.seed, 3f, NoiseResolution);
                    var height = Mathf.FloorToInt(noiseHeight + biome.minHeight);
                    var tree = new Tree(pos) { height = height };
                    _trees.Add(tree);
                    voxelType = VoxelType.Stump;
                }
            }
            if (y < terrainHeight) voxelType = y > terrainHeight - 5 ? biome.subsurfaceVoxelType : VoxelType.Stone;

            //生物多样性地形

            if (voxelType == VoxelType.Stone)
                foreach (var lode in biome.lodes)
                {
                    if (y < lode.minHeight || y > lode.maxHeight) continue;

                    if (NoiseGenerator.Get3DPerlinNoise(pos, lode.offset + ChunkConfig.seed, lode.scale, lode.threshold)) voxelType = lode.voxelType;
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

        private static bool TreePass(Vector3 pos, Biome biome)
        {
            if (!IsPositionCanBuild(pos, 2)) return false;

            var noisePos = new Vector2(pos.x, pos.z);
            var treeZoneNoise = NoiseGenerator.Get2DPerlinNoise(noisePos, ChunkConfig.seed, biome.treeZoneScale, NoiseResolution);
            if (!(treeZoneNoise > biome.treeZoneThreshold)) return false;

            var treeNoise = NoiseGenerator.Get2DPerlinNoise(noisePos, ChunkConfig.seed, biome.treeScale, NoiseResolution);
            return treeNoise > biome.treeThreshold;
        }
        public override string ToString()
        {
            return $"World{Center}";
        }

        private void GenerateTrees()
        {
            while (!_trees.IsCompleted)
            {
                foreach (var tree in _trees.GetConsumingEnumerable())
                {
                    if (tree.Glow())
                    {
                        _trees.Add(tree);
                    }
                }
            }
        }

        private void UpdateChunks()
        {
            while (true)
            {
                for (var x = 0; x < ChunkConfig.chunkCoordSize; x++)
                {
                    for (var z = 0; z < ChunkConfig.chunkCoordSize; z++)
                    {
                        _chunks[x, z]?.UpdateChunk();
                    }
                }
            }
            // ReSharper disable once FunctionNeverReturns
        }

        private Task ConcurrentGenerateWorld()
        {
            return Task.Run(() =>
            {
                for (var mc = 1; mc < MiddleCoord + 1; mc++)
                {
                    var mcSync = mc;
                    Parallel.For(0, mcSync + 1, i =>
                    {
                        if (i > 0 && i < mcSync)
                        {
                            RePopulateChunkFromCoord(new ChunkCoord(MiddleCoord - i, MiddleCoord + mcSync));
                            RePopulateChunkFromCoord(new ChunkCoord(MiddleCoord + mcSync, MiddleCoord + i));
                            RePopulateChunkFromCoord(new ChunkCoord(MiddleCoord - mcSync, MiddleCoord - i));
                            RePopulateChunkFromCoord(new ChunkCoord(MiddleCoord + i, MiddleCoord - mcSync));
                        }
                        RePopulateChunkFromCoord(new ChunkCoord(MiddleCoord + i, MiddleCoord + mcSync));
                        RePopulateChunkFromCoord(new ChunkCoord(MiddleCoord - mcSync, MiddleCoord + i));
                        RePopulateChunkFromCoord(new ChunkCoord(MiddleCoord + mcSync, MiddleCoord - i));
                        RePopulateChunkFromCoord(new ChunkCoord(MiddleCoord - i, MiddleCoord - mcSync));
                    });
                }
            });
        }
    }
}
