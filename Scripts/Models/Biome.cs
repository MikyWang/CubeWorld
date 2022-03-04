using System.Collections;
using System.Collections.Generic;
using MilkSpun.Common.Models;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MilkSpun.CubeWorld.Models
{
    [CreateAssetMenu(fileName = "Config", menuName = "MilkSpun/创建一个生物群落")]
    public class Biome : ScriptableObject
    {
        [TabGroup("生物群落")] [Tooltip("噪声的缩放比")] public float scale;
        [TabGroup("生物群落")] [Tooltip("生物群落类型")] public BiomeType biomeType;
        [TabGroup("生物群落")]
        [Tooltip("地表Voxel")]
        public VoxelType surfaceVoxelType;
        [TabGroup("生物群落")]
        [Tooltip("地表下Voxel")]
        public VoxelType subsurfaceVoxelType;
        [TabGroup("地形")] [Tooltip("地表Voxel")] public float terrainScale;
        [TabGroup("地形")] [Tooltip("offset")] public int offset;
        [TabGroup("地形")] [Tooltip("可见块平地高度")] public int solidGroundHeight;
        [TabGroup("地形")] [Tooltip("地形基本高度")] public int terrainHeight;
        [TabGroup("地形")] [Tooltip("Waves")] public List<Wave> waves;
        [TabGroup("地心")] [InlineEditor] public Lode[] lodes;
        [TabGroup("树木")]
        [Tooltip("树木区域噪声的缩放比")]
        public bool hasTree;
        [TabGroup("树木")]
        [Tooltip("树木区域噪声的缩放比")]
        public float treeZoneScale = 1.3f;
        [TabGroup("树木")]
        [Tooltip("树木区域的阈值"), Range(0.1f, 1f)]
        public float treeZoneThreshold = 0.6f;
        [TabGroup("树木")]
        [Tooltip("树木区域噪声的缩放比")]
        public float treeScale = .8f;
        [TabGroup("树木")]
        [Tooltip("树木的阈值"), Range(0.1f, 1f)]
        public float treeThreshold = 0.8f;
        [TabGroup("树木")] [Tooltip("最大树木高度")] public int maxHeight = 12;
        [TabGroup("树木")] [Tooltip("最大树木高度")] public int minHeight = 5;

    }

    public enum BiomeType
    {
        Grassland,
        Desert,
        Snow
    }
}
