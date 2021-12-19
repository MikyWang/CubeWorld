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

        public static int GetTextureID(
            this VoxelConfig voxelConfig,
            VoxelFaceType voxelFaceType,
            int textureAtlasSize)
        {
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
