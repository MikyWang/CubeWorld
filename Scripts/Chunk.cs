using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using MilkSpun.Common;
using MilkSpun.CubeWorld.Managers;
using MilkSpun.CubeWorld.Models;
using MilkSpun.CubeWorld.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MilkSpun.CubeWorld
{
    [System.Serializable]
    public class Chunk
    {
        private ChunkCoord _chunkCoord;

        private readonly List<Vector3> _vertices;
        private readonly List<int> _triangles;
        private readonly List<Vector2> _uv;
        private readonly List<Vector2> _uv2; //存放Texture2D Array的索引.
        private int _verticesIndex;
        private readonly Voxel[,,] _voxels;
        private ChunkRenderer _chunkRenderer;

        public Vector3 Position { get; }
        public Vector3 LocalPosition { get; }
        private static ChunkConfig ChunkConfig => GameManager.Instance.chunkConfig;
        private static ChunkRenderer ChunkPrefab => GameManager.Instance.ChunkPrefab;
        private static World World => GameManager.Instance.World;

        public Chunk(ChunkCoord chunkCoord)
        {
            _vertices = new List<Vector3>();
            _triangles = new List<int>();
            _uv = new List<Vector2>();
            _uv2 = new List<Vector2>();
            _voxels = new Voxel[ChunkConfig.chunkWidth, ChunkConfig.chunkHeight,
                ChunkConfig.chunkWidth];
            _chunkCoord = chunkCoord;
            LocalPosition = new Vector3
            {
                x = _chunkCoord.x * ChunkConfig.chunkWidth,
                y = 0f,
                z = _chunkCoord.z * ChunkConfig.chunkWidth
            };
            Position = World.Center + LocalPosition;
        }

        public ref Voxel GetVoxelFromPosition(float x, float y, float z)
        {
            var yVoxel = Mathf.FloorToInt(y);
            var xVoxel = Mathf.FloorToInt(x - Position.x);
            var zVoxel = Mathf.FloorToInt(z - Position.z);
            ref var voxel = ref _voxels[xVoxel, yVoxel, zVoxel];
            if (!voxel.Initialize)
            {
                voxel = new Voxel(this, xVoxel, yVoxel, zVoxel);
                var pos = voxel.GetWorldPosition();
                voxel.VoxelType = World.GenerateVoxelType(pos);
            }
            return ref voxel;
        }

        public ref Voxel GetTopSolidVoxelFromPosition(float x, float z)
        {
            var xVoxel = Mathf.FloorToInt(x - Position.x);
            var zVoxel = Mathf.FloorToInt(z - Position.z);
            for (var y = ChunkConfig.chunkHeight - 1; y >= 0; y--)
            {
                ref var voxel = ref _voxels[xVoxel, y, zVoxel];
                if (!voxel.Initialize)
                {
                    voxel = new Voxel(this, xVoxel, y, zVoxel);
                    var pos = voxel.GetWorldPosition();
                    voxel.VoxelType = World.GenerateVoxelType(pos);
                }
                if (voxel.GetVoxelConfig().isSolid)
                {
                    return ref voxel;
                }
            }
            return ref _voxels[xVoxel, 0, zVoxel];
        }

        public bool IsPositionInChunk(float x, float y, float z)
        {
            var yVoxel = Mathf.FloorToInt(y);
            var xVoxel = Mathf.FloorToInt(x - Position.x);
            var zVoxel = Mathf.FloorToInt(z - Position.z);

            return xVoxel >= 0 &&
                   xVoxel <= ChunkConfig.chunkWidth - 1 &&
                   yVoxel >= 0 &&
                   yVoxel <= ChunkConfig.chunkHeight - 1 &&
                   zVoxel >= 0 &&
                   zVoxel <= ChunkConfig.chunkWidth - 1;
        }

        public async Task<Chunk> CreateChunk()
        {
            await Task.Run(() =>
            {
                this.LoopVoxel(PopulateVoxel);
            });
            _chunkRenderer ??= Object.Instantiate(ChunkPrefab, World.Transform);
            _chunkRenderer.Chunk = this;
            return this;
        }

        public Mesh ConvertToMesh()
        {
            return new Mesh
            {
                name = _chunkCoord.ToString(),
                vertices = _vertices.ToArray(),
                triangles = _triangles.ToArray(),
                uv = _uv.ToArray(),
                uv2 = _uv2.ToArray()
            };
        }

        private void PopulateVoxel(int x, int y, int z)
        {
            if (!_voxels[x, y, z].Initialize)
            {
                _voxels[x, y, z] = new Voxel(this, x, y, z);
                _voxels[x, y, z].VoxelType = World.GenerateVoxelType(_voxels[x, y, z].LocalPos + LocalPosition);
            }
            ref var voxel = ref _voxels[x, y, z];

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

        public async void ClearData()
        {
            await Task.Run(() =>
            {
                _verticesIndex = 0;
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

        public struct Voxel
        {
            public readonly int X;
            public readonly int Y;
            public readonly int Z;
            public VoxelType VoxelType;
            public bool Initialize { get; }
            public Chunk Chunk { get; }
            public Vector3 LocalPos => new(X, Y, Z);

            public Voxel(
                Chunk chunk,
                int x,
                int y,
                int z,
                VoxelType voxelType = VoxelType.Air)
            {
                X = x;
                Y = y;
                Z = z;
                VoxelType = voxelType;
                Chunk = chunk;
                Initialize = true;
            }

            /// <summary>
            /// 判断Voxel的面是否不可见
            /// </summary>
            /// <param name="voxelFaceType">Voxel的朝向 <see cref="VoxelFaceType"/></param>
            /// <returns></returns>
            public bool IsPlaneInVisible(VoxelFaceType voxelFaceType)
            {
                var voxelConfig = GetVoxelConfig();
                if (!voxelConfig.isSolid) return true;

                var xCoordPos = X + Chunk.LocalPosition.x;
                var zCoordPos = Z + Chunk.LocalPosition.z;

                var voxelFacePos = new Vector3(xCoordPos, Y, zCoordPos) +
                                   ChunkConfig.VoxelFaceOffset[(int)
                                       voxelFaceType];

                if (!World.CheckPositionOnGround(voxelFacePos.x, voxelFacePos.y, voxelFacePos.z))
                {
                    return false;
                }

                var x = Mathf.FloorToInt(voxelFacePos.x);
                var y = Mathf.FloorToInt(voxelFacePos.y);
                var z = Mathf.FloorToInt(voxelFacePos.z);
                var width = ChunkConfig.WorldSizeInVoxels;
                var height = ChunkConfig.chunkHeight;

                return x >= 0 &&
                       x <= width - 1 &&
                       y >= 0 &&
                       y <= height - 1 &&
                       z >= 0 &&
                       z <= width - 1;
            }

            public Vector3 GetWorldPosition()
            {
                var localPos = Chunk.LocalPosition + LocalPos;
                return World.Center + localPos;
            }

            /// <summary>
            /// 获取Voxel的配置
            /// </summary>
            /// <returns>Voxel的配置</returns>
            public VoxelConfig GetVoxelConfig()
            {
                var voxelType = VoxelType;
                var voxelConfig =
                    GameManager.Instance.voxelConfigs.Find(vt => vt.voxelType == voxelType);
                return voxelConfig;
            }

            /// <summary>
            /// 获取Voxel纹理的ID
            /// </summary>
            /// <param name="voxelFaceType">Voxel的朝向</param>
            /// <param name="textureAtlasSize">贴图中所有纹理的个数 如:4x4</param>
            /// <returns>纹理ID</returns>
            public int GetTextureID(
                VoxelFaceType voxelFaceType,
                int textureAtlasSize)
            {
                var voxelConfig = GetVoxelConfig();

                return voxelFaceType switch
                {
                    VoxelFaceType.Back => voxelConfig.backFaceTexture +
                                          voxelConfig.page * textureAtlasSize,
                    VoxelFaceType.Front => voxelConfig.frontFaceTexture +
                                           voxelConfig.page * textureAtlasSize,
                    VoxelFaceType.Top => voxelConfig.topFaceTexture +
                                         voxelConfig.page * textureAtlasSize,
                    VoxelFaceType.Bottom => voxelConfig.bottomFaceTexture +
                                            voxelConfig.page * textureAtlasSize,
                    VoxelFaceType.Left => voxelConfig.leftFaceTexture +
                                          voxelConfig.page * textureAtlasSize,
                    VoxelFaceType.Right => voxelConfig.rightFaceTexture +
                                           voxelConfig.page * textureAtlasSize,
                    _ => 0
                };
            }

        }

        public override string ToString()
        {
            return _chunkCoord.ToString();
        }

    }
}
