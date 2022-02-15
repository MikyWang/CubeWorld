using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MilkSpun.CubeWorld.Models
{
    [CreateAssetMenu(fileName = "", menuName = "MilkSpun/创建Chunk配置")]
    public class ChunkConfig : ScriptableObject
    {
        [Tooltip("随机数种子")] public int seed;
        [Tooltip("一个Chunk的宽度,默认16x16")] public int chunkWidth = 16;
        [Tooltip("一个Chunk的高度,默认128")] public int chunkHeight = 128;
        [Tooltip("整个纹理各有多少行,如:4x4个小纹理")] public int textureAtlasSizeInBlocks = 4;
        [Tooltip("世界上拥有多少个Chunk,默认5x5")] public int chunkCoordSize = 5;
        [Tooltip("树木生长速度，单位毫秒")] public int treeGlowSpeed = 100;

        /// <summary>
        /// 世界的宽度
        /// </summary>
        public int WorldSizeInVoxels => chunkCoordSize * chunkWidth;

        /// <summary>
        /// 每个纹理的uv长度
        /// </summary>
        public float NormalizedBlockTextureSize => 1f / textureAtlasSizeInBlocks;

        /// <summary>
        /// 每个纹理图包含的纹理个数
        /// </summary>
        public int TextureAtlasSize => textureAtlasSizeInBlocks * textureAtlasSizeInBlocks;

        /// <summary>
        /// Voxel周围的面的相对位置
        /// </summary>
        public static readonly Dictionary<VoxelFaceType, Vector3> VoxelFaceOffset = new()
        {
            { VoxelFaceType.Back, Vector3.back },
            { VoxelFaceType.Front, Vector3.forward },
            { VoxelFaceType.Top, Vector3.up },
            { VoxelFaceType.Bottom, Vector3.down },
            { VoxelFaceType.Left, Vector3.left },
            { VoxelFaceType.Right, Vector3.right },
        };
    }
}
