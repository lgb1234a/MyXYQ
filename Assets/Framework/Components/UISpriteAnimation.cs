using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Framework
{
    [RequireComponent(typeof(Image))]
    public class UISpriteAnimation : MonoBehaviour
    {
        private Image m_image;
        private int m_currentFrameIndex = 0;
        private bool m_isPlaying;
        public Sprite[] m_sprites;
        public bool m_autoPlay;
        public bool m_loop;
        public float m_fps = 1;
        private float m_deltaTime = 0;
        public int m_frameCount
        {
            get
            {
                return m_sprites.Length;
            }
        }

        void Awake() 
        {
            m_image = GetComponent<Image>();
        }

        // Start is called before the first frame update
        void Start()
        {
            if (m_autoPlay) {
                m_isPlaying = true;
            }else {
                m_isPlaying = false;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (!m_isPlaying || 0 == m_frameCount) {
                return;
            }

            m_deltaTime = m_deltaTime + Time.deltaTime;
            if (m_deltaTime > 1/m_fps) {
                m_deltaTime = 0;
                UpdateSprite();
            }
            
        }

        void UpdateSprite() {
            m_image.sprite = m_sprites[m_currentFrameIndex];
            m_image.SetNativeSize();
            m_currentFrameIndex++;
            m_currentFrameIndex = m_currentFrameIndex%m_frameCount;
        }
    }
}
