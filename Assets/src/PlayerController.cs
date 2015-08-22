using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour 
{
    public Rigidbody m_rbody;
    public float m_maxSpeed = 1.0f;
    public Transform m_groundCheckPoint;
    public float m_groundCheckRadius = 0.3f;
    public int m_groundLayer;
    private bool m_isOnGround;

	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    void FixedUpdate()
    {
        // Check if we're on ground
        groundCheck();
        //
        if (m_isOnGround)
        {
            // Basic steering
            Vector3 steerDir = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
            if (steerDir.magnitude > 1.0f) steerDir.Normalize();
            m_rbody.velocity = new Vector3(m_maxSpeed * steerDir.x, m_rbody.velocity.y, m_maxSpeed * steerDir.z);
        }
    }

    void groundCheck()
    {
        Collider[] hits = Physics.OverlapSphere(m_groundCheckPoint.position, m_groundCheckRadius, m_groundLayer);
        if (hits.Length > 0)
            m_isOnGround = true;
        else
            m_isOnGround = false;
    }
}
