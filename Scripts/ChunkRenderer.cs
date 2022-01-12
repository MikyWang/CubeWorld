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
        
        private void Awake()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _meshCollider = GetComponent<MeshCollider>();
        }
        private void Start()
        {
            gameObject.name = Chunk.ToString();
            Refresh();
        }

        private void Refresh()
        {
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
    }
}
