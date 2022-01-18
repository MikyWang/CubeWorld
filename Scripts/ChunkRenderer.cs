using System.Threading;
using MilkSpun.CubeWorld.Models;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace MilkSpun.CubeWorld
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
    public class ChunkRenderer : MonoBehaviour
    {
        private MeshFilter _meshFilter;
        private MeshCollider _meshCollider;

        public Chunk Chunk { get; set; }

        [OnInspectorInit]
        private void Awake()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _meshCollider = GetComponent<MeshCollider>();
        }
        public void Refresh()
        {
            gameObject.name = Chunk.ToString();
            transform.localPosition = Chunk.LocalPosition;
            var mesh = Chunk.ConvertToMesh();
            _meshFilter.mesh = mesh;
            ReGenerateMeshCollider(mesh);
            Chunk.ClearData();
        }

        private void ReGenerateMeshCollider(Mesh mesh)
        {
            Physics.BakeMesh(mesh.GetInstanceID(), false);
            _meshCollider.sharedMesh = mesh;
        }

        [Button("预览地块")]
        private async void TestChunk()
        {
            Chunk= new Chunk(new ChunkCoord(5, 5));
            await Chunk.CreateChunk();
            Refresh();
        }
    }
}
