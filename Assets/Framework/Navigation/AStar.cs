using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Framework
{
    public enum EvaluationFunctionType {
        Euclidean,
        Manhattan,
        Diagonal,
    }

    public class Node
    {
        Vector2Int m_position;
        public Vector2Int position => m_position;
        public Node parent;
        
        int m_g;
        public int g {
            get => m_g;
            set {
                m_g = value;
                m_f = m_g + m_h;
            }
        }

        int m_h;
        public int h {
            get => m_h;
            set {
                m_h = value;
                m_f = m_g + m_h;
            }
        }

        int m_f;
        public int f => m_f;

        public Node(Vector2Int pos, Node parent, int g, int h) {
            m_position = pos;
            this.parent = parent;
            m_g = g;
            m_h = h;
            m_f = m_g + m_h;
        }
    }

    public class AStar {
        static int FACTOR = 10;
        static int FACTOR_DIAGONAL = 14;

        // 展示的数据
        IMapInfo m_map;
        // 初始数据
        int[] m_originMap;
        Vector2Int m_mapSize;
        Vector2Int m_player, m_destination;
        EvaluationFunctionType m_evaluationFunctionType;

        Dictionary<Vector2Int, Node> m_openDic = new Dictionary<Vector2Int, Node>();
        Dictionary<Vector2Int, Node> m_closeDic = new Dictionary<Vector2Int, Node>();

        Node m_destinationNode;

        // 其实这里不应该持有map类实例，应该制定一个接口，外部传入任何符合接口标准的实例对象均可，方便后续的扩展
        public void Init(IMapInfo map, Vector2Int mapSize, EvaluationFunctionType type = EvaluationFunctionType.Diagonal) {
            m_map = map;
            m_originMap = map.GetGridValues().Clone() as int[];
            m_mapSize = mapSize;
            m_evaluationFunctionType = type;
        }

        public IEnumerator Start(Vector2Int player, Vector2Int destination) {
            Clear();
            HandleDestination(player, destination);
            AddNodeInOpenQueue(new Node(m_player, null, 0, 0));

            while(m_openDic.Count > 0 && m_destinationNode == null) {
                m_openDic = m_openDic.OrderBy(kv => kv.Value.f).ToDictionary(p => p.Key, o => o.Value);
                Node node = m_openDic.First().Value;
                m_openDic.Remove(node.position);
                OperateNeighborNode(node);
                AddNodeInCloseDic(node);
                yield return null;
            }
            if(m_destinationNode == null)
                Debug.LogError("找不到可用路径");
            else
                ShowPath(m_destinationNode);
        }

        // 如果目标点是障碍物，则取玩家点到目标点连线上离目标点最近的可通过点
        void HandleDestination(Vector2Int player, Vector2Int destination)
        {
            if(m_map.IsCoordinateObstacle(destination))
            {
                // 反向寻路找到离目标点最近的可达点坐标
                m_player = destination;
                m_destination = player;
                AddNodeInOpenQueue(new Node(destination, null, 0, 0));

                while(m_openDic.Count > 0 && m_destinationNode == null) {
                    m_openDic = m_openDic.OrderBy(kv => kv.Value.f).ToDictionary(p => p.Key, o => o.Value);
                    Node node = m_openDic.First().Value;
                    m_openDic.Remove(node.position);
                    if (OperateObstaclbeNeighborNode(node))
                        break;
                    AddNodeInCloseDic(node);
                }
                if(m_destinationNode == null)
                    Debug.LogError("找不到可用目标点");
                else
                {
                    m_destination = m_destinationNode.position;
                    m_player = player;
                }

                Clear();
            }else
            {
                m_player = player;
                m_destination = destination;
            }
        }

        //处理目标点在障碍区域相邻的节点
        bool OperateObstaclbeNeighborNode(Node node)
        {
            for(int i = -1; i < 2; i++) {
                for(int j = -1; j < 2; j++) {
                    if(i == 0 && j == 0)
                        continue;
                    Vector2Int pos = new Vector2Int(node.position.x + i, node.position.y + j);
                    //超出地图范围
                    if(pos.x < 0 || pos.x >= m_mapSize.x || pos.y < 0 || pos.y >= m_mapSize.y)
                        continue;
                    //已经处理过的节点
                    if(m_closeDic.ContainsKey(pos))
                        continue;
                    //将相邻节点加入open中
                    if(i == 0 || j == 0)
                        if (AddObstacleNeighborNodeInQueue(node, pos, FACTOR))
                            return true;
                    else
                        if (AddObstacleNeighborNodeInQueue(node, pos, FACTOR_DIAGONAL))
                            return true;
                }
            }
            return false;
        }

        //处理相邻的节点
        void OperateNeighborNode(Node node) {
            for(int i = -1; i < 2; i++) {
                for(int j = -1; j < 2; j++) {
                    if(i == 0 && j == 0)
                        continue;
                    Vector2Int pos = new Vector2Int(node.position.x + i, node.position.y + j);
                    //超出地图范围
                    if(pos.x < 0 || pos.x >= m_mapSize.x || pos.y < 0 || pos.y >= m_mapSize.y)
                        continue;
                    //已经处理过的节点
                    if(m_closeDic.ContainsKey(pos))
                        continue;
                    //障碍物节点
                    if(m_map.IsCoordinateObstacle(pos))
                        continue;
                    //将相邻节点加入open中
                    if(i == 0 || j == 0)
                        AddNeighborNodeInQueue(node, pos, FACTOR);
                    else
                        AddNeighborNodeInQueue(node, pos, FACTOR_DIAGONAL);
                }
            }
        }

        bool AddObstacleNeighborNodeInQueue(Node parentNode, Vector2Int position, int g)
        {
            int nodeG = parentNode.g + g;
            if(m_openDic.ContainsKey(position)) {
                if(nodeG < m_openDic[position].g) {
                    m_openDic[position].g = nodeG;
                    m_openDic[position].parent = parentNode;
                }
            }
            else {
                Node node = new Node(position, parentNode, nodeG, GetH(position));
                if(m_map.IsCoordinateDefaultValue(position))
                {
                    m_destinationNode = node;
                    return true;
                }
                else
                    AddNodeInOpenQueue(node);
            }
            return false;
        }

        void AddNeighborNodeInQueue(Node parentNode, Vector2Int position, int g) {
            int nodeG = parentNode.g + g;
            if(m_openDic.ContainsKey(position)) {
                if(nodeG < m_openDic[position].g) {
                    m_openDic[position].g = nodeG;
                    m_openDic[position].parent = parentNode;
                }
            }
            else {
                Node node = new Node(position, parentNode, nodeG, GetH(position));
                if(position == m_destination)
                    m_destinationNode = node;
                else
                    AddNodeInOpenQueue(node);
            }
        }

        void AddNodeInOpenQueue(Node node) {
            m_openDic[node.position] = node;
        }

        //加入close中，并更新网格状态
        void AddNodeInCloseDic(Node node) {
            m_closeDic.Add(node.position, node);
        }

        void ShowPath(Node node) {
            while(node != null) {
                m_map.SetGridValue(node.position, MapInfo.Path_Value);
                node = node.parent;
            }
        }

        int GetH(Vector2Int position) {
            if(m_evaluationFunctionType == EvaluationFunctionType.Manhattan)
                return GetManhattanDistance(position);
            else if(m_evaluationFunctionType == EvaluationFunctionType.Diagonal)
                return GetDiagonalDistance(position);
            else
                return Mathf.CeilToInt(GetEuclideanDistance(position));
        }

        int GetDiagonalDistance(Vector2Int position) {
            int x = Mathf.Abs(m_destination.x - position.x);
            int y = Mathf.Abs(m_destination.y - position.y);
            int min = Mathf.Min(x, y);
            return min * FACTOR_DIAGONAL + Mathf.Abs(x - y) * FACTOR;
        }

        int GetManhattanDistance(Vector2Int position) {
            return Mathf.Abs(m_destination.x - position.x) * FACTOR + Mathf.Abs(m_destination.y - position.y) * FACTOR;
        }

        float GetEuclideanDistance(Vector2Int position) {
            return Mathf.Sqrt(Mathf.Pow((m_destination.x - position.x) * FACTOR, 2) + Mathf.Pow((m_destination.y - position.y) * FACTOR, 2));
        }

        public void Clear() {
            for (int i = 0; i < m_originMap.Length; i++)
                m_map.SetGridValue(m_map.GridIndex2Coordinate(i), m_originMap[i]);
            m_openDic.Clear();
            m_closeDic.Clear();

            m_destinationNode = null;
        }
    }
}
