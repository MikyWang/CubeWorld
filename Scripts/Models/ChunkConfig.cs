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
    }
}
