using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using Framework;

namespace FrameworkEditor
{
    public class MapCreator : EditorWindow
    {
        private MapGrid m_mapGrid;
        private UnityEngine.Object m_targetMap;
        private Texture2D m_mapTexture;
        
        private int m_rows = 0;
        private int m_columns = 0;
        private string m_mapName;


        [MenuItem("Tools/MapEditor")]
        static void Create()
        {
            var window = EditorWindow.GetWindow<MapCreator>();
            window.titleContent = new GUIContent("地图编辑器");
        }

        void OnGUI()
        {
            m_targetMap = EditorGUILayout.ObjectField("地图贴图", m_targetMap, typeof(UnityEngine.Texture2D), false, null);

            if (m_targetMap)
            {
                var mapPath = AssetDatabase.GetAssetPath(m_targetMap);
                m_mapTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(mapPath);
                
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"w:{m_mapTexture.width}  h:{m_mapTexture.height}");
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                m_rows = int.Parse(EditorGUILayout.TextField("行数", m_rows.ToString()));
                m_columns = int.Parse(EditorGUILayout.TextField("列数", m_columns.ToString()));
                if (GUILayout.Button("创建"))
                {
                    m_mapName = m_targetMap.name;
                    GenerateMap();
                }
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("清理"))
            {
                ClearMapGameObject();
            }
        }

        void GenerateMap()
        {
            if (m_targetMap != null) {
                var map = new GameObject("Map");

                GameObject mapRenderer = new GameObject(m_mapName);
                var sr = mapRenderer.AddComponent<SpriteRenderer>();
                var mapNavigationController = mapRenderer.AddComponent<MapNavigationController>();
                mapNavigationController.m_setPlayerLocationBtn = GameObject.Find("SetPlayerLocationBtn").GetComponent<Button>();
                mapNavigationController.m_setDestinationBtn = GameObject.Find("SetDestinationLocationBtn").GetComponent<Button>();
                mapNavigationController.m_aStarButton = GameObject.Find("AStarBtn").GetComponent<Button>();
                mapNavigationController.m_pathFlag = GameObject.Find("PathFlag");
                mapNavigationController.m_obstacleFlag = GameObject.Find("ObstalceFlag");

                // 设置要渲染的地图
                sr.sprite = Sprite.Create(m_mapTexture, new Rect(0,0,m_mapTexture.width,m_mapTexture.height),Vector2.zero);
                mapRenderer.transform.SetParent(map.transform);
                // 添加要检测射线的碰撞器
                var bc = map.AddComponent<BoxCollider>();
                bc.size = new Vector3(m_mapTexture.width * 0.01f, m_mapTexture.height * 0.01f, 0.2f);
                bc.center = new Vector3(bc.size.x * 0.5f,  bc.size.y * 0.5f, 0f);
                // 设置网格参数并初始化
                m_mapGrid = map.AddComponent<MapGrid>();
                m_mapGrid.SetGridRows(m_rows);
                m_mapGrid.SetGridColumns(m_columns);
                m_mapGrid.RecalculateGridData();
            }
        }

        void ClearMapGameObject()
        {
            GameObject map = GameObject.Find("Map");
            if (map != null)
            {
                GameObject.DestroyImmediate(map);
            }
        }
    }
}
