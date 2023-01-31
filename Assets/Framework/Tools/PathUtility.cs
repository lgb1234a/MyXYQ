using System;
using System.IO;
namespace Framework
{
    public class PathUtility
    {
        public static string GetResourcesRelativePath(string path) {
            var ext = Path.GetExtension(path);
            var filePath = path.Replace(ext, "");
            string resourceDir = "/Resources/";
            int index = filePath.LastIndexOf(resourceDir, StringComparison.Ordinal);
            if (index >= 0)
            {
                filePath = filePath.Substring(index + resourceDir.Length);
            }
            return filePath;
        }
    }
}