using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;

namespace Framework
{
    public class CharacterController : MonoBehaviour
    {
        public float m_speed = 1.0f;
        public Animator m_animator;
        private Queue<Vector2> m_pathLocations = new Queue<Vector2>();
        private Vector2 m_nextMoveLocation = Vector2.left;
        private int m_fingerID = -1;

        private void Awake()
        {
        #if !UNITY_EDITOR
            m_fingerID = 0;
        #endif
        }
        // Update is called once per frame
        void Update()
        {
            HandleMouseClicked();
            HandleCharacterMove();
        }

        void HandleMouseClicked() {
            if (GameManager.Instance.m_inFight)
                return;
            if (EventSystem.current.IsPointerOverGameObject(m_fingerID))
                return;

            if (Input.GetMouseButtonDown(0)) {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hitInfo;
                if (Physics.Raycast(ray, out hitInfo, 200f)) {
                    var paths = LevelManager.Instance.Navigate(transform.position, hitInfo.point);
                    TriggerIdleAnimator();
                    EnqueuePathLocations(paths);
                }
            }
        }

        void EnqueuePathLocations(IEnumerable<Vector2> locations)
        {
            m_pathLocations.Clear();
            foreach(var location in locations) {
                m_pathLocations.Enqueue(location);
            }
        }

        void HandleCharacterMove() {
            if (GameManager.Instance.m_inAutoMoveState) {
                HandleCharacterMoveFollowMousePosition();
            }else {
                HandleCharacterNavigateMove();
            }
        }

        void HandleCharacterMoveFollowMousePosition() {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, 200f)) {
                var movement = hitInfo.point - transform.position;
                TriggerRunAnimator(movement);

                if (LevelManager.Instance.IsCoordinateObstacle(hitInfo.point)) {
                    if (LevelManager.Instance.CanMove(transform, hitInfo.point)) {
                        TranslateTransform(movement);
                    }
                } 
                else{
                    var paths = LevelManager.Instance.Navigate(transform.position, hitInfo.point);
                    if (paths.Count() > 0) {
                        var nextPath = paths.First();
                        movement = new Vector3(nextPath.x, nextPath.y, 0) - transform.position;
                        TranslateTransform(movement);
                    }
                }
            }
        }

        void HandleCharacterNavigateMove() {
            if (m_pathLocations.Count > 0) {
                if (IsNextMoveLocationInvalid()) {
                    m_nextMoveLocation = m_pathLocations.Dequeue();
                }
                else if (IsArrived(m_nextMoveLocation)) {
                    m_nextMoveLocation = m_pathLocations.Dequeue();
                }
            }
            if (IsNextMoveLocationInvalid())
                return;
            
            if (IsArrived(m_nextMoveLocation)) {
                InvalidNextMoveLocation();
                TriggerIdleAnimator();
                return;
            }

            var distance = Vector3.Distance(transform.position, new Vector3(m_nextMoveLocation.x, m_nextMoveLocation.y, 0));
            var movement = new Vector3(m_nextMoveLocation.x, m_nextMoveLocation.y, 0) - transform.position;
            TranslateTransform(movement);
        }

        
        bool IsNextMoveLocationInvalid() {
            return m_nextMoveLocation == Vector2.left;
        }

        void InvalidNextMoveLocation() {
            m_nextMoveLocation = Vector2.left;
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
