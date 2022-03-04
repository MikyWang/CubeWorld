using System;
using System.Collections;
using System.Collections.Generic;
using MilkSpun.CubeWorld.Managers;
using MilkSpun.CubeWorld.Models;
using UnityEngine;
using UnityEngine.Assertions;
using Tree = MilkSpun.CubeWorld.Models.Tree;

namespace MilkSpun.CubeWorld.Utils
{
    public static class ChunkExtentions
    {
        private static World World => GameManager.Instance.World;
        private static ChunkConfig ChunkConfig => GameManager.Instance.chunkConfig;
        /// <summary>
        /// 生长树木，并判断是否需要再生长
        /// </summary>
        /// <param name="tree"><see cref="Tree"/></param>
        /// <returns>还需要生长返回true,否则false</returns>
        public static bool Glow(this Tree tree)
        {
            var pos = tree.position;
            var chunk = World.GetChunkFromPosition(pos.x, pos.z);
            ref var voxel = ref chunk.GetVoxelFromPosition(pos.x, pos.y, pos.z);
            if (voxel.voxelType != VoxelType.Stump)
            {
                return false; //不再是树根了，无需再生长。
            }
            var addHeight = 1;
            while (addHeight <= tree.height)
            {
                ref var glowVoxel = ref chunk.GetVoxelFromPosition(pos.x, pos.y + addHeight, pos.z);
                if (glowVoxel.voxelType == VoxelType.Air)
                {
                    glowVoxel.voxelType = VoxelType.Stump;
                    return true;
                }
                addHeight++;
            }

            if (addHeight <= tree.height) return false; //还未生长到最高，无需生成叶子

            var leafHeight = pos.y + tree.height;
            for (var y = 0; y < 3; y++)
            {
                for (var x = pos.x + y - 2; x <= pos.x - y + 2; x++)
                {
                    for (var z = pos.z + y - 2; z <= pos.z - y + 2; z++)
                    {
                        if (leafHeight + y >= ChunkConfig.chunkHeight) break;

                        if (!World.IsPositionOnWorld(x, z)) continue;
                        var leafChunk = World.GetChunkFromPosition(x, z);
                        ref var leafVoxel = ref leafChunk.GetVoxelFromPosition(x, leafHeight + y, z);
                        if (leafVoxel.voxelType != VoxelType.Air) continue;

                        if (Mathf.Abs(x - pos.x) < 0.001f && Mathf.Abs(z - pos.z) < 0.001f && y < 2)
                        {
                            leafVoxel.voxelType = VoxelType.Stump;
                        }
                        else
                        {
                            leafVoxel.voxelType = VoxelType.Leaf;
                        }
                        leafChunk.PrepareUpdate();
                    }
                }
            }

            return false; //生长完成，无需再生长了.
        }
    }
}
