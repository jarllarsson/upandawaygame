using UnityEngine;
using System.Collections;

public class SquishStretch : MonoBehaviour 
{
    public Transform m_transform;
    public float m_transitionTime;
    private Vector3 m_currentGoal=Vector3.zero;
    private Vector3 m_original;
    private bool m_isNew = false;
    private bool m_isAnimating = false;
    private bool m_loopBack = false;
    private float m_loopBackTime = 0.0f;
    private bool m_startSet = false;
    private Vector3 m_startScale;
    private Vector3 m_velocity;
	// Use this for initialization
	void Start () 
    {
        m_original = m_transform.localScale;
	}
	
	// Update is called once per frame
	void Update () 
    {
	    if (m_isAnimating)
        {
            // animate
            m_transform.localScale = Vector3.SmoothDamp(m_transform.localScale, m_currentGoal, ref m_velocity, m_transitionTime);
            // anim end check
            if (Vector3.SqrMagnitude(m_transform.localScale - m_currentGoal) < 0.001f)
            {
                m_isAnimating = false;
                if (m_loopBack)
                {
                    setStart(m_transform.localScale);
                    setGoal(m_original, m_loopBackTime);
                }
            }
        }
	}

    public void setStart(Vector3 p_start)
    {
        m_startSet = true;
        m_startScale = p_start;
    }

    public void setGoal(Vector3 p_goal, float p_time, bool p_loopBack=false, float p_loopBackTime=0.0f)
    {       
        if (Vector3.SqrMagnitude(p_goal - m_currentGoal) > 0.001f) // if new goal
        {
            if (m_startSet)
                m_transform.localScale = m_startScale;
            else
                m_transform.localScale = m_original;
            m_currentGoal = p_goal;
            m_transitionTime = p_time;
            m_isAnimating = true;
            m_isNew = true;
            m_startSet = false;
            // Allow for loopback to original automatically:
            m_loopBack = p_loopBack;
            m_loopBackTime = p_loopBackTime;
        }
    }
}
