using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FrameDebugger : MonoBehaviour
{
    const float m_fpsMeasurePeriod = 1f;
    private int m_fpsAccumulator = 0;
    private float m_fpsNextPeriod = 0;
    private int m_currentFps;
    const string m_display = "{0} fps";
    public Text m_text;
    public int m_fps;
    void Awake()
    {
        Application.targetFrameRate = m_fps;
        m_fpsNextPeriod = Time.realtimeSinceStartup + m_fpsMeasurePeriod;
        DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {
        m_fpsAccumulator++;
        if(Time.realtimeSinceStartup > m_fpsNextPeriod) {
            m_currentFps = (int)(m_fpsAccumulator/m_fpsMeasurePeriod);
            m_fpsAccumulator = 0;
            m_fpsNextPeriod += m_fpsMeasurePeriod;
            m_text.text = string.Format(m_display, m_currentFps);
        }
    }
}
