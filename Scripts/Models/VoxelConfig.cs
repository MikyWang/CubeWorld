using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace MilkSpun.CubeWorld.Models
{
    [CreateAssetMenu(fileName = "", menuName = "MilkSpun/创建VoxelConfig")]
    public partial class VoxelConfig : ScriptableObject
    {
        [FormerlySerializedAs("voxelMapType")] [Title("基本设置")] [Tooltip("方块类型")] public VoxelType voxelType;
        [Tooltip("是否为固体.")] public bool isSolid;
        [Title("纹理ID")]
        [Tooltip("使用第几页纹理")]
        [Range(0, 8)]
        public int page;
        [Tooltip("后面纹理"), OnValueChanged("ShowTexture")]
        public int backFaceTexture;
        [Tooltip("前面纹理"), OnValueChanged("ShowTexture")]
        public int frontFaceTexture;
        [Tooltip("顶部纹理"), OnValueChanged("ShowTexture")]
        public int topFaceTexture;
        [Tooltip("底部纹理"), OnValueChanged("ShowTexture")]
        public int bottomFaceTexture;
        [Tooltip("左面纹理"), OnValueChanged("ShowTexture")]
        public int leftFaceTexture;
        [Tooltip("右面纹理"), OnValueChanged("ShowTexture")]
        public int rightFaceTexture;
    }

    public enum VoxelFaceType : byte
    {
        Back,
        Front,
        Top,
        Bottom,
        Left,
        Right
    }

    public enum VoxelType : byte
    {
        Air,
        Dirt,
        Sand,
        Stone,
        Ice,
        Grass
    }

}
