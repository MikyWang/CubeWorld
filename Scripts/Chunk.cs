using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using MilkSpun.Common;
using MilkSpun.CubeWorld.Managers;
using MilkSpun.CubeWorld.Models;
using MilkSpun.CubeWorld.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MilkSpun.CubeWorld
{
    public class Chunk : IEquatable<Chunk>
    {
        private readonly ChunkCoord _chunkCoord;
        private readonly ChunkMeshData _meshData;
        private readonly Voxel[,,] _voxels;
        private ChunkRenderer _chunkRenderer;
        private int _verticesIndex;
        private Task _populateTask;
        public Vector3 Position { get; }
        public Vector3 LocalPosition { get; }
        private static ChunkConfig ChunkConfig => GameManager.Instance.chunkConfig;
        private static ChunkRenderer ChunkPrefab => GameManager.Instance.ChunkPrefab;
        private static World World => GameManager.Instance.World;

        public Chunk(ChunkCoord chunkCoord)
        {
            _meshData = new ChunkMeshData();
            _voxels = new Voxel[ChunkConfig.chunkWidth, ChunkConfig.chunkHeight, ChunkConfig.chunkWidth];
            this._chunkCoord = chunkCoord;
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
                voxel.voxelType = World.GenerateVoxelType(pos);
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
                    voxel.voxelType = World.GenerateVoxelType(pos);
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

        public async void CreateChunk()
        {
            if (_populateTask is not null)
            {
                await _populateTask;
                _populateTask = Populate();

            }
            else
            {
                _populateTask = Populate();
                await _populateTask;
            }
        }

        private async Task Populate()
        {
            for (var y = 0; y < ChunkConfig.chunkHeight; y++)
            {
                for (var z = 0; z < ChunkConfig.chunkWidth; z++)
                {
                    for (var x = 0; x < ChunkConfig.chunkWidth; x++)
                    {
                        PopulateVoxel(x, y, z);
                    }
                }
            }
            await Refresh();
        }

        public Mesh ConvertToMesh()
        {
            return new Mesh
            {
                name = _chunkCoord.ToString(),
                vertices = _meshData.vertices.ToArray(),
                normals = _meshData.normals.ToArray(),
                triangles = _meshData.triangles.ToArray(),
                uv = _meshData.uv.ToArray(),
                uv2 = _meshData.uv2.ToArray()
            };
        }

        private void PopulateVoxel(int x, int y, int z)
        {
            if (!_voxels[x, y, z].Initialize)
            {
                _voxels[x, y, z] = new Voxel(this, x, y, z);
                _voxels[x, y, z].voxelType = World.GenerateVoxelType(_voxels[x, y, z].LocalPos + LocalPosition);
            }
            ref var voxel = ref _voxels[x, y, z];

            var faces = Common.Utils.GetEnumValues<VoxelFaceType>();
            var faceConfigs = voxel.GetVoxelConfig().blockConfig.faceConfigs;

            foreach (var faceType in faces)
            {
                if (voxel.IsPlaneInvisible(faceType))
                {
                    continue;
                }
                var faceConfig = faceConfigs[(int)faceType];
                foreach (var vert in faceConfig.vertices)
                {
                    _meshData.vertices.Add(vert + voxel.LocalPos);
                    _meshData.normals.Add(ChunkConfig.VoxelFaceOffset[faceType]);
                }
                var textureID = voxel.GetTextureID(faceType, ChunkConfig.TextureAtlasSize);
                AddTexture(textureID, faceConfig.uv);

                foreach (var triangle in faceConfig.triangles)
                {
                    _meshData.triangles.Add(_verticesIndex + triangle);
                }
                _verticesIndex += faceConfig.vertices.Count;
            }
        }

        private void AddTexture(int textureID, IEnumerable<Vector2> uvs)
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

            const float uvOffset = 0.005f;
            foreach (var uv in uvs)
            {
                var ux = x + uv.x * normalizedBlockTextureSize + (1 - 2 * uv.x) * uvOffset;
                var uy = y + uv.y * normalizedBlockTextureSize + (1 - 2 * uv.y) * uvOffset;
                _meshData.uv.Add(new Vector2(ux, uy));
            }

            var index = textureID / textureAtlasSize;
            var v = index % 8;
            for (var i = 0; i < 4; i++)
            {
                _meshData.uv2.Add(new Vector2(v, v));
            }

        }
        public async Task Refresh()
        {
            await GameManager.RunInUnitySyncContext(() =>
            {
                _chunkRenderer ??= Object.Instantiate(ChunkPrefab, World.Transform);
                _chunkRenderer.Chunk = this;
                _chunkRenderer.Refresh();
            });
            ClearData();
        }

        public void ClearData()
        {
            _verticesIndex = 0;
            _meshData.Clear();
        }

        public override string ToString()
        {
            return _chunkCoord.ToString();
        }

        public struct Voxel
        {
            public readonly int x;
            public readonly int y;
            public readonly int z;
            public VoxelType voxelType;
            public bool Initialize { get; }
            public Chunk Chunk { get; }
            public Vector3 LocalPos => new(x, y, z);

            public Voxel(
                Chunk chunk,
                int x,
                int y,
                int z,
                VoxelType voxelType = VoxelType.Air)
            {
                this.x = x;
                this.y = y;
                this.z = z;
                this.voxelType = voxelType;
                Chunk = chunk;
                Initialize = true;
            }

            /// <summary>
            /// 判断Voxel的面是否不可见
            /// </summary>
            /// <param name="voxelFaceType">Voxel的朝向 <see cref="VoxelFaceType"/></param>
            /// <returns></returns>
            public bool IsPlaneInvisible(VoxelFaceType voxelFaceType)
            {
                var voxelConfig = GetVoxelConfig();
                if (!voxelConfig.isSolid) return true;

                var xCoordPos = this.x + Chunk.LocalPosition.x;
                var zCoordPos = this.z + Chunk.LocalPosition.z;

                var voxelFacePos = new Vector3(xCoordPos, this.y, zCoordPos) + ChunkConfig.VoxelFaceOffset[voxelFaceType];

                if (!World.CheckPositionOnGround(voxelFacePos.x, voxelFacePos.y, voxelFacePos.z))
                {
                    return false;
                }

                var chunk = World.GetChunkFromPosition(voxelFacePos.x, voxelFacePos.z);
                var voxel = chunk.GetVoxelFromPosition(voxelFacePos.x, voxelFacePos.y, voxelFacePos.z);
                if (voxel.GetVoxelConfig().isTransparency) return false;

                var lx = Mathf.FloorToInt(voxelFacePos.x);
                var ly = Mathf.FloorToInt(voxelFacePos.y);
                var lz = Mathf.FloorToInt(voxelFacePos.z);
                var width = ChunkConfig.WorldSizeInVoxels;
                var height = ChunkConfig.chunkHeight;

                return lx >= 0 &&
                       lx <= width - 1 &&
                       ly >= 0 &&
                       ly <= height - 1 &&
                       lz >= 0 &&
                       lz <= width - 1;
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
                var lVoxelType = voxelType;
                var voxelConfig =
                    GameManager.Instance.voxelConfigs.Find(vt => vt.voxelType == lVoxelType);
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
        public bool Equals(Chunk other)
        {
            return other != null && _chunkCoord == other._chunkCoord;
        }
        public override bool Equals(object obj)
        {
            return obj is Chunk other && Equals(other);
        }
        public override int GetHashCode()
        {
            return _chunkCoord.GetHashCode();
        }
    }
}
