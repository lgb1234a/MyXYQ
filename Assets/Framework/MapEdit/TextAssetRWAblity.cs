using UnityEngine;
using UnityEditor;
using System.IO;

namespace Framework
{
    public class TextAssetRWAblity<T> : JsonSerializablity<T>
    {
        public void ExportToFile(string path)
        {
            if (!File.Exists(path))
            {
                FileStream fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite);
                fs.Close();
            }

            var content = Serialize();
            File.WriteAllText(path, content);
            AssetDatabase.Refresh();
        }

        public static T ImportFromFile(string path)
        {
            if (!File.Exists(path))
            {
                return default;
            }

            UnityEngine.Object textObj = AssetDatabase.LoadMainAssetAtPath(path);
            TextAsset textAsset = TextAsset.Instantiate<TextAsset>(textObj as TextAsset);
            return Deserialize(textAsset.text);
        }
    }
}