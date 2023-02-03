using UnityEngine;

namespace Framework
{
    public class CursorManager : MonoBehaviour
    {
        public Texture2D[] m_cursors;
        private int m_cursorIndex = 0;
        private int m_cursorCounter = 0;
        const int __CursorAnimFramesInterval = 6;

        public UnityEngine.Object m_cursorEffectPrefab;
        public Transform m_cursorEffectParent;
        public float m_mouseDownInterval = 0;

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            if (!GameManager.Instance.m_inFight)
                UpdateCursorClickEffect();
            
            UpdateCursorCounter();
            if (m_cursorCounter%__CursorAnimFramesInterval == 0)
                UpdateCursorTexture();
        }

        void UpdateCursorCounter() {
            m_cursorCounter++;
            if (m_cursorCounter > __CursorAnimFramesInterval) 
                m_cursorCounter = 0;
        }

        void UpdateCursorClickEffect() {
            if(Input.GetMouseButtonDown(0)) {
                GameManager.Instance.m_inAutoMoveState = false;
                CreateMouseTrackEffect();
            }
            else if (Input.GetMouseButton(0)) {
                m_mouseDownInterval += Time.deltaTime;
                if (m_mouseDownInterval > 2f) {
                    GameManager.Instance.m_inAutoMoveState = true;
                }
            }
            else if (Input.GetMouseButtonUp(0)) {
                m_mouseDownInterval = 0;
            }

            if (GameManager.Instance.m_inAutoMoveState) {
                if (Time.frameCount%2 == 0) {
                    CreateMouseTrackEffect();
                }
            }
        }

        void CreateMouseTrackEffect() {
            var worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var position = m_cursorEffectParent.InverseTransformPoint(worldPos);
            position.z = -0.9f;
            var effectGO = Instantiate(m_cursorEffectPrefab, position, Quaternion.identity, m_cursorEffectParent) as GameObject;
        }

        void UpdateCursorTexture() {
            m_cursorIndex = m_cursorIndex%m_cursors.Length;
            Cursor.SetCursor(m_cursors[m_cursorIndex], Vector2.zero, CursorMode.Auto);
            m_cursorIndex++;
        }
    }
}
