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
        [Tooltip("CineMachine的预制件")]
        [SerializeField, Space, Required]
        private GameObject cmPrefab;
        [Tooltip("起始玩家的预制件")]
        [PreviewField(50f, ObjectFieldAlignment.Right)]
        [SerializeField, Space, Required]
        private GameObject originalPlayerPrefab;
        [Tooltip("地形材质")]
        [PreviewField(50f, ObjectFieldAlignment.Right)]
        [SerializeField, Space, Required]
        private Material chunkMaterial;
        [InlineEditor, Space] public ChunkConfig chunkConfig;
        [InlineEditor, Space] public List<VoxelConfig> voxelConfigs;
        public World World { get; private set; }
        public CinemachineFreeLook CmFreeLook { get; private set; }
        public Material ChunkMaterial => chunkMaterial;
        public GameObject OriginalPlayerPrefabPrefab => originalPlayerPrefab;

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(this);
        }
        private async void Start()
        {
            Random.InitState(chunkConfig.seed);
            Locator.World = World = World is null ? await new World().GenerateWorld() : await World.GenerateWorld();
            SetCamera(GeneratePlayer());
        }

        private GameObject GeneratePlayer()
        {
            var middle = World.MiddleCoord * chunkConfig.chunkWidth +
                         Mathf.FloorToInt((float)chunkConfig.chunkWidth / 2);
            var pos = new Vector3(middle, chunkConfig.chunkHeight+10f, middle);
            var player = Instantiate(originalPlayerPrefab);
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
