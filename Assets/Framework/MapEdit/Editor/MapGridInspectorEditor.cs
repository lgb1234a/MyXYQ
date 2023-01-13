using UnityEngine;
using UnityEditor;
using Framework;
using System.IO;

namespace FrameworkEditor
{
    [CustomEditor(typeof(MapGrid))]
    public class MapGridInspectorEditor : Editor
    {
        private MapGrid m_mapGrid;
        private bool m_editMode = true;
        private int m_gridValue;

        private string m_exportFileName;
        private string m_importFileName;
        void OnEnable()
        {
            m_mapGrid = target as MapGrid;
            var spriteRenderer = m_mapGrid.transform.GetChild(0);
            m_exportFileName = spriteRenderer.name;
            m_importFileName = spriteRenderer.name;
        }

        void OnSceneGUI()
        {
            if (m_editMode)
            {
                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
                m_mapGrid.Show();
                Event e = Event.current;
                if (e.button == 0 && (e.type == EventType.MouseDrag || e.type == EventType.MouseDown))
                {
                    // 获取鼠标产生的射线
                    Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                    RaycastHit hitInfo;
                    if (Physics.Raycast(ray, out hitInfo, 200f))
                    {
                        int tx = (int)(hitInfo.point.x / m_mapGrid.GetGridWidth());
                        int ty = (int)(hitInfo.point.y / m_mapGrid.GetGridHeight());
                        var coordinate = new Vector2Int(tx, ty);
                        m_mapGrid.SetGridValue(coordinate, m_gridValue);
                        HandleUtility.Repaint();
                    }
                }

                if (e.type == EventType.MouseDown && e.button == 1)
                {
                    // 获取鼠标产生的射线
                    Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                    RaycastHit hitInfo;
                    if (Physics.Raycast(ray, out hitInfo, 200f))
                    {
                        int tx = (int)(hitInfo.point.x / m_mapGrid.GetGridWidth());
                        int ty = (int)(hitInfo.point.y / m_mapGrid.GetGridHeight());
                        var coordinate = new Vector2Int(tx, ty);
                        m_mapGrid.SetGridValue(coordinate, 0);
                        HandleUtility.Repaint();
                    }
                }
            }else {
                // 恢复编辑器的选择
                HandleUtility.Repaint();
                m_mapGrid.Hide();
            }
        }

        public override void OnInspectorGUI()
        {
            m_editMode = EditorGUILayout.Toggle("EditMode", m_editMode);
            m_gridValue = EditorGUILayout.IntSlider("GridValue", m_gridValue, 1, 2);
            EditorGUILayout.Space(10f);

            EditorGUILayout.BeginHorizontal();
            m_exportFileName = EditorGUILayout.TextField("ExportName", m_exportFileName);
            if (GUILayout.Button("保存"))
            {
                if (File.Exists(Environment.GetMapJsonDataPath(m_exportFileName)))
                {
                    var tip = $"目前已存在名为{m_exportFileName}的文件，是否覆盖？";
                    var r = EditorUtility.DisplayDialog("Warning", tip, "确定", "不覆盖");
                    if (r)
                        ExportFile(m_exportFileName);
                }
            }
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();
            m_importFileName = EditorGUILayout.TextField("ImportName", m_importFileName);
            if (GUILayout.Button("导入"))
            {
                var tip = $"导入会重置掉当前所有改动，是否继续？";
                var r = EditorUtility.DisplayDialog("Warning", tip, "确定", "不继续");
                if (r)
                    ImportFile(m_importFileName);
            }
            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("自动填充"))
            {
                Solve();
                // var count = m_mapGrid.GetGridValues().Length;
                // for (int i = 0; i < count; i++)
                // {
                //     m_mapGrid.GetGridValues()[i] = 0;
                // }
                UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
            }
            if (GUILayout.Button("清空"))
            {
                var count = m_mapGrid.GetGridValues().Length;
                for (int i = 0; i < count; i++)
                {
                    m_mapGrid.GetGridValues()[i] = 0;
                }
                UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
            }
        }


        void ExportFile(string fileName)
        {
            var info = m_mapGrid.GetMapInfo();
            string path = Environment.GetMapJsonDataPath(fileName);
            info.ExportToFile(path);
        }

        void ImportFile(string fileName)
        {
            string path = Environment.GetMapJsonDataPath(fileName);
            var mapInfo = MapInfo.ImportFromFile(path);
            m_mapGrid.SetMapInfo(mapInfo);
        }

        // 任何不在边界上，或者不与边界上的0相邻的0都会被填充为1
        void Solve() {
            var board = m_mapGrid.GetMapInfo().m_gridValues;
            var rows = m_mapGrid.GetMapInfo().m_rows;
            var columns = m_mapGrid.GetMapInfo().m_columns;
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
                    var index = m_mapGrid.GetMapInfo().GridCoordinate2Index(new Vector2Int(i, j));
                    if (board[index] == 0)
                        board[index] = 1;
                    if(board[index] == -9999)
                        board[index] = 0;
                }
            }
        }

        void Dfs(int[] board, int i, int j)
        {
            var index = m_mapGrid.GetMapInfo().GridCoordinate2Index(new Vector2Int(i, j));
            var rows = m_mapGrid.GetMapInfo().m_rows;
            var columns = m_mapGrid.GetMapInfo().m_columns;
            if(i<0||j<0||i>=rows||j>=columns|| board[index] != 0)
                return;
            board[index] = -9999;
            Dfs(board,i-1,j);
            Dfs(board,i+1,j);
            Dfs(board,i,j-1);
            Dfs(board,i,j+1);
            return ;
        }
    }
}
