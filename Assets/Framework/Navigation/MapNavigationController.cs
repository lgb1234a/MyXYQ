using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

namespace Framework
{
    
    public struct Int2
    {
        public int x;
        public int y;

        public Int2(int x, int y) {
            this.x = x;
            this.y = y;
        }

        public int GetMapGridIndex(int rows)
        {
            return this.x * rows + this.y;
        }

        public override string ToString() {
            return $"x:{x.ToString()}   y:{y.ToString()}";
        }

        public override int GetHashCode() {
            return x ^ (y * 256);
        }

        public override bool Equals(object obj) {
            if(obj.GetType() != typeof(Int2))
                return false;
            Int2 int2 = (Int2)obj;
            return x == int2.x && y == int2.y;
        }

        public static bool operator ==(Int2 a, Int2 b) {
            return a.Equals(b);
        }

        public static bool operator !=(Int2 a, Int2 b) {
            return !a.Equals(b);
        }
    }

    public class MapNavigationController : MonoBehaviour
    {
        public GameObject m_flag;
        public GameObject m_abstacleFlag;
        public Button m_setPlayerLocationBtn;
        public Button m_setDestinationBtn;
        public Button m_aStarButton;
        public bool m_isStepOneByOne;
        public EvaluationFunctionType m_evaluationFunctionType = EvaluationFunctionType.Manhattan;

        AStar m_aStar;
        MapInfo m_mapInfo;
        IEnumerator m_aStarProcess;

        bool m_inSettingPlayerLocation;
        bool m_inSettingDestinationLocation;
        Int2 m_playerLocation;
        Int2 m_destinationLocation;

        List<GameObject> m_flags = new List<GameObject>();

        void Start()
        {
            m_setPlayerLocationBtn.onClick.AddListener(OnClickedPlayerLocationBtn);
            m_setDestinationBtn.onClick.AddListener(OnClickedDestinationBtn);
            m_aStarButton.onClick.AddListener(OnClickedAStarBtn);

            m_aStar = new AStar();
            string path = "Assets/Resources/Text/Map/MWZ.txt";
            m_mapInfo = MapInfo.ImportFromFile(path);
            ShowAbstacles();
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
            if(!m_aStar.isInit) {
                var mapSize = new Int2(m_mapInfo.m_rows, m_mapInfo.m_columns);
                m_aStar.Init(m_mapInfo.m_gridValues, mapSize, m_playerLocation, m_destinationLocation, m_evaluationFunctionType);
                m_aStarProcess = m_aStar.Start();
            }
            if(m_isStepOneByOne) {
                if(!m_aStarProcess.MoveNext()) {
                    Debug.Log("寻路结束");
                }
            }
            else {
                while(m_aStarProcess.MoveNext())
                    ;
                ShowPath();
                
            }
        }

        void InstantiateFlag(GameObject flag, Vector3 position)
        {
            var go = UnityEngine.Object.Instantiate(flag, position, Quaternion.identity);
            go.SetActive(true);
            m_flags.Add(go);
        }

        void InstantiatePathFlag(float x, float y)
        {
            InstantiateFlag(m_flag, new Vector3(x, y));
        }

        void InstantiateAbstacleFlag(float x, float y)
        {
            InstantiateFlag(m_abstacleFlag, new Vector3(x, y));
        }

        void ShowPath()
        {
            int i = 0;
            foreach(var value in m_mapInfo.m_gridValues)
            {
                if (value == -1)
                {
                    int x = i / m_mapInfo.m_rows;
                    int y = i % m_mapInfo.m_columns;
                    float positionX = x * m_mapInfo.m_gridWidth;
                    float positionY = y * m_mapInfo.m_gridHeight;
                    InstantiatePathFlag(positionX, positionY);
                }
                i++;
            }
        }

        void ShowAbstacles()
        {
            int i = 0;
            foreach(var value in m_mapInfo.m_gridValues)
            {
                if (value == 1)
                {
                    int x = i / m_mapInfo.m_rows;
                    int y = i % m_mapInfo.m_columns;
                    float positionX = x * m_mapInfo.m_gridWidth;
                    float positionY = y * m_mapInfo.m_gridHeight;
                    InstantiateAbstacleFlag(positionX, positionY);
                }
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
                            m_playerLocation = new Int2(tx, ty);
                            InstantiatePathFlag(hitInfo.point.x, hitInfo.point.y);
                        }
                        else if (m_inSettingDestinationLocation)
                        {
                            m_destinationLocation = new Int2(tx, ty);
                            InstantiatePathFlag(hitInfo.point.x, hitInfo.point.y);
                        }
                    }
                    m_inSettingPlayerLocation = false;
                    m_inSettingDestinationLocation = false;
                }
            }
        }
    }
}
