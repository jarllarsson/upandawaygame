using UnityEngine;
using System.Collections;

public class Shaker : MonoBehaviour 
{
    public Transform m_transform;
    public Vector3 m_shakeAmplitude;
    public Vector3 m_frequencyOffset;
    public Vector3 m_frequencyPower;
    public float m_time;
    private float m_timeTicker = 0.0f;
    public float m_easeInAmplitudeTime;
    private Vector3 m_easingVelocity;
    private bool m_isShaking;
    private Vector3 m_originalLPos;
	// Use this for initialization
	void Start () 
    {
        m_originalLPos = m_transform.localPosition;
	}
	
	// Update is called once per frame
	void Update () 
    {
	    if (m_isShaking)
        {
            m_transform.localPosition = m_originalLPos + new Vector3(m_shakeAmplitude.x * Mathf.Sin(m_timeTicker * m_frequencyPower.x + m_frequencyOffset.x),
                m_shakeAmplitude.y * Mathf.Sin(m_timeTicker * m_frequencyPower.y + m_frequencyOffset.y),
                m_shakeAmplitude.z * Mathf.Sin(m_timeTicker * m_frequencyPower.z + m_frequencyOffset.z));
            m_shakeAmplitude = Vector3.SmoothDamp(m_shakeAmplitude, Vector3.zero, ref m_easingVelocity,m_easeInAmplitudeTime);
            m_timeTicker += Time.deltaTime;
            if (m_timeTicker >= m_time)
            {
                m_isShaking = false;
                m_transform.localPosition = m_originalLPos;
            }
        }
	}

    void shake(Vector3 p_power, float p_time, float p_easingTime)
    {
        m_transform.localPosition = m_originalLPos;
        m_shakeAmplitude = p_power;
        m_time = p_time;
        m_timeTicker = 0.0f;
        m_easeInAmplitudeTime = p_easingTime;
        m_isShaking = true;
    }
}
