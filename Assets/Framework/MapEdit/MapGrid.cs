using UnityEngine;
using UnityEditor;
using System.IO;

namespace Framework
{
    public class MapGrid : MonoBehaviour
    {
        private bool m_isShow = true;
        private float m_gridWidth;
        private float m_gridHeight;
        private int[] m_gridValues;  //一维数组，左下角为原点，从下到上，从左往右的顺序记录
        private int m_rows;
        private int m_columns;
        private MapInfo m_mapInfo;

        void InitMapData()
        {
            string path = Environment.GetMapJsonDataPath(transform.GetChild(0).name);
            if (File.Exists(path))
            {
                m_mapInfo = MapInfo.ImportFromFile(path);
                m_gridWidth = m_mapInfo.m_gridWidth;
                m_gridHeight = m_mapInfo.m_gridHeight;
                m_gridValues = m_mapInfo.m_gridValues;
                m_rows = m_mapInfo.m_rows;
                m_columns = m_mapInfo.m_columns;
            }
        }

        // 当编辑器运行或者停止运行都会触发，重新刷新
        void OnValidate()
        {
            InitMapData();
        }

        public void Show()
        {
            m_isShow = true;
        }

        public void Hide()
        {
            m_isShow = false;
        }

        public float GetGridWidth()
        {
            return m_gridWidth;
        }

        public float GetGridHeight()
        {
            return m_gridHeight;
        }

        public int[] GetGridValues()
        {
            return m_gridValues;
        }

        public void SetGridValues(int[] values)
        {
            m_gridValues = values;
        }

        public void SetGridValue(Vector2Int coordinate, int value)
        {
            int index = coordinate.x*m_rows + coordinate.y;
            m_gridValues[index] = value;
        }

        public int GetGridValue(Vector2Int coordinate)
        {
            int index = coordinate.x*m_rows + coordinate.y;
            return m_gridValues[index];
        }

        public int GetGridRows()
        {
            return m_rows;
        }

        public void SetGridRows(int rows)
        {
            m_rows = rows;
        }

        public int GetGridColumns()
        {
            return m_columns;
        }

        public void SetGridColumns(int columns)
        {
            m_columns = columns;
        }

        public void RecalculateGridData()
        {
            var mapSprite = transform.GetChild(0);
            var sr = mapSprite.GetComponent<SpriteRenderer>();
            m_gridWidth = sr.sprite.bounds.size.x / m_columns;
            m_gridHeight = sr.sprite.bounds.size.y / m_rows;
            m_gridValues = new int[m_rows * m_columns];
        }

        void OnDrawGizmos()
        {
            if (!m_isShow)
            {
                return;
            }
            Gizmos.color = Color.white;
            var mapSprite = transform.GetChild(0);
            var sr = mapSprite.GetComponent<SpriteRenderer>();
            for (int i = 0; i < m_columns; i++) {
                float x = Mathf.Min(m_gridWidth * i, sr.sprite.bounds.size.x);
                Gizmos.DrawLine(new Vector2(x,0), new Vector2(x, sr.sprite.bounds.size.y));
            }

            for (int i = 0; i < m_rows; i++) {
                float y = Mathf.Min(m_gridHeight * i, sr.sprite.bounds.size.y);
                Gizmos.DrawLine(new Vector2(0, y), new Vector2(sr.sprite.bounds.size.x, y));
            }

            // 画正方体
            for (int i = 0; i < m_columns; i++)
            {
                for (int j = 0; j < m_rows; j ++)
                {
                    var coordinate = new Vector2Int(i, j);
                    if (GetGridValue(coordinate) == MapInfo.Obstacle_Value)
                    {
                        // 不可走，红色
                        Gizmos.color = new Color(1,0,0,0.5f);
                        Gizmos.DrawCube(new Vector3(i * m_gridWidth + m_gridWidth*0.5f, j*m_gridHeight + m_gridHeight*0.5f, 0), new Vector3(m_gridWidth, m_gridHeight, 0.1f));
                    }

                    if (GetGridValue(coordinate) == MapInfo.Occlusion_Value)
                    {
                        // 被遮挡，蓝色
                        Gizmos.color = new Color(0,0,1,0.5f);
                        Gizmos.DrawCube(new Vector3(i * m_gridWidth + m_gridWidth*0.5f, j*m_gridHeight + m_gridHeight*0.5f, 0), new Vector3(m_gridWidth, m_gridHeight, 0.1f));
                    }
                }
            }
        }
    }
}
