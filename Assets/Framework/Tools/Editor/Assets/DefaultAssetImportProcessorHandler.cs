using UnityEditor;
using UnityEngine;
using UnityEditor.AssetImporters;


namespace FrameworkEditor
{
    public class DefaultAssetImportProcessorHandler : IAssetsImportProcessor
    {
        public void OnPreprocessTexture(string assetPath, AssetImportContext context, TextureImporter textureImporter)
        {
            TextureImporterPlatformSettings tps = textureImporter.GetPlatformTextureSettings("Standalone");
            tps.compressionQuality = (int) TextureCompressionQuality.Best;
            tps.format = TextureImporterFormat.RGBA32;
            tps.overridden = true;
            textureImporter.SetPlatformTextureSettings(tps);
        }

        public void OnPostprocessTexture(string assetPath, AssetImportContext context, TextureImporter textureImporter, Texture2D texture)
        {

        }

        public void OnPostprocessSprites(string assetPath, AssetImportContext context, TextureImporter textureImporter, Texture2D texture, Sprite[] sprites)
        {

        }
    }
}