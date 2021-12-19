using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MilkSpun.CubeWorld.Models
{
    [CreateAssetMenu(fileName = "", menuName = "MilkSpun/创建Chunk配置")]
    public class ChunkConfig : ScriptableObject
    {
        public int chunkWidth = 16;
        public int chunkHeight = 128;

        /// <summary>
        ///每个Cube的顶点数据
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
        public static readonly Vector2[] Uvs =
        {
            new(0f, 0f),
            new(1f, 0f),
            new(1f, 1f),
            new(0f, 1f)
        };

        /// <summary>
        /// Voxel周围的面的相对位置
        /// </summary>
        public static readonly Vector3[] VoxelFaceOffset =
        {
            new(0f, 0f, -1f), //后面
            new(0f, 0f, 1f),  //前面
            new(0f, 1f, 0f),  //上面
            new(0f, -1f, 0f), //下面
            new(-1f, 0f, 0f), //左面
            new(1f, 0f, 0f),  //右面
        };
    }
}
