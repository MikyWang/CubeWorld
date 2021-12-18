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
        [SerializeField]
        private Material chunkMaterial;
        public Transform World { get; set; }
        public Material ChunkMaterial => chunkMaterial;
        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(this);
        }

        private void Start()
        {
            World = GameObject.FindWithTag("World").transform;
            var chunk = new Chunk(new ChunkCoord(0, 0));
        }
    }
}
