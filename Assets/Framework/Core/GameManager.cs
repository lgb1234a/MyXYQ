using UnityEngine;

namespace Framework
{
    public class GameManager : MonoBehaviourSingletonTemplate<GameManager>
    {
        public bool m_inFight = false;
        public bool m_inAutoMoveState = false;
    }
}

