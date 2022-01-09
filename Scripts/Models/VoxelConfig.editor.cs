#if UNITY_EDITOR
using System.Threading.Tasks;
using MilkSpun.Common;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
namespace MilkSpun.CubeWorld.Models
{
    public partial class VoxelConfig
    {
        [ShowInInspector, ReadOnly]
        [PreviewField(145f, ObjectFieldAlignment.Right)]
        public Texture2D[] TexturesPreview { get; private set; }

        [OnInspectorInit]
        private async Task ShowTexture()
        {
            await Task.Yield();
            if (!isSolid) return;

            TexturesPreview = new Texture2D[2];
            var path = $"{ConstStrings.TextureAssetPath}/BlockTextures.asset";
            var array = AssetDatabase.LoadAssetAtPath<Texture2DArray>(path);

            TexturesPreview[0] =
                TextureBuilder.GetPartTextureFromTexture2DArray(array, page, 16, topFaceTexture);
            TexturesPreview[1] =
                TextureBuilder.GetPartTextureFromTexture2DArray(array, page, 16, leftFaceTexture);
        }
    }
}
#endif
