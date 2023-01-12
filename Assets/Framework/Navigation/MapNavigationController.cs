using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Framework
{
    public class MapNavigationController : MonoBehaviour
    {
        public GameObject m_pathFlag;
        public GameObject m_obstacleFlag;
        public Button m_setPlayerLocationBtn;
        public Button m_setDestinationBtn;
        public Button m_aStarButton;
        public EvaluationFunctionType m_evaluationFunctionType = EvaluationFunctionType.Manhattan;

        AStar m_aStar;
        MapInfo m_mapInfo;
        IEnumerator m_aStarProcess;

        bool m_inSettingPlayerLocation;
        bool m_inSettingDestinationLocation;
        Vector2Int m_playerLocation;
        Vector2Int m_destinationLocation;

        void Start()
        {
            m_setPlayerLocationBtn.onClick.AddListener(OnClickedPlayerLocationBtn);
            m_setDestinationBtn.onClick.AddListener(OnClickedDestinationBtn);
            m_aStarButton.onClick.AddListener(OnClickedAStarBtn);

            m_aStar = new AStar();
            string path = Environment.GetCurrentEnvironment().GetMapJsonDataPath(name);
            m_mapInfo = MapInfo.ImportFromFile(path);

            var mapSize = new Vector2Int(m_mapInfo.m_rows, m_mapInfo.m_columns);
            m_aStar.Init(m_mapInfo.m_gridValues, mapSize, m_evaluationFunctionType);
            ShowObstacles();
        }

        void OnClickedPlayerLocationBtn()
        {
            m_inSettingPlayerLocation = true;
        }

        void OnClickedDestinationBtn()
        {
            m_inSettingDestinationLocation = true;
        }

        void OnClickedAStarBtn()
        {
            // 先清除上一次生成的路径
            ClearPath();
            m_aStarProcess = m_aStar.Start(m_playerLocation, m_destinationLocation);
            while(m_aStarProcess.MoveNext())
                ;
            ShowPath();
        }

        GameObject InstantiateFlag(GameObject flag, Vector3 position)
        {
            var go = UnityEngine.Object.Instantiate(flag, position, Quaternion.identity);
            go.SetActive(true);
            return go;
        }

        void InstantiatePathFlag(Vector2 position)
        {
            var go = InstantiateFlag(m_pathFlag, new Vector3(position.x, position.y));
            var coordinate = m_mapInfo.WorldPosition2GridCoordinate(position);
            go.name = $"{coordinate.x}_{coordinate.y}";
        }

        void InstantiateObstacleFlag(Vector2 position)
        {
            InstantiateFlag(m_obstacleFlag, new Vector3(position.x, position.y));
        }

        void ShowPath()
        {
            int i = 0;
            foreach(var value in m_mapInfo.m_gridValues)
            {
                if (value == MapInfo.Path_Value)
                    InstantiatePathFlag(m_mapInfo.GridIndex2WorldPosition(i));
                i++;
            }
        }

        void ClearPath()
        {
            int count = m_mapInfo.m_gridValues.Length;
            for(int i = 0; i < count; i++)
            {
                if (m_mapInfo.m_gridValues[i] == MapInfo.Path_Value)
                {
                    m_mapInfo.m_gridValues[i] = 0;
                    var coordinate = m_mapInfo.GridIndex2Coordinate(i);
                    var go = GameObject.Find($"{coordinate.x}_{coordinate.y}");
                    UnityEngine.Object.DestroyImmediate(go);
                }
            }
        }

        void ShowObstacles()
        {
            int i = 0;
            foreach(var value in m_mapInfo.m_gridValues)
            {
                if (value == MapInfo.Obstacle_Value)
                    InstantiateObstacleFlag(m_mapInfo.GridIndex2WorldPosition(i));
                i++;
            }
        }

        void Update()
        {
            if (m_inSettingPlayerLocation || m_inSettingDestinationLocation)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hitInfo;
                    if (Physics.Raycast(ray, out hitInfo, 200f))
                    {
                        int tx = (int)(hitInfo.point.x / m_mapInfo.m_gridWidth);
                        int ty = (int)(hitInfo.point.y / m_mapInfo.m_gridHeight);
                        
                        if (m_inSettingPlayerLocation)
                        {
                            m_playerLocation = new Vector2Int(tx, ty);
                            m_mapInfo.SetGridValue(m_playerLocation, MapInfo.Path_Value);
                            InstantiatePathFlag(new Vector2(hitInfo.point.x, hitInfo.point.y));
                        }
                        else if (m_inSettingDestinationLocation)
                        {
                            m_destinationLocation = new Vector2Int(tx, ty);
                            m_mapInfo.SetGridValue(m_destinationLocation, MapInfo.Path_Value);
                            InstantiatePathFlag(new Vector2(hitInfo.point.x, hitInfo.point.y));
                        }
                    }
                    m_inSettingPlayerLocation = false;
                    m_inSettingDestinationLocation = false;
                }
            }
        }
    }
}
