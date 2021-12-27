using System;
using System.Collections;
using System.Collections.Generic;
using MilkSpun.CubeWorld.Managers;
using MilkSpun.CubeWorld.Models;
using UnityEngine;
using UnityEngine.Assertions;

namespace MilkSpun.CubeWorld.Utils
{
    public static class ChunkExtentions
    {
        private static ChunkConfig ChunkConfig => GameManager.Instance.chunkConfig;
        private static List<VoxelConfig> VoxelConfigs => GameManager.Instance.voxelConfigs;
        private static World World => GameManager.Instance.World;
        /// <summary>
        /// 循环chunk的每个顶点
        /// </summary>
        /// <param name="chunk">Chunk</param>
        /// <param name="action">Voxel的坐标</param>
        public static void LoopVoxel(this Chunk chunk, Action<int, int, int> action)
        {
            for (var y = 0; y < ChunkConfig.chunkHeight; y++)
            {
                for (var z = 0; z < ChunkConfig.chunkWidth; z++)
                {
                    for (var x = 0; x < ChunkConfig.chunkWidth; x++)
                    {
                        action(x, y, z);
                    }
                }
            }
        }

        
    }
}
