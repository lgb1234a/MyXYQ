using UnityEngine;
using UnityEditor;
using Framework;
using System.IO;

namespace FrameworkEditor
{
    [CustomEditor(typeof(MapGrid))]
    public class MapGridInspectorEditor : Editor {
        private MapGrid m_mapGrid;
        private bool m_editMode;
        private int m_gridValue;

        private string m_exportFileName;
        private string m_importFileName;
        void OnEnable() {
            m_mapGrid = target as MapGrid;
            var spriteRenderer = m_mapGrid.transform.GetChild(0);
            m_exportFileName = spriteRenderer.name;
            m_importFileName = spriteRenderer.name;
        }

        void OnSceneGUI() {
            if (m_editMode) {
                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
                m_mapGrid.Show();
                Event e = Event.current;
                if (e.button == 0 && (e.type == EventType.MouseDrag || e.type == EventType.MouseDown)) {
                    // 获取鼠标产生的射线
                    Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                    RaycastHit hitInfo;
                    if (Physics.Raycast(ray, out hitInfo, 200f)) {
                        m_mapGrid.ClickedToMarkFlagValue(hitInfo.point, m_gridValue);
                        HandleUtility.Repaint();
                    }
                }

                if (e.type == EventType.MouseDown && e.button == 1) {
                    // 获取鼠标产生的射线
                    Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                    RaycastHit hitInfo;
                    if (Physics.Raycast(ray, out hitInfo, 200f)) {
                        m_mapGrid.ClickedToUnmarkFlagValue(hitInfo.point);
                        HandleUtility.Repaint();
                    }
                }
            }else {
                // 恢复编辑器的选择
                HandleUtility.Repaint();
                m_mapGrid.Hide();
            }
        }

        public override void OnInspectorGUI() {
            m_editMode = EditorGUILayout.Toggle("EditMode", m_editMode);
            m_gridValue = EditorGUILayout.IntSlider("GridValue", m_gridValue, 1, 2);
            EditorGUILayout.Space(10f);

            EditorGUILayout.BeginHorizontal();
            m_exportFileName = EditorGUILayout.TextField("ExportName", m_exportFileName);
            if (GUILayout.Button("保存")) {
                if (File.Exists(Environment.GetMapJsonDataPath(m_exportFileName))) {
                    var tip = $"目前已存在名为{m_exportFileName}的文件，是否覆盖？";
                    var r = EditorUtility.DisplayDialog("Warning", tip, "确定", "不覆盖");
                    if (r)
                        ExportFile(m_exportFileName);
                }
            }
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();
            m_importFileName = EditorGUILayout.TextField("ImportName", m_importFileName);
            if (GUILayout.Button("导入")) {
                var tip = $"导入会重置掉当前所有改动，是否继续？";
                var r = EditorUtility.DisplayDialog("Warning", tip, "确定", "不继续");
                if (r)
                    ImportFile(m_importFileName);
            }
            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("自动填充")) {
                m_mapGrid.AutoFillGridsColor();
                UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
            }
            if (GUILayout.Button("清空")) {
                m_mapGrid.ClearFlags();
                UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
            }
        }


        void ExportFile(string fileName) {
            var info = m_mapGrid.GetMapInfo() as MapInfo;
            string path = Environment.GetMapJsonDataPath(fileName);
            TextAssetRWAblity.ExportToFile(info, path);
        }

        void ImportFile(string fileName) {
            string path = Environment.GetMapJsonDataPath(fileName);
            var mapInfo = TextAssetRWAblity.ImportFromFile<MapInfo>(path);
            m_mapGrid.SetMapInfo(mapInfo);
        }
    }
}
