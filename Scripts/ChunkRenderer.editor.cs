#if UNITY_EDITOR
using System.Collections.Generic;
using MilkSpun.CubeWorld.Models;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace MilkSpun.CubeWorld
{
    public partial class ChunkRenderer
    {
        public BlockConfig blockConfig;

        [Button("生成Mesh")]
        public void Populate()
        {
            var vertices = new List<Vector3>();
            var triangles = new List<int>();
            var uv = new List<Vector2>();
            var index = 0;

            foreach (var faceConfig in blockConfig.faceConfigs)
            {
                vertices.AddRange(faceConfig.vertices);
                uv.AddRange(faceConfig.uv);
                for (int i = 0; i < uv.Count; i++)
                {
                    uv[i] = uv[i] + new Vector2(0,2);
                }
                foreach (var triangle in faceConfig.triangles)
                {
                    triangles.Add(index + triangle);
                }
                index += faceConfig.vertices.Count;
            }

            var mesh = new Mesh
            {
                vertices = vertices.ToArray(),
                triangles = triangles.ToArray(),
                uv = uv.ToArray()
            };

            mesh.RecalculateNormals();
            
            var file = "Assets/MilkSpun/MyCube.asset";
            
            AssetDatabase.DeleteAsset(file);
            AssetDatabase.CreateAsset(mesh, file);
            AssetDatabase.ImportAsset(file, ImportAssetOptions.ForceUpdate);

            _meshFilter.mesh = mesh;

        }

    }
}

#endif
