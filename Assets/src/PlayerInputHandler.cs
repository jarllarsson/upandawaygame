using UnityEngine;
using System.Collections;

public class PlayerInputHandler : MonoBehaviour {
    public PlayerController m_playerController;
    public PlayerHurtScript m_hurtScript;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
        bool canSteer = true;
        if (m_hurtScript) canSteer = !m_hurtScript.isHurting();
        if (canSteer)
        {
            m_playerController.setSteeringInput(new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical")));
            m_playerController.setJumpingInput(Input.GetAxis("Jump"));
        }
        else
        {
            m_playerController.setSteeringInput(new Vector3(0.0f, 0.0f, 0.0f));
            m_playerController.setJumpingInput(0.0f);
        }
	}
}
