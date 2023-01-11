using UnityEngine;
using UnityEditor;
using Framework;

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
                        
                        m_mapGrid.SetGridValue(tx, ty, m_gridValue);
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
                        
                        m_mapGrid.SetGridValue(tx, ty, 0);
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
            m_exportFileName = EditorGUILayout.TextField("ExportName", m_exportFileName);
            m_importFileName = EditorGUILayout.TextField("ImportName", m_importFileName);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("导出"))
            {
                OutputFile(m_exportFileName);
            }
            
            if (GUILayout.Button("导入"))
            {
                InputFile(m_importFileName);
            }
            EditorGUILayout.EndHorizontal();
        }


        void OutputFile(string fileName)
        {
            MapInfo info = new MapInfo(m_mapGrid.GetGridWidth(), m_mapGrid.GetGridHeight(), m_mapGrid.GetGridValues(), m_mapGrid.GetGridRows(), m_mapGrid.GetGridColumns());
            string path = Application.dataPath + "/Resources/Text/Map/" + fileName + ".txt";
            info.ExportToFile(path);
        }

        void InputFile(string fileName)
        {
            string path = "Assets/Resources/Text/Map/" + fileName + ".txt";
            var mapInfo = MapInfo.ImportFromFile(path);
            m_mapGrid.SetGridValues(mapInfo.m_gridValues);
        }
    }
}
