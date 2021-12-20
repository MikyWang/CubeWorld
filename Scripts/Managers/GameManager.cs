using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MilkSpun.Common;
using MilkSpun.CubeWorld.Models;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MilkSpun.CubeWorld
{
    public class GameManager : Singleton<GameManager>
    {
        [Tooltip("地形材质")]
        [PreviewField(50f, ObjectFieldAlignment.Right)]
        [SerializeField, Space]
        private Material chunkMaterial;
        [InlineEditor, Space] public ChunkConfig chunkConfig;
        [InlineEditor, Space] public List<VoxelConfig> voxelConfigs;
        public Transform World { get; set; }
        public Material ChunkMaterial => chunkMaterial;

        private Chunk[,] _chunks;
        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(this);
        }

        [OnInspectorInit]
        private void Init()
        {
            base.Awake();
        }

        [Button("创建世界")]
        private void Start()
        {
            if (World)
                DestroyImmediate(World.gameObject);
            GenerateWorld();
        }

        private void GenerateWorld()
        {
            World = new GameObject("World").transform;
            _chunks = new Chunk[chunkConfig.chunkCoordSize, chunkConfig.chunkCoordSize];
            _ = GenerateBottomLeftWorld();
            _ = GenerateBottomRightWorld();
            _ = GenerateTopLeftWorld();
            _ = GenerateTopRightWorld();
        }

        private async Task GenerateBottomLeftWorld()
        {
            for (var z = chunkConfig.chunkCoordSize / 2 - 1; z > 0; z--)
            {
                for (var x = chunkConfig.chunkCoordSize / 2 - 1; x > 0; x--)
                {
                    await Task.Yield();
                    _chunks[x, z] = new Chunk(new ChunkCoord(x, z));
                }
            }
        }

        private async Task GenerateTopLeftWorld()
        {
            for (var z = chunkConfig.chunkCoordSize / 2; z < chunkConfig.chunkCoordSize; z++)
            {
                for (var x = chunkConfig.chunkCoordSize / 2 - 1; x > 0; x--)
                {
                    await Task.Yield();
                    _chunks[x, z] = new Chunk(new ChunkCoord(x, z));
                }
            }
        }
        private async Task GenerateTopRightWorld()
        {
            for (var z = chunkConfig.chunkCoordSize / 2; z < chunkConfig.chunkCoordSize; z++)
            {
                for (var x = chunkConfig.chunkCoordSize / 2; x < chunkConfig.chunkCoordSize; x++)
                {
                    await Task.Yield();
                    _chunks[x, z] = new Chunk(new ChunkCoord(x, z));
                }
            }
        }
        private async Task GenerateBottomRightWorld()
        {
            for (var z = chunkConfig.chunkCoordSize / 2 - 1; z > 0; z--)
            {
                for (var x = chunkConfig.chunkCoordSize / 2; x < chunkConfig.chunkCoordSize; x++)
                {
                    await Task.Yield();
                    _chunks[x, z] = new Chunk(new ChunkCoord(x, z));
                }
            }
        }

    }
}
