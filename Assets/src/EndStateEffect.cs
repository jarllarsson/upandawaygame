using UnityEngine;
using System.Collections;

public class EndStateEffect : MonoBehaviour {
    public MonoBehaviour[] m_scriptsToDisableOnTrigger;
    public Renderer m_rendererToRun;
    public bool m_trigActivate = false;
    private bool m_activated = false;
    public int m_sceneToRestart;
    public bool m_restartScene = true;
    public bool m_pauseOnTrig = true;
    float ownTime = 0;
    public bool m_animate = true;

    private float m_alpha = 1.0f;
    public void activate()
    {
        foreach (MonoBehaviour script in m_scriptsToDisableOnTrigger)
            script.enabled = false;
        m_activated=true;
        if (m_pauseOnTrig) Time.timeScale = 0.0f;
    }

    bool m_jumpBtnIsDown=false;

	// Use this for initialization
	void Start () 
    {
        ownTime = Time.realtimeSinceStartup;
        Color col = m_rendererToRun.material.GetColor("_TintColor");
        m_rendererToRun.material.SetColor("_TintColor",new Color(col.r,col.g,col.b,0.0f));
	}

	// Update is called once per frame
	void Update () 
    {
        if (m_trigActivate)
        {
            activate();
            m_trigActivate = false;
        }
	    if (m_activated)
        {
            Color col = m_rendererToRun.material.GetColor("_TintColor");
            m_rendererToRun.material.SetColor("_TintColor", Color.Lerp(col, new Color(1.0f, 1.0f, 1.0f, 1.0f), 0.01f*ownTime));
            Debug.Log("!");
            if (col.a > 0.5f && Input.GetAxisRaw("Jump") > 0.0f)
            {
                Time.timeScale = 1.0f;
                if (m_restartScene)
                    Application.LoadLevel(m_sceneToRestart);
                else
                {
                    foreach (MonoBehaviour script in m_scriptsToDisableOnTrigger)
                        script.enabled = true;
                    Destroy(gameObject);
                }
            }
        }

        if (Input.GetAxis("Jump") > 0.0f)
            m_jumpBtnIsDown = true;
        else
            m_jumpBtnIsDown = false;
        ownTime = Time.realtimeSinceStartup - ownTime;
	}
}
