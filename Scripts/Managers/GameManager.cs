using System;
using System.Collections;
using System.Collections.Generic;
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
            World = new GameObject("World").transform;
            var chunk = new Chunk(new ChunkCoord(0, 0));
        }
    }
}
