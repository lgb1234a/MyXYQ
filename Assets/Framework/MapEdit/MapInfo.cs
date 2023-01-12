using System;
using UnityEngine;

namespace Framework
{
    [Serializable]
    public class MapInfo : TextAssetRWAblity<MapInfo>
    {
        // 路径
        public static int Path_Value = -1;
        // 障碍物
        public static int Obstacle_Value = 1;
        // 遮挡
        public static int Occlusion_Value = 2;
        public float m_gridWidth;
        public float m_gridHeight;
        public int[] m_gridValues;  //一维数组，左下角为原点，从下到上，从左往右的顺序记录
        public int m_rows;
        public int m_columns;

        public MapInfo(float gridWidth, float gridHeight, int[] gridValues, int rows, int columns)
        {
            m_gridWidth = gridWidth;
            m_gridHeight = gridHeight;
            m_gridValues = gridValues;
            m_rows = rows;
            m_columns = columns;
        }

        public void SetGridValue(Int2 coordinate, int value)
        {
            m_gridValues[GridCoordinate2Index(coordinate)] = value;
        }

        public Int2 GridIndex2Coordinate(int index)
        {
            int x = index / m_rows;
            int y = index % m_columns;
            return new Int2(x, y);
        }

        public int GridCoordinate2Index(Int2 coordinate)
        {
            int index = coordinate.x*m_rows + coordinate.y;
            return index;
        }

        public Vector2 GridIndex2WorldPosition(int index)
        {
            Int2 coordinate = GridIndex2Coordinate(index);
            float positionX = coordinate.x * m_gridWidth + m_gridWidth * 0.5f;
            float positionY = coordinate.y * m_gridHeight + m_gridHeight * 0.5f;
            return new Vector2(positionX, positionY);
        }

        public Vector2 WorldPosition2GridCoordinate(Vector2 position)
        {
            int tx = (int)(position.x / m_gridWidth);
            int ty = (int)(position.y / m_gridHeight);
            return new Vector2(tx, ty);
        }
    }
}