using UnityEngine;
using System.Collections;

public class PlayerInputHandler : MonoBehaviour {
    public PlayerController m_playerController;
    public PlayerHurtScript m_hurtScript;
    public float m_inputTime = 0.0f;
    public float m_inputTimeSpd = 1.2f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () 
    {
        bool canSteer = true;
        if (m_hurtScript) canSteer = !m_hurtScript.isHurting();
        if (canSteer)
        {
            Vector3 inputVec=new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
            m_playerController.setSteeringInput(inputVec);
            m_playerController.setJumpingInput(Input.GetAxis("Jump"));

            float inputMagnitude = inputVec.normalized.magnitude;
            Debug.Log(inputMagnitude);
            if (inputMagnitude > 0.9f)
            {
                m_inputTime += inputMagnitude * m_inputTimeSpd * Time.deltaTime;
            }
            else
                m_inputTime = 0.0f;
        }
        else
        {
            m_playerController.setSteeringInput(new Vector3(0.0f, 0.0f, 0.0f));
            m_playerController.setJumpingInput(0.0f);
            m_inputTime = 0.0f;
        }
        m_playerController.m_inutTimeSpdMultiplier = Mathf.Clamp(m_inputTime, 0.1f, 1.0f);
	}
}
