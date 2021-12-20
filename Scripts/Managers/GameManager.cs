using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MilkSpun.Common;
using MilkSpun.CubeWorld.Models;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace MilkSpun.CubeWorld
{
    public class GameManager : Singleton<GameManager>
    {
        [Tooltip("起始玩家的Prefab")]
        [PreviewField(50f, ObjectFieldAlignment.Right)]
        [SerializeField, Space]
        private GameObject originalPlayerPrefab;
        [Tooltip("地形材质")]
        [PreviewField(50f, ObjectFieldAlignment.Right)]
        [SerializeField, Space]
        private Material chunkMaterial;
        [InlineEditor, Space] public ChunkConfig chunkConfig;
        [InlineEditor, Space] public List<VoxelConfig> voxelConfigs;
        public World World { get; private set; }
        public Material ChunkMaterial => chunkMaterial;
        public GameObject OriginalPlayerPrefabPrefab => originalPlayerPrefab;

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
            World = World is null ? new World() : World.GenerateWorld();
            // var player = Instantiate(originalPlayerPrefab);
        }



    }
}
