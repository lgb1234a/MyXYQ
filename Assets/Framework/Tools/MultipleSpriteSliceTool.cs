using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System.IO;

namespace FrameworkEditor
{
    public class MultipleSpriteSliceTool
    {
        [MenuItem("Assets/Slice Texture Manual", true)]
        private static bool ShouldShowManual() {
            if (Selection.activeObject is Texture2D) {
                var path = AssetDatabase.GetAssetPath(Selection.activeObject);
                TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
                if (ti.spriteImportMode == SpriteImportMode.Multiple)
                    return ti.spritesheet.Length > 1;
            }
            return false;
        }

        [MenuItem("Assets/Slice Texture Manual")]
        private static void StartManual() {
            var myTexture = Selection.activeObject as Texture2D;
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
            ti.isReadable = true;
            foreach(var spritesheet in ti.spritesheet) {
                var texture = CreateSubTexture(myTexture, spritesheet.rect);
                var directory = Path.GetDirectoryName(path);
                var newTextureName = $"{spritesheet.name}.png";
                var subTexturePath = Path.Combine(directory, newTextureName);
                File.WriteAllBytes(subTexturePath, texture.EncodeToPNG());
                AssetDatabase.ImportAsset(subTexturePath);
            }
        }

        [MenuItem("Assets/Slice Texture Auto", true)]
        private static bool ShouldShowAuto()
        {
            return Selection.activeObject is Texture2D;
        }

        [MenuItem("Assets/Slice Texture Auto")]
        private static void StartAuto()
        {
            var myTexture = Selection.activeObject as Texture2D;
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);

            int minimumSpriteSize = 16;
            int extrudeSize = 0;
            Rect[] rects = InternalSpriteUtility.GenerateAutomaticSpriteRectangles(myTexture, minimumSpriteSize, extrudeSize);
            var rectsList = new List<Rect>(rects);
            rectsList = SortRects(rectsList, myTexture.height);

            var fileName = Path.GetFileNameWithoutExtension(path);
            int rectNum = 0;
            
            foreach (var rect in rectsList) {
                var texture = CreateSubTexture(myTexture, rect);
                var directory = Path.GetDirectoryName(path);
                var newTextureName = $"{fileName}_{rectNum++}.png";
                var subTexturePath = Path.Combine(directory, newTextureName);
                File.WriteAllBytes(subTexturePath, texture.EncodeToPNG());
                AssetDatabase.ImportAsset(subTexturePath);
            }
            AssetDatabase.SaveAssets();
        }


        private static List<Rect> SortRects(List<Rect> rects, float textureHeight) {
            List<Rect> list = new List<Rect>();
            while (rects.Count > 0) {
                Rect rect = rects[rects.Count - 1];
                var sweepRect = new Rect(0f, rect.yMin, rect.width, textureHeight);
                var list2 = RectSweep(rects, sweepRect);
                if (list2.Count <= 0) {
                    list.AddRange(rects);
                    break;
                }
                list.AddRange(list2);
            }
            return list;
        }

        private static List<Rect> RectSweep(List<Rect> rects, Rect sweepRect) {
            List<Rect> result;
            if (rects == null) 
                result = new List<Rect>();
            else {
                List<Rect> list = new List<Rect>();
                foreach(var current in rects) {
                    if (current.Overlaps(sweepRect))
                        list.Add(current);
                }

                foreach(var current in list) {
                    rects.Remove(current);
                }
                list.Sort((a,b) => a.x.CompareTo(b.x));
                result = list;
            }
            return result;
        }

        private static Texture2D CreateSubTexture(Texture2D texture, Rect rect) {
            var croppedTexture = new Texture2D((int)rect.width, (int)rect.height);
            var pixels = texture.GetPixels(
                (int)rect.x,
                (int)rect.y,
                (int)rect.width,
                (int)rect.height
            );
            croppedTexture.SetPixels(pixels);
            croppedTexture.Apply();
            return croppedTexture;
        }
    }
}