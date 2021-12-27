using System.Collections.Generic;
using MilkSpun.CubeWorld.Managers;
using MilkSpun.CubeWorld.Models;
using UnityEngine;

namespace MilkSpun.CubeWorld.Utils
{
    public static class VoxelExtentions
    {
        private static ChunkConfig ChunkConfig => GameManager.Instance.chunkConfig;
        private static List<VoxelConfig> VoxelConfigs => GameManager.Instance.voxelConfigs;
        private static World World => GameManager.Instance.World;


        public static ref Chunk GetChunk(this Voxel voxel)
        {
            return ref World.Chunks[voxel.ChunkCoord.x, voxel.ChunkCoord.z];
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

        public static Vector3 GetWorldPosition(this Voxel voxel)
        {
            var chunkX = voxel.ChunkCoord.x * ChunkConfig.chunkWidth;
            var chunkZ = voxel.ChunkCoord.z * ChunkConfig.chunkWidth;
            var localPos = new Vector3(chunkX, 0, chunkZ) + voxel.LocalPos;
            return World.Transform.TransformPoint(localPos);
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
