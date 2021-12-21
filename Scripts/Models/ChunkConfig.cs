using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MilkSpun.CubeWorld.Models
{
    [CreateAssetMenu(fileName = "", menuName = "MilkSpun/创建Chunk配置")]
    public class ChunkConfig : ScriptableObject
    {
        [Tooltip("一个Chunk的宽度,默认16x16")]
        public int chunkWidth = 16;
        [Tooltip("一个Chunk的高度,默认128")]
        public int chunkHeight = 128;
        [Tooltip("整个纹理各有多少行,如:4x4个小纹理")]
        public int textureAtlasSizeInBlocks = 4;
        [Tooltip("世界上拥有多少个Chunk,默认5x5")]
        public int chunkCoordSize = 5;

        /// <summary>
        /// 世界的宽度
        /// </summary>
        public  int WorldSizeInVoxels => chunkCoordSize * chunkWidth;
        /// <summary>
        /// 每个纹理的uv长度
        /// </summary>
        public float NormalizedBlockTextureSize => 1f / textureAtlasSizeInBlocks;
        /// <summary>
        /// 每个纹理图包含的纹理个数
        /// </summary>
        public int TextureAtlasSize => textureAtlasSizeInBlocks * textureAtlasSizeInBlocks;
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
            { 5, 6, 4, 7 }, //前面
            { 3, 7, 2, 6 }, //上面
            { 1, 5, 0, 4 }, //下面
            { 4, 7, 0, 3 }, //左面
            { 1, 2, 5, 6 }  //右面
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
