using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using MilkSpun.CubeWorld.Managers;
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

        private readonly List<Vector3> _vertices;
        private readonly List<int> _triangles;
        private readonly List<Vector2> _uv;
        private readonly List<Vector2> _uv2; //存放Texture2D Array的索引.
        private int _verticesIndex;
        private readonly Voxel[,,] _voxels;

        public Vector3 Position => _chunkObject.transform.position;
        private static World World => GameManager.Instance.World;
        private static ChunkConfig ChunkConfig => GameManager.Instance.chunkConfig;

        public bool Active
        {
            set => _chunkObject.SetActive(value);
            get => _chunkObject.activeSelf;
        }

        public Chunk(in ChunkCoord chunkCoord)
        {
            _vertices = new List<Vector3>();
            _triangles = new List<int>();
            _uv = new List<Vector2>();
            _uv2 = new List<Vector2>();
            _voxels = new Voxel[ChunkConfig.chunkWidth, ChunkConfig.chunkHeight,
                ChunkConfig.chunkWidth];
            _chunkCoord = chunkCoord;
            InitGameObject();
            CreateChunk();
        }

        private void CreateChunk()
        {
            this.LoopVoxel((x, y, z) =>
            {
                _voxels[x, y, z] = new Voxel(_chunkCoord, x, y, z, VoxelType.Grass);
                PopulateVoxel(in _voxels[x, y, z]);
            });
            CreateMesh();
        }
        private void CreateMesh()
        {
            var mesh = new Mesh
            {
                name = _chunkCoord.ToString(),
                vertices = _vertices.ToArray(),
                triangles = _triangles.ToArray(),
                uv = _uv.ToArray(),
                uv2 = _uv2.ToArray()
            };
            _meshFilter.mesh = mesh;
            _ = ReGenerateMeshCollider(mesh);
            ClearData();
        }

        private void PopulateVoxel(in Voxel voxel)
        {
            for (var p = 0; p < 6; p++)
            {
                if (voxel.IsPlaneInVisible((VoxelFaceType)p))
                {
                    continue;
                }

                for (var i = 0; i < 4; i++)
                {
                    var vertIndex = ChunkConfig.VoxelTris[p, i];
                    _vertices.Add(ChunkConfig.VoxelVerts[vertIndex] + voxel.LocalPos);
                }

                //TODO 根据噪声图生成不同的Voxel并显示.
                var textureID = voxel.GetTextureID((VoxelFaceType)p, ChunkConfig.TextureAtlasSize);
                AddTexture(textureID);

                _triangles.Add(_verticesIndex);
                _triangles.Add(_verticesIndex + 1);
                _triangles.Add(_verticesIndex + 2);
                _triangles.Add(_verticesIndex + 2);
                _triangles.Add(_verticesIndex + 1);
                _triangles.Add(_verticesIndex + 3);
                _verticesIndex += 4;
            }
        }

        private void AddTexture(int textureID)
        {
            var textureAtlasSizeInBlocks = ChunkConfig.textureAtlasSizeInBlocks;
            var normalizedBlockTextureSize = ChunkConfig.NormalizedBlockTextureSize;
            var textureAtlasSize = ChunkConfig.TextureAtlasSize;

            var id = textureID % textureAtlasSize;

            var row = id / textureAtlasSizeInBlocks;
            var invertedRow = (textureAtlasSizeInBlocks - 1) - row;
            var col = id % textureAtlasSizeInBlocks;

            var x = col * normalizedBlockTextureSize;
            var y = invertedRow * normalizedBlockTextureSize;

            const float uvOffset = 0.001f;
            _uv.Add(new Vector2(x + uvOffset, y + uvOffset));
            _uv.Add(new Vector2(x + uvOffset, y + normalizedBlockTextureSize - uvOffset));
            _uv.Add(new Vector2(x + normalizedBlockTextureSize - uvOffset, y + uvOffset));
            _uv.Add(new Vector2(x + normalizedBlockTextureSize - uvOffset,
                y + normalizedBlockTextureSize - uvOffset));

            var index = textureID / textureAtlasSize;
            var v = index % 8;
            for (var i = 0; i < 4; i++)
            {
                _uv2.Add(new Vector2(v, v));
            }

        }

        private void InitGameObject()
        {
            _chunkObject = new GameObject(_chunkCoord.ToString());
            _chunkObject.transform.SetParent(World.Transform);
            _chunkObject.transform.localPosition = new Vector3
            {
                x = _chunkCoord.x * ChunkConfig.chunkWidth,
                y = 0f,
                z = _chunkCoord.z * ChunkConfig.chunkWidth
            };
            _meshFilter = _chunkObject.AddComponent<MeshFilter>();
            _meshRenderer = _chunkObject.AddComponent<MeshRenderer>();
            _meshRenderer.material = GameManager.Instance.ChunkMaterial;
            _meshCollider = _chunkObject.AddComponent<MeshCollider>();
        }

        private async void ClearData()
        {
            await Task.Run(() =>
            {
                _vertices.Clear();
                _vertices.TrimExcess();
                _triangles.Clear();
                _triangles.TrimExcess();
                _uv.Clear();
                _uv.TrimExcess();
                _uv2.Clear();
                _uv2.TrimExcess();
            });
        }

        private async Task ReGenerateMeshCollider(Mesh mesh)
        {
            await Task.Yield();
            Physics.BakeMesh(mesh.GetInstanceID(), false);
            _meshCollider.sharedMesh = mesh;
        }

    }
}
