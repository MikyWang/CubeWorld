using System.Collections.Generic;
using MilkSpun.CubeWorld.Managers;
using MilkSpun.CubeWorld.Models;
using UnityEngine;

namespace MilkSpun.CubeWorld.Utils
{
    public static class VoxelExtentions
    {
        private static List<VoxelConfig> VoxelConfigs => GameManager.Instance.voxelConfigs;
        private static World World => GameManager.Instance.World;


        

    }
}
