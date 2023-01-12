using UnityEngine;
namespace Framework
{
    public static class Vector2Extension
    {
        public static int GetMapGridIndex(this Vector2Int vector, int rows)
        {
            return vector.x * rows + vector.y;
        }
    }
}