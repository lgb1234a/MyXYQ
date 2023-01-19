using UnityEngine;
using System.IO;

namespace Framework
{
    public class MapGrid : MonoBehaviour {
        private bool m_isShow = true;
        private IMapInfo m_mapInfo;
        public bool m_editMode;

        void InitMapData() {
            string path = Environment.GetMapJsonDataPath(transform.GetChild(0).name);
            if (File.Exists(path)) {
                m_mapInfo = TextAssetRWAblity.ImportFromFile<MapInfo>(path);
            }
            else {
                m_mapInfo = new MapInfo();
            }
        }

        // 当编辑器运行或者停止运行都会触发，重新刷新
        void OnValidate() {
            InitMapData();
        }

        public void Show() {
            m_isShow = true;
        }

        public void Hide() {
            m_isShow = false;
        }

        public IMapInfo GetMapInfo() {
            return m_mapInfo;
        }

        public void SetMapInfo(IMapInfo mapInfo) {
            m_mapInfo = mapInfo;
        }

        public void RecalculateGridData(Vector3 mapSize, int rows, int columns) {
            m_mapInfo.SetGridRows(rows);
            m_mapInfo.SetGridColumns(columns);
            m_mapInfo.SetGridWidth(mapSize.x / columns);
            m_mapInfo.SetGridHeight(mapSize.y / rows);
            m_mapInfo.SetGridValues(new int[rows*columns]);
        }

        public void ClickedToMarkFlagValue(Vector3 point, int flagValue) {
            var coordinate = m_mapInfo.WorldPosition2GridCoordinate(point);
            if (m_mapInfo.IsCoordinateDefaultValue(coordinate)) 
                m_mapInfo.SetGridValue(coordinate, flagValue);
        }

        public void ClickedToUnmarkFlagValue(Vector3 point) {
            var coordinate = m_mapInfo.WorldPosition2GridCoordinate(point);
            m_mapInfo.SetGridValue(coordinate, 0);
        }

        public void ClearFlags() {
            var count = m_mapInfo.GetGridValues().Length;
            for (int i = 0; i < count; i++) {
                m_mapInfo.GetGridValues()[i] = 0;
            }
        }

        // 任何不在边界上，或者不与边界上的0相邻的0都会被填充为1
        public void AutoFillGridsColor() {
            var board = m_mapInfo.GetGridValues();
            var rows = m_mapInfo.GetGridRows();
            var columns = m_mapInfo.GetGridColumns();
            for (int i = 0; i < rows; i++) {
                Dfs(board, i, 0);
                Dfs(board, i, columns - 1);
            }

            for(int j = 0; j < columns; j++) {
                Dfs(board, 0, j);
                Dfs(board, rows-1, j);
            }

            for(int i = 0; i < rows; i++) {
                for (int j = 0; j < columns; j++) {
                    var index = m_mapInfo.GridCoordinate2Index(new Vector2Int(i, j));
                    if (board[index] == 0)
                        board[index] = DFS2FindValue(board, i, j);
                    if(board[index] == -9999)
                        board[index] = 0;
                }
            }
        }

        // 为了让封闭区域内填充的值和边界一致，需要递归获取封闭区域边界的值
        int DFS2FindValue(int[] board, int i, int j) {
            var index = m_mapInfo.GridCoordinate2Index(new Vector2Int(i, j));
            var rows = m_mapInfo.GetGridRows();
            var columns = m_mapInfo.GetGridColumns();
            if (board[index] != 0) {
                return board[index];
            }

            return DFS2FindValue(board,i-1,j);
        }

        void Dfs(int[] board, int i, int j) {
            var index = m_mapInfo.GridCoordinate2Index(new Vector2Int(i, j));
            var rows = m_mapInfo.GetGridRows();
            var columns = m_mapInfo.GetGridColumns();
            if(i<0||j<0||i>=rows||j>=columns|| board[index] != 0)
                return;
            board[index] = -9999;
            Dfs(board,i-1,j);
            Dfs(board,i+1,j);
            Dfs(board,i,j-1);
            Dfs(board,i,j+1);
            return ;
        }

        void OnDrawGizmos() {
            if (!m_isShow) {
                return;
            }
            Gizmos.color = Color.white;
            var mapSprite = transform.GetChild(0);
            var sr = mapSprite.GetComponent<SpriteRenderer>();
            for (int i = 0; i < m_mapInfo.GetGridColumns(); i++) {
                float x = Mathf.Min(m_mapInfo.GetGridWidth()*i, sr.sprite.bounds.size.x);
                Gizmos.DrawLine(new Vector2(x,0), new Vector2(x, sr.sprite.bounds.size.y));
            }

            for (int i = 0; i < m_mapInfo.GetGridRows(); i++) {
                float y = Mathf.Min(m_mapInfo.GetGridHeight()*i, sr.sprite.bounds.size.y);
                Gizmos.DrawLine(new Vector2(0, y), new Vector2(sr.sprite.bounds.size.x, y));
            }

            // 画正方体
            for (int i = 0; i < m_mapInfo.GetGridColumns(); i++) {
                for (int j = 0; j < m_mapInfo.GetGridRows(); j ++) {
                    var coordinate = new Vector2Int(i, j);
                    var gridWidth = m_mapInfo.GetGridWidth();
                    var gridHeight = m_mapInfo.GetGridHeight();
                    var gridCenterX = i*gridWidth + gridWidth*0.5f;
                    var gridCenterY = j*gridHeight + gridHeight*0.5f;
                    if (m_mapInfo.IsCoordinateObstacle(coordinate)) {
                        // 不可走，红色
                        Gizmos.color = new Color(1,0,0,0.5f);
                        Gizmos.DrawCube(new Vector3(gridCenterX, gridCenterY, 0), new Vector3(gridWidth, gridHeight, 0.1f));
                    }

                    if (m_mapInfo.IsCoordinateOcclusion(coordinate)) {
                        // 被遮挡，蓝色
                        Gizmos.color = new Color(0,0,1,0.5f);
                        Gizmos.DrawCube(new Vector3(gridCenterX, gridCenterY, 0), new Vector3(gridWidth, gridHeight, 0.1f));
                    }
                }
            }
        }
    }
}
