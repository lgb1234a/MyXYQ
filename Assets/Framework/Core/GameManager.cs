using UnityEngine;

namespace Framework
{
    public class GameManager : MonoBehaviourSingletonTemplate<GameManager>
    {
        public bool m_inFight = false;
        public bool m_inAutoMoveState = false;
        public Cinemachine.CinemachineConfiner m_cc;

        void Start() {
            var map = GameObject.Find("Map");
            m_cc.m_BoundingShape2D = map.GetComponent<PolygonCollider2D>();
        }
    }
}

