#if UNITY_EDITOR
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
    public partial class GameManager
    {
        [Button("编辑器模式初始化")]
        private void Init()
        {
            Instance = this;
            World = new World();
        } 
    }
}

#endif
