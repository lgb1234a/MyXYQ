using System;

namespace Framework
{
    [Serializable]
    public class MapInfo : TextAssetRWAblity<MapInfo>
    {
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
    }
}