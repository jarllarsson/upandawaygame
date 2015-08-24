using UnityEngine;
using System.Collections;

public class PlayerAnimatorHandler : MonoBehaviour 
{
    public PlayerController m_playerController;
    public Animator m_anim;
    public bool m_useIsJumping = true;
    public float m_maxSpeed;
    int m_jumpHash = Animator.StringToHash("IsJumping");
    int m_speedHash = Animator.StringToHash("Speed");
    int m_yVelocityHash = Animator.StringToHash("yVel");
	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (m_useIsJumping) m_anim.SetBool(m_jumpHash, m_playerController.isJumping());
        m_anim.SetFloat(m_speedHash, Mathf.Min(m_maxSpeed,m_playerController.getPlaneMovementSpeed()));
        m_anim.SetFloat(m_yVelocityHash, m_playerController.getVelocity().y);
	}
}
