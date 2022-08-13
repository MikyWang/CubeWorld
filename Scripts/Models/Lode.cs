using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MilkSpun.Common;
using UnityEngine;

namespace MilkSpun.CubeWorld.Models
{
    [CreateAssetMenu(fileName = "Config", menuName = "MilkSpun/创建一个Lode")]
    public class Lode : ScriptableObject
    {
        [Tooltip("Block类型")] public VoxelType voxelType;
        [Tooltip("有效的最低高度")] public int minHeight;
        [Tooltip("有效的最高高度")] public int maxHeight;
        [Tooltip("用于3D噪声的缩放比")] public float scale;
        [Tooltip("用于3D噪声的阈值")] public float threshold;
        [Tooltip("用于3D噪声的偏移量")] public float offset;

    }
}
