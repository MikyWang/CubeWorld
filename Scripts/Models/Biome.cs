using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MilkSpun.CubeWorld.Models
{
    [CreateAssetMenu(fileName = "Config", menuName = "MilkSpun/创建一个生物群落")]
    public class Biome : ScriptableObject
    {
        [Tooltip("生物群落类型")] public BiomeType biomeType;
        [Tooltip("可见块平地高度")] public int solidGroundHeight;
        [Tooltip("地形基本高度")] public int terrainHeight;
        [Tooltip("噪声的缩放比")] public float scale;
        [Tooltip("可出现的Block"), InlineEditor] public Lode[] lodes;
    }

    public enum BiomeType
    {
        Default
    }
}
