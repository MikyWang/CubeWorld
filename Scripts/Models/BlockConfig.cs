using System.Collections.Generic;
using UnityEngine;
namespace MilkSpun.CubeWorld.Models
{
    [CreateAssetMenu(fileName = "", menuName = "MilkSpun/创建BlockConfig")]
    public class BlockConfig : ScriptableObject
    {
        public List<FaceConfig> faceConfigs = new(6);
    }

    [System.Serializable]
    public class FaceConfig
    {
        [Tooltip("朝向")] public VoxelFaceType voxelFaceType;
        [Tooltip("顶点")] public List<Vector3> vertices = new(4);
        [Tooltip("顶点uv")] public List<Vector2> uv = new(4);
        [Tooltip("三角形顶点索引")] public List<int> triangles = new(6);
    }
}
