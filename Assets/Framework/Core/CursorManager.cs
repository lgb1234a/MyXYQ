using UnityEngine;
using System.Collections;

namespace Framework
{
    public class CursorManager : MonoBehaviour
    {
        public Texture2D[] m_cursors;
        public float cursorAnimInterval = 0.1f;
        private int cursorIndex = 0;

        public UnityEngine.Object m_cursorEffectPrefab;
        public Transform m_cursorEffectParent;
        public float m_mouseDownInterval = 0;

        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(UpdateCursorTexture());
        }

        // Update is called once per frame
        void Update()
        {
            if (!GameManager.Instance.m_inFight)
                UpdateCursorClickEffect();
        }

        void UpdateCursorClickEffect() {
            if(Input.GetMouseButtonDown(0)) {
                GameManager.Instance.m_inAutoMoveState = false;
                StartCoroutine(CreateMouseTrackEffect());
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
                StartCoroutine(CreateMouseTrackEffect());
            }
        }

        IEnumerator CreateMouseTrackEffect() {
            var worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var position = m_cursorEffectParent.InverseTransformPoint(worldPos);
            position.z = -0.9f;
            var effectGO = Instantiate(m_cursorEffectPrefab, position, Quaternion.identity, m_cursorEffectParent) as GameObject;
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
        }

        IEnumerator UpdateCursorTexture() {
            while(true) {
                cursorIndex = cursorIndex%m_cursors.Length;
                Cursor.SetCursor(m_cursors[cursorIndex], Vector2.zero, CursorMode.Auto);
                cursorIndex++;
                yield return new WaitForSeconds(cursorAnimInterval);
            }
        }
    }
}
