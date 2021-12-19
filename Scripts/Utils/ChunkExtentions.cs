using System;
using System.Collections;
using System.Collections.Generic;
using MilkSpun.CubeWorld.Models;
using UnityEngine;

namespace MilkSpun.CubeWorld.Utils
{
    public static class ChunkExtentions
    {
        /// <summary>
        /// 循环chunk的每个顶点
        /// </summary>
        /// <param name="chunk">Chunk</param>
        /// <param name="action">Position(x,y,z)</param>
        public static void LoopVoxel(this Chunk chunk, Action<int, int, int> action)
        {
            var chunkConfig = GameManager.Instance.chunkConfig;
            for (var y = 0; y < chunkConfig.chunkHeight; y++)
            {
                for (var z = 0; z < chunkConfig.chunkWidth; z++)
                {
                    for (var x = 0; x < chunkConfig.chunkWidth; x++)
                    {
                        action(x, y, z);
                    }
                }
            }
        }
    }
}
