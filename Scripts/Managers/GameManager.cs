using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cinemachine;
using MilkSpun.Common;
using MilkSpun.CubeWorld.Models;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace MilkSpun.CubeWorld.Managers
{
    public class GameManager : Singleton<GameManager>
    {
        [Tooltip("起始玩家的预制件")]
        [PreviewField(50f, ObjectFieldAlignment.Right)]
        [SerializeField, Space, Required]
        private GameObject originalPlayerPrefab;
        [Tooltip("CineMachine的预制件")]
        [SerializeField, Space, Required]
        private GameObject cmPrefab;
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

        [OnInspectorInit]
        private void Init()
        {
            base.Awake();
        }

        [Button("创建世界")]
        private void Start()
        {
            World = World is null ? new World() : World.GenerateWorld();
            SetCamera();
        }

        private void SetCamera()
        {
            CmFreeLook ??= Instantiate(cmPrefab).GetComponent<CinemachineFreeLook>();
            var middle = chunkConfig.WorldSizeInVoxels / 2;
            var pos = new Vector3(middle, chunkConfig.chunkHeight, middle);
            var player = Instantiate(originalPlayerPrefab);
            player.transform.position = pos;
            CmFreeLook.LookAt = player.transform;
            CmFreeLook.Follow = player.transform;
        }


    }
}
