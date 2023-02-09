using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace Framework
{
    public class GameHomeView : MonoBehaviour
    {
        public Text m_mapDescrible;
        public Sprite[] m_shichenList;
        public Image m_shichenImage;
        public Sprite[] m_dayAndNight;
        public Image m_dayNightImage;
        public Sprite[] m_clockNums;
        public Image[] m_clockTextPartals;
        private float m_timeDelta;
        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            var coordinate = LevelManager.Instance.GetCharacterCurrentCoordinate();
            m_mapDescrible.text = string.Format("{0}[{1}, {2}]", "魔王寨", coordinate.x, coordinate.y);

            UpdateClock();
        }

        void UpdateClock() {
            m_timeDelta += Time.deltaTime;
            if (m_timeDelta > 1) {
                m_timeDelta = 0;

                var now = DateTime.Now;
                var hour = now.Hour.ToString("00");
                UpdateHour(hour);
                var minute = now.Minute.ToString("00");
                UpdateMinute(minute);
                UpdateShichen(now.Minute);
                var second = now.Second.ToString("00");
                UpdateSecond(second);
            }
        }

        void UpdateSecond(string secondStr) {
            var first = secondStr[0];
            UpdateClockUI(m_clockTextPartals[4], first);
            var second = secondStr[1];
            UpdateClockUI(m_clockTextPartals[5], second);
        }

        void UpdateMinute(string minuteStr) {
            var first = minuteStr[0];
            UpdateClockUI(m_clockTextPartals[2], first);
            var second = minuteStr[1];
            UpdateClockUI(m_clockTextPartals[3], second);
        }

        void UpdateHour(string hourStr) {
            var first = hourStr[0];
            UpdateClockUI(m_clockTextPartals[0], first);
            var second = hourStr[1];
            UpdateClockUI(m_clockTextPartals[1], second);
        }

        void UpdateClockUI(Image uiPartal, char timePart) {
            if (uiPartal.sprite.name != timePart.ToString()) {
                uiPartal.sprite = m_clockNums[int.Parse(timePart.ToString())];
                uiPartal.SetNativeSize();
            }
        }

        void UpdateShichen(int minute) {
            minute = minute % 30;
            var idx = (int) (minute/2.5f);
            if (m_shichenImage.sprite.name != m_shichenList[idx].name) {
                m_shichenImage.sprite = m_shichenList[idx];
            }
            idx = idx > 5 ? 1 : 0;
            if (m_dayNightImage.sprite.name != m_dayAndNight[idx].name) {
                m_dayNightImage.sprite = m_dayAndNight[idx];
            }
        }
    }
}
