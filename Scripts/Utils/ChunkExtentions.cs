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

        /// <summary>
        /// 判断Voxel的面是否不可见
        /// </summary>
        /// <param name="voxel"><see cref="Voxel"/></param>
        /// <param name="voxelFaceType">Voxel的朝向 <see cref="VoxelFaceType"/></param>
        /// <returns></returns>
        public static bool IsPlaneInVisible(this Voxel voxel, VoxelFaceType voxelFaceType)
        {
            var voxelConfig = voxel.GetVoxelConfig();
            if (!voxelConfig.isSolid) return true;
            
            var xCoordPos = voxel.X + voxel.ChunkCoord.x * ChunkConfig.chunkWidth;
            var zCoordPos = voxel.Z + voxel.ChunkCoord.z * ChunkConfig.chunkWidth;

            var voxelFacePos = new Vector3(xCoordPos, voxel.Y, zCoordPos) +
                               ChunkConfig.VoxelFaceOffset[(int)
                                   voxelFaceType];

            var x = Mathf.FloorToInt(voxelFacePos.x);
            var y = Mathf.FloorToInt(voxelFacePos.y);
            var z = Mathf.FloorToInt(voxelFacePos.z);

            var width = ChunkConfig.WorldSizeInVoxels;
            var height = ChunkConfig.chunkHeight;

            return x >= 0 &&
                   x <= width - 1 &&
                   y >= 0 &&
                   y <= height - 1 &&
                   z >= 0 &&
                   z <= width - 1;
        }

        /// <summary>
        /// 获取Voxel的配置
        /// </summary>
        /// <param name="voxel">当前<see cref="Voxel"/></param>
        /// <returns>Voxel的配置</returns>
        public static VoxelConfig GetVoxelConfig(this Voxel voxel)
        {
            var voxelConfig = VoxelConfigs.Find(vt => vt.voxelType == voxel.VoxelType);
            return voxelConfig;
        }

        /// <summary>
        /// 获取Voxel纹理的ID
        /// </summary>
        /// <param name="voxel">当前Voxel</param>
        /// <param name="voxelFaceType">Voxel的朝向</param>
        /// <param name="textureAtlasSize">贴图中所有纹理的个数 如:4x4</param>
        /// <returns>纹理ID</returns>
        public static int GetTextureID(
            this Voxel voxel,
            VoxelFaceType voxelFaceType,
            int textureAtlasSize)
        {
            var voxelConfig = voxel.GetVoxelConfig();

            return voxelFaceType switch
            {
                VoxelFaceType.Back => voxelConfig.backFaceTexture +
                                      voxelConfig.page * textureAtlasSize,
                VoxelFaceType.Front => voxelConfig.frontFaceTexture +
                                       voxelConfig.page * textureAtlasSize,
                VoxelFaceType.Top => voxelConfig.topFaceTexture +
                                     voxelConfig.page * textureAtlasSize,
                VoxelFaceType.Bottom => voxelConfig.bottomFaceTexture +
                                        voxelConfig.page * textureAtlasSize,
                VoxelFaceType.Left => voxelConfig.leftFaceTexture +
                                      voxelConfig.page * textureAtlasSize,
                VoxelFaceType.Right => voxelConfig.rightFaceTexture +
                                       voxelConfig.page * textureAtlasSize,
                _ => 0
            };
        }
    }
}
