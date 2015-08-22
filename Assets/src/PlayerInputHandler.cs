using UnityEngine;
using System.Collections;

public class PlayerInputHandler : MonoBehaviour {
    public PlayerController m_playerController;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
        m_playerController.setSteeringInput(new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical")));
        m_playerController.setJumpingInput(Input.GetAxis("Jump"));
	}
}
