using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using MilkSpun.CubeWorld.Models;
using MilkSpun.CubeWorld.Utils;
using UnityEngine;

namespace MilkSpun.CubeWorld
{
    public class Chunk
    {
        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;
        private MeshCollider _meshCollider;
        private GameObject _chunkObject;
        private ChunkCoord _chunkCoord;

        private List<Vector3> _vertices;
        private List<int> _triangles;
        private List<Vector2> _uv;
        private int _verticesIndex;


        public Vector3 Position => _chunkObject.transform.position;
        private static Transform World => GameManager.Instance.World;
        private static ChunkConfig ChunkConfig => GameManager.Instance.chunkConfig;

        public Chunk(in ChunkCoord chunkCoord)
        {
            _chunkCoord = chunkCoord;
            InitGameObject();
            CreateChunk();
        }

        private void CreateChunk()
        {
            _vertices = new List<Vector3>();
            _triangles = new List<int>();
            _uv = new List<Vector2>();
            this.LoopVoxel((x, y, z) =>
            {
                PopulateVoxel(new Vector3(x, y, z));
            });
            var mesh = new Mesh
            {
                name = _chunkCoord.ToString(),
                vertices = _vertices.ToArray(),
                triangles = _triangles.ToArray(),
                uv = _uv.ToArray()
            };
            _meshFilter.mesh = mesh;
            _ = ReGenerateMeshCollider(mesh);
        }

        private void PopulateVoxel(Vector3 voxelPos)
        {
            for (var p = 0; p < 6; p++)
            {
                if (IsPlaneInVisible(voxelPos + ChunkConfig.VoxelFaceOffset[p]))
                {
                    continue;
                }

                for (var i = 0; i < 4; i++)
                {
                    var vertIndex = ChunkConfig.VoxelTris[p, i];
                    _vertices.Add(ChunkConfig.VoxelVerts[vertIndex] + voxelPos);
                    _uv.Add(ChunkConfig.Uvs[i]);
                }

                _triangles.Add(_verticesIndex);
                _triangles.Add(_verticesIndex + 1);
                _triangles.Add(_verticesIndex + 2);
                _triangles.Add(_verticesIndex + 2);
                _triangles.Add(_verticesIndex + 1);
                _triangles.Add(_verticesIndex + 3);
                _verticesIndex += 4;
            }
        }

        private static bool IsPlaneInVisible(Vector3 voxelFacePos)
        {
            var x = Mathf.FloorToInt(voxelFacePos.x);
            var y = Mathf.FloorToInt(voxelFacePos.y);
            var z = Mathf.FloorToInt(voxelFacePos.z);
            var width = ChunkConfig.chunkWidth;
            var height = ChunkConfig.chunkHeight;

            return x >= 0 &&
                   x <= width - 1 &&
                   y >= 0 &&
                   y <= height - 1 &&
                   z >= 0 &&
                   z <= width - 1;
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
            _meshCollider = _chunkObject.AddComponent<MeshCollider>();
        }

        private async Task ReGenerateMeshCollider(Mesh mesh)
        {
            await Task.Yield();
            Physics.BakeMesh(mesh.GetInstanceID(), false);
            _meshCollider.sharedMesh = mesh;
        }

    }
}
