using UnityEngine;
using UnityEditor;
using System.IO;

namespace Framework
{
    public class MapGrid : MonoBehaviour
    {
        private bool m_isShow = true;
        private IMapInfo m_mapInfo;

        void InitMapData()
        {
            string path = Environment.GetMapJsonDataPath(transform.GetChild(0).name);
            if (File.Exists(path))
            {
                m_mapInfo = MapInfo.ImportFromFile(path);
            }else
            {
                m_mapInfo = new MapInfo();
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

        public IMapInfo GetMapInfo()
        {
            return m_mapInfo;
        }

        public void SetMapInfo(IMapInfo mapInfo)
        {
            m_mapInfo = mapInfo;
        }

        public void RecalculateGridData(Vector3 mapSize, int rows, int columns)
        {
            m_mapInfo.SetGridRows(rows);
            m_mapInfo.SetGridColumns(columns);
            m_mapInfo.SetGridWidth(mapSize.x / columns);
            m_mapInfo.SetGridHeight(mapSize.y / rows);
            m_mapInfo.SetGridValues(new int[rows * columns]);
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
            for (int i = 0; i < m_mapInfo.GetGridColumns(); i++) {
                float x = Mathf.Min(m_mapInfo.GetGridWidth() * i, sr.sprite.bounds.size.x);
                Gizmos.DrawLine(new Vector2(x,0), new Vector2(x, sr.sprite.bounds.size.y));
            }

            for (int i = 0; i < m_mapInfo.GetGridRows(); i++) {
                float y = Mathf.Min(m_mapInfo.GetGridHeight() * i, sr.sprite.bounds.size.y);
                Gizmos.DrawLine(new Vector2(0, y), new Vector2(sr.sprite.bounds.size.x, y));
            }

            // 画正方体
            for (int i = 0; i < m_mapInfo.GetGridColumns(); i++)
            {
                for (int j = 0; j < m_mapInfo.GetGridRows(); j ++)
                {
                    var coordinate = new Vector2Int(i, j);
                    var gridWidth = m_mapInfo.GetGridWidth();
                    var gridHeight = m_mapInfo.GetGridHeight();
                    if (m_mapInfo.IsCoordinateObstacle(coordinate))
                    {
                        // 不可走，红色
                        Gizmos.color = new Color(1,0,0,0.5f);
                        Gizmos.DrawCube(new Vector3(i * gridWidth + gridWidth*0.5f, j*gridHeight + gridHeight*0.5f, 0), new Vector3(gridWidth, gridHeight, 0.1f));
                    }

                    if (m_mapInfo.IsCoordinateOcclusion(coordinate))
                    {
                        // 被遮挡，蓝色
                        Gizmos.color = new Color(0,0,1,0.5f);
                        Gizmos.DrawCube(new Vector3(i * gridWidth + gridWidth*0.5f, j*gridHeight + gridHeight*0.5f, 0), new Vector3(gridWidth, gridHeight, 0.1f));
                    }
                }
            }
        }
    }
}
