using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MilkSpun.CubeWorld.Models
{
    [System.Serializable]
    public class ChunkMeshData
    {
        [Tooltip("存放地块的顶点")] public List<Vector3> vertices = new();
        [Tooltip("存放地块三角形顶点索引")] public List<int> triangles = new();
        [Tooltip("存放法线")] public List<Vector3> normals = new();
        [Tooltip("存放地块的UV")] public List<Vector2> uv = new();
        [Tooltip("存放Texture2D Array的索引.")] public List<Vector2> uv2 = new();

        public void Clear()
        {
            vertices.Clear();
            vertices.TrimExcess();
            normals.Clear();
            normals.TrimExcess();
            triangles.Clear();
            triangles.TrimExcess();
            uv.Clear();
            uv.TrimExcess();
            uv2.Clear();
            uv2.TrimExcess();
        }
    }
}
