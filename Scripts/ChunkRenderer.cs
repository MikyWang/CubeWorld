using System;
using System.Threading;
using System.Threading.Tasks;
using MilkSpun.CubeWorld.Managers;
using MilkSpun.CubeWorld.Models;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace MilkSpun.CubeWorld
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
    public partial class ChunkRenderer : MonoBehaviour
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
        }
        private void ReGenerateMeshCollider(Mesh mesh)
        {
            Physics.BakeMesh(mesh.GetInstanceID(), false);
            _meshCollider.sharedMesh = mesh;
        }


    }
}
