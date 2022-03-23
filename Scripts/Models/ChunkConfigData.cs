using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
namespace MilkSpun.CubeWorld.Models
{
    public static class ChunkConfigData
    {
        public static int Seed { get; private set; }
        public static int ChunkWidth { get; private set; }
        public static int ChunkHeight { get; private set; }
        public static int TextureAtlasSizeInBlocks { get; private set; }
        public static int ChunkCoordSize { get; private set; }
        
        /// <summary>
        /// 世界的宽度
        /// </summary>
        public static int WorldSizeInVoxels => ChunkCoordSize * ChunkWidth;
        
        /// <summary>
        /// 每个纹理的uv长度
        /// </summary>
        public static float NormalizedBlockTextureSize => 1f / TextureAtlasSizeInBlocks;
        
        /// <summary>
        /// 每个纹理图包含的纹理个数
        /// </summary>
        public static int TextureAtlasSize => TextureAtlasSizeInBlocks * TextureAtlasSizeInBlocks;
        
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

        public static void Init(ChunkConfig config)
        {
            Seed = config.seed;
            ChunkWidth = config.chunkWidth;
            ChunkHeight = config.chunkHeight;
            TextureAtlasSizeInBlocks = config.textureAtlasSizeInBlocks;
            ChunkCoordSize = config.chunkCoordSize;
        }
    }
}
