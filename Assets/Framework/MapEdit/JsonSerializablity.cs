using UnityEngine;

namespace Framework
{
    public class JsonSerializablity<T>
    {
        public string Serialize()
        {
            string jsonString = JsonUtility.ToJson(this);
            return jsonString;
        }

        public static T Deserialize(string content)
        {
            var t = JsonUtility.FromJson<T>(content);
            return t;
        }
    }
}