using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace FrameworkEditor
{
    public class AssetsImportProcessor : AssetPostprocessor
    {
        private static IAssetsImportProcessor m_defaultProcessor;
        private static IAssetsImportProcessor m_customProcessor;

        public static IAssetsImportProcessor GetAssetPostProcessor() {
            if (m_customProcessor != null) {
                return m_customProcessor;
            }

            if (m_defaultProcessor == null) {
                m_defaultProcessor = new DefaultAssetImportProcessorHandler();
            }

            return m_defaultProcessor;
        }

        public static void SetAssetsImportProcessor(IAssetsImportProcessor processor) {
            m_customProcessor = processor;
        }

        private void OnPreprocessTexture()
        {
            GetAssetPostProcessor().OnPreprocessTexture(assetPath, context, assetImporter as TextureImporter);
        }
    }
}
