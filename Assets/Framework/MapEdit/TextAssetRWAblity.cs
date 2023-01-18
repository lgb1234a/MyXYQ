using UnityEngine;
using UnityEditor;
using System.IO;

namespace Framework
{
    public class TextAssetRWAblity {
        public static void ExportToFile(object obj, string path) {
            if (!File.Exists(path)) {
                FileStream fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite);
                fs.Close();
            }

            var content = JsonSerializablity.Serialize(obj);
            File.WriteAllText(path, content);
            AssetDatabase.Refresh();
        }

        public static T ImportFromFile<T>(string path) {
            if (!File.Exists(path)) {
                return default;
            }

            UnityEngine.Object textObj = AssetDatabase.LoadMainAssetAtPath(path);
            TextAsset textAsset = TextAsset.Instantiate<TextAsset>(textObj as TextAsset);
            return JsonSerializablity.Deserialize<T>(textAsset.text);
        }
    }
}