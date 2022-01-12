using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cinemachine;
using MilkSpun.Common;
using MilkSpun.Common.MilkSpun.Scripts.Common;
using MilkSpun.CubeWorld.Models;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace MilkSpun.CubeWorld.Managers
{
    public class GameManager : Singleton<GameManager>
    {
        [HorizontalGroup("game")]
        [TabGroup("game/player", "主角相关")]
        [Tooltip("CineMachine的预制件")]
        [SerializeField, Space, Required]
        private GameObject cmPrefab;
        [Tooltip("起始玩家的预制件")]
        [PreviewField(50f, ObjectFieldAlignment.Right)]
        [SerializeField, Space, Required]
        [TabGroup("game/player", "主角相关")]
        private GameObject originalPlayerPrefab;
        [TabGroup("game/player", "地形相关")]
        [Tooltip("世界预制体"), Title("预制体")]
        [SerializeField, Space, Required]
        private WorldRenderer worldPrefab;
        [TabGroup("game/player", "地形相关")]
        [Tooltip("地块预制体")]
        [SerializeField, Space, Required]
        private ChunkRenderer chunkPrefab;
        [TabGroup("game/player", "地形相关")]
        [InlineEditor, Space, Title("地形配置")]
        public ChunkConfig chunkConfig;
        [TabGroup("game/player", "地形相关")]
        [InlineEditor, SerializeField, Space]
        private Biome biome;
        [TabGroup("game/player", "地形相关")]
        [InlineEditor, Space]
        public List<VoxelConfig> voxelConfigs;
        public World World { get; private set; }
        public CinemachineFreeLook CmFreeLook { get; private set; }
        public ChunkRenderer ChunkPrefab => chunkPrefab;
        public WorldRenderer WorldPrefab => worldPrefab;
        public Biome Biome => biome;

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(this);
        }
        private async void Start()
        {
            Random.InitState(chunkConfig.seed);
            Locator.World = World ??= new World();
            await World.GenerateWorld();
            SetCamera(GeneratePlayer());
        }

        private GameObject GeneratePlayer()
        {
            var middle = World.MiddleCoord * chunkConfig.chunkWidth +
                         Mathf.FloorToInt((float)chunkConfig.chunkWidth / 2);
            var middleChunk = World.Chunks[World.MiddleCoord, World.MiddleCoord];
            var solidVoxel = middleChunk.GetTopSolidVoxelFromPosition(middle, middle);
            var pos = new Vector3(middle, solidVoxel.Y + 1, middle);
            var player = Instantiate(originalPlayerPrefab, pos, Quaternion.identity);
            player.transform.position = pos;
            return player;
        }
        private void SetCamera(GameObject focusObject)
        {
            CmFreeLook ??= Instantiate(cmPrefab).GetComponent<CinemachineFreeLook>();
            CmFreeLook.LookAt = focusObject.transform;
            CmFreeLook.Follow = focusObject.transform;
            CmFreeLook.m_YAxis.Value = 0.5f;
        }


    }
}
