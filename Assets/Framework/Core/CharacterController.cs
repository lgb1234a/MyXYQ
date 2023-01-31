using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Framework
{
    public class CharacterController : MonoBehaviour
    {
        public float m_speed = 1.0f;
        public Animator m_animator;
        private Queue<Vector2> m_pathLocations = new Queue<Vector2>();
        private Coroutine m_currentMove;
        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(HandleCharacterMove());
        }

        // Update is called once per frame
        void Update()
        {
            HandleMouseClicked();
        }

        void HandleMouseClicked() {
            if (!GameManager.Instance.m_inFight) {
                if (Input.GetMouseButtonDown(0)) {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hitInfo;
                    if (Physics.Raycast(ray, out hitInfo, 200f)) {
                        var paths = LevelManager.Instance.Navigate(transform.position, hitInfo.point);
                        StopMoveCoroutine();
                        EnqueuePathLocations(paths);
                    }
                }
            }
        }

        void StopMoveCoroutine() {
            if (m_currentMove != null) {
                StopCoroutine(m_currentMove);
                m_currentMove = null;
            }
        }

        void EnqueuePathLocations(IEnumerable<Vector2> locations)
        {
            m_pathLocations.Clear();
            foreach(var location in locations) {
                m_pathLocations.Enqueue(location);
            }
        }

        IEnumerator HandleCharacterMove() {
            while (true)
            {
                if (GameManager.Instance.m_inAutoMoveState) {
                    yield return HandleCharacterMoveFollowMousePosition();
                }else if (m_pathLocations.Count > 0) {
                    yield return HandleCharacterNavigateMove();
                }else {
                    yield return new WaitForEndOfFrame();
                }
            }
        }

        IEnumerator HandleCharacterMoveFollowMousePosition() {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, 200f)) {
                if (LevelManager.Instance.CanMove(transform, hitInfo.point)) {
                    var movement = hitInfo.point - transform.position;
                    TranslateTransform(movement);
                }
            }
            yield return new WaitForEndOfFrame();
        }

        IEnumerator HandleCharacterNavigateMove() {
            var nextLocation = m_pathLocations.Dequeue();
            var distance = Vector3.Distance(transform.position, new Vector3(nextLocation.x, nextLocation.y, 0));
            var movement = new Vector3(nextLocation.x, nextLocation.y, 0) - transform.position;
            m_currentMove = StartCoroutine(Move(movement));
            var moveInterval = distance/m_speed;
            yield return new WaitForSeconds(moveInterval);
            StopMoveCoroutine();
            if (m_pathLocations.Count == 0) {
                // 上面计算的moveInterval可能无法跑满最后一帧就结束了，所以需要单独处理这部分偏移量
                m_currentMove = StartCoroutine(FixEndLocationDeviation(nextLocation));
            }
        }

        IEnumerator Move(Vector3 movement) {
            while(true) {
                TranslateTransform(movement);
                yield return new WaitForEndOfFrame();
            }
        }

        IEnumerator FixEndLocationDeviation(Vector2 destination) {
            while(!IsArrived(destination)) {
                var movement = new Vector3(destination.x, destination.y, 0) - transform.position;
                TranslateTransform(movement, false);
                yield return new WaitForEndOfFrame();
            }
            TriggerIdleAnimator();
        }

        bool IsArrived(Vector2 destination) {
            var gridSize = LevelManager.Instance.GetGridSize();
            var minOffsetX = gridSize.x*0.1f;
            var minOffsetY = gridSize.y*0.1f;
            return Math.Abs(destination.x - transform.position.x) < minOffsetX 
            && Math.Abs(destination.y - transform.position.y) < minOffsetY;
        }

        void TranslateTransform(Vector3 movement, bool updateRunAnimator = true) {
            transform.Translate(movement.normalized * m_speed * Time.deltaTime);
            if (updateRunAnimator)
                TriggerRunAnimator(movement);
        }

        void TriggerRunAnimator(Vector3 movement) {
            m_animator.SetBool("IsIdle", false);
            m_animator.SetFloat("X", movement.normalized.x);
            m_animator.SetFloat("Y", movement.normalized.y);
        }

        void TriggerIdleAnimator() {
            m_animator.SetBool("IsIdle", true);
        }
    }
}
