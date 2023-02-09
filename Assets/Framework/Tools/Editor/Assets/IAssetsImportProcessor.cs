using UnityEngine;
using UnityEditor;
using UnityEditor.AssetImporters;

namespace FrameworkEditor
{
    public interface IAssetsImportProcessor
    {
        public void OnPreprocessTexture(string assetPath, AssetImportContext context, TextureImporter textureImporter);
        public void OnPostprocessTexture(string assetPath, AssetImportContext context, TextureImporter textureImporter, Texture2D texture);
        public void OnPostprocessSprites(string assetPath, AssetImportContext context, TextureImporter textureImporter, Texture2D texture, Sprite[] sprites);
    }
}