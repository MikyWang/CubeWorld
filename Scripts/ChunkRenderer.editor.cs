#if UNITY_EDITOR
using System.Collections.Generic;
using MilkSpun.CubeWorld.Models;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using System.Linq;
using Tree = MilkSpun.CubeWorld.Models.Tree;

namespace MilkSpun.CubeWorld
{
    public partial class ChunkRenderer
    {
        public BlockConfig blockConfig;

        [Button("测试")]

        public void Test()
        {
          
        }

    }
}

#endif
