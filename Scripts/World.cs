using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using MilkSpun.CubeWorld.Models;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MilkSpun.CubeWorld
{
    public class World
    {
        public Transform Transform => _worldObject.transform;
        private static ChunkConfig ChunkConfig => GameManager.Instance.chunkConfig;

        private GameObject _worldObject;
        private Chunk[,] _chunks;

        public World()
        {
            _chunks = new Chunk[ChunkConfig.chunkCoordSize, ChunkConfig.chunkCoordSize];
            GenerateWorld();
        }
        public World GenerateWorld()
        {
            GenerateWorldObject();
            _ = PopulateBottomLeftWorld();
            _ = PopulateBottomRightWorld();
            _ = PopulateTopLeftWorld();
            _ = PopulateTopRightWorld();
            
            return this;
        }
        private void GenerateWorldObject()
        {
            if (_worldObject)
            {
                Object.DestroyImmediate(_worldObject);
            }
            _worldObject = new GameObject("World");
        }
        private async Task PopulateBottomLeftWorld()
        {
            for (var z = ChunkConfig.chunkCoordSize / 2 - 1; z >= 0; z--)
            {
                for (var x = ChunkConfig.chunkCoordSize / 2 - 1; x >= 0; x--)
                {
                    await Task.Yield();
                    _chunks[x, z] = new Chunk(new ChunkCoord(x, z));
                }
            }
        }

        private async Task PopulateTopLeftWorld()
        {
            for (var z = ChunkConfig.chunkCoordSize / 2; z < ChunkConfig.chunkCoordSize; z++)
            {
                for (var x = ChunkConfig.chunkCoordSize / 2 - 1; x >= 0; x--)
                {
                    await Task.Yield();
                    _chunks[x, z] = new Chunk(new ChunkCoord(x, z));
                }
            }
        }
        private async Task PopulateTopRightWorld()
        {
            for (var z = ChunkConfig.chunkCoordSize / 2; z < ChunkConfig.chunkCoordSize; z++)
            {
                for (var x = ChunkConfig.chunkCoordSize / 2; x < ChunkConfig.chunkCoordSize; x++)
                {
                    await Task.Yield();
                    _chunks[x, z] = new Chunk(new ChunkCoord(x, z));
                }
            }
        }
        private async Task PopulateBottomRightWorld()
        {
            for (var z = ChunkConfig.chunkCoordSize / 2 - 1; z >= 0; z--)
            {
                for (var x = ChunkConfig.chunkCoordSize / 2; x < ChunkConfig.chunkCoordSize; x++)
                {
                    await Task.Yield();
                    _chunks[x, z] = new Chunk(new ChunkCoord(x, z));
                }
            }
        }

       

    }
}
