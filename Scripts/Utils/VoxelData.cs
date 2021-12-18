using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MilkSpun.CubeWorld.Utils
{
    public static class VoxelData
    {
        /// <summary>
        /// 每个Cube的顶点数据
        /// </summary>
        public static readonly Vector3[] VoxelVerts =
        {
            new(0f, 0f, 0f),
            new(1f, 0f, 0f),
            new(1f, 1f, 0f),
            new(0f, 1f, 0f),
            new(0f, 0f, 1f),
            new(1f, 0f, 1f),
            new(1f, 1f, 1f),
            new(0f, 1f, 1f)
        };

        /// <summary>
        /// Cube每个面的三角形的顶点索引
        /// </summary>
        public static readonly int[,] VoxelTris =
        {
            { 0, 3, 1, 2 }, //后面
            { 4, 5, 7, 6 }, //前面
            { 3, 7, 2, 6 }, //上面
            { 0, 1, 4, 5 }, //下面
            { 4, 7, 0, 3 }, //左面
            { 1, 2, 5, 6 }  //右面
        };
    }
}
