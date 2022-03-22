using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MilkSpun.CubeWorld.Models
{
    [System.Serializable]
    public class ChunkMeshData
    {
        [Tooltip("存放地块的顶点")] public List<Vector3> vertices = new(256);
        [Tooltip("存放地块三角形顶点索引")] public List<int> triangles = new(256);
        [Tooltip("存放法线")] public List<Vector3> normals = new(256);
        [Tooltip("存放地块的UV")] public List<Vector2> uv = new(256);
        [Tooltip("存放Texture2D Array的索引.")] public List<Vector2> uv2 = new(256);

        public void Clear()
        {
            vertices.Clear();
            normals.Clear();
            triangles.Clear();
            uv.Clear();
            uv2.Clear();
        }
    }
}
