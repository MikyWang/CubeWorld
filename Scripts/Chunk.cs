using System.Collections;
using System.Collections.Generic;
using MilkSpun.CubeWorld.Models;
using MilkSpun.CubeWorld.Utils;
using UnityEngine;

namespace MilkSpun.CubeWorld
{
    public class Chunk
    {
        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;
        private GameObject _chunkObject;
        private ChunkCoord _chunkCoord;
        
        private List<Vector3> _vertices;
        private List<int> _triangles;
        private int _verticesIndex;

        public Vector3 Position => _chunkObject.transform.position;
        private static Transform World => GameManager.Instance.World;

        public Chunk(in ChunkCoord chunkCoord)
        {
            _chunkCoord = chunkCoord;
            InitGameObject();
            CreateMesh();
        }

        private void CreateMesh()
        {
            _vertices = new List<Vector3>();
            _triangles = new List<int>();

            for (var p = 0; p < 6; p++)
            {
                for (var i = 0; i < 4; i++)
                {
                    var vertIndex = VoxelData.VoxelTris[p, i];
                    _vertices.Add(VoxelData.VoxelVerts[vertIndex]);
                }
                
                _triangles.Add(_verticesIndex);
                _triangles.Add(_verticesIndex + 1);
                _triangles.Add(_verticesIndex + 2);
                _triangles.Add(_verticesIndex + 2);
                _triangles.Add(_verticesIndex + 1);
                _triangles.Add(_verticesIndex + 3);
                _verticesIndex+=4;
            }

            var mesh = new Mesh
            {
                name = _chunkCoord.ToString(),
                vertices = _vertices.ToArray(),
                triangles = _triangles.ToArray()
            };
            _meshFilter.mesh = mesh;
        }
        
        private void InitGameObject()
        {
            _chunkObject = new GameObject(_chunkCoord.ToString());
            _chunkObject.transform.SetParent(World);
            _chunkObject.transform.localPosition = new Vector3
            {
                x = _chunkCoord.x,
                y = 0f,
                z = _chunkCoord.z
            };
            _meshFilter = _chunkObject.AddComponent<MeshFilter>();
            _meshRenderer = _chunkObject.AddComponent<MeshRenderer>();
            _meshRenderer.material = GameManager.Instance.ChunkMaterial;
        }

    }
}
