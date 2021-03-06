﻿using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour 
{
    public Rigidbody m_rbody;
    public float m_maxSpeed = 1.0f;
    public float m_acceleration = 1.0f;
    public float m_velocityFallof = 0.8f;
    public Transform m_groundCheckPoint;
    public float m_groundCheckRadius = 0.3f;
    public LayerMask m_groundLayer;
    private bool m_isOnGround;
    private Vector3 m_inputDir;
    private Vector3 m_steerDir;
    private Vector3 m_velocity;
    private Vector3 m_moveDir;


    public Transform m_playerSteerFacing;
    public Transform m_pointOfViewRunning;
    public Transform m_pointOfViewJumping;
    private bool m_isSteering=false;

    public float m_jumpPower = 1.0f;
    public float m_doubleJumpPower = 2.0f;
    public float m_tripleJumpPower = 3.0f;
    public Vector3 m_extraJumpMovementForce;


    public float m_camOffsetUpOnJump = 5.0f;
    public Transform m_camPivotToOffsetOnJump;
    private Vector3 m_camPivotOriginalLPos;
    public float m_camLookatOffsetUpOnJump = 5.0f;
    public Transform m_camLookatToOffsetOnJump;
    private Vector3 m_camLookatOriginalLPos;
    public Transform m_transformToRotateOnJump;

    public SquishStretch m_jumpSquisher;

    // Jump vars
    private int m_jumpCount=0; // current sequential jump
    private bool m_isJumping=false; // am I jumping?
    private float m_jumpCountCoolDown = 0.0f;
    public float m_doubleJumpCountCoolDownLim = 0.2f;
    public float m_holdJumpDiminishForce = 0.7f; // amount to diminish hold-jump power each tick
    private float m_currentJumpForce = 0.0f;
    private bool m_jumpButtonDown = false;
    private bool m_jumpButtonReleased = true;
    private float m_jumpInputVal;



    public AudioSource m_audioSource;
    public AudioClip m_jumpSnd1,m_jumpSnd2,m_jumpSnd3;
	// Use this for initialization
	void Start () 
    {
        if (m_camPivotToOffsetOnJump) m_camPivotOriginalLPos = m_camPivotToOffsetOnJump.localPosition;
        if (m_camLookatToOffsetOnJump) m_camLookatOriginalLPos = m_camLookatToOffsetOnJump.localPosition;
	}
	
	// Update is called once per frame
	void Update () 
    {


	}

    public void setSteeringInput(Vector3 p_inputDir)
    {
        if (p_inputDir.magnitude>0.2f)
            m_inputDir = p_inputDir;
        else
            m_inputDir = Vector3.zero;
    }

    public void setJumpingInput(float p_jumpVal)
    {
        m_jumpInputVal = p_jumpVal;
    }

    void FixedUpdate()
    {
        if (!m_jumpButtonDown)
        {
            m_jumpButtonReleased = true;
            resetJumpForce();
        }
        else
        {
            m_jumpButtonReleased = false;
        }
        m_jumpButtonDown = m_jumpInputVal>0.0f;


        // Check if we're on ground, only when we're still(on y-axis) or falling down
        if (m_rbody.velocity.y <= 0.0f) groundCheck();
        // Reset jump if we're falling to ground
        if (m_isOnGround && m_rbody.velocity.y <= 0.0f)
        {
            m_isJumping = false;
        }
        //
        if (m_isOnGround && !m_isJumping)
        {
            m_isJumping = false;
            steer(1.0f);
            coolDownDoubleJumpCount(); // Count the jumps
            if (m_transformToRotateOnJump!=null) m_transformToRotateOnJump.localEulerAngles = Vector3.zero;
            // Reset cam pivot
            if (m_camPivotToOffsetOnJump)
                m_camPivotToOffsetOnJump.localPosition = Vector3.Lerp(m_camPivotToOffsetOnJump.localPosition, m_camPivotOriginalLPos, Time.deltaTime * 1.0f);
            // Reset cam lookat
            if (m_camLookatToOffsetOnJump)
                m_camLookatToOffsetOnJump.localPosition = Vector3.Lerp(m_camLookatToOffsetOnJump.localPosition, m_camLookatOriginalLPos, Time.deltaTime * 1.0f);
            // Try jump
            if (wasJumpBtnReleased())
            {
                triggerJump();
            }
        }
        if (m_isJumping)
        {
            steer(0.8f);
            // Offset cam pivot
            if (m_camPivotToOffsetOnJump)
                m_camPivotToOffsetOnJump.localPosition = Vector3.Lerp(m_camPivotToOffsetOnJump.localPosition, m_camPivotOriginalLPos+Vector3.up*m_camOffsetUpOnJump, Time.deltaTime * 3.0f);
            // Offset cam lookat
            if (m_camLookatToOffsetOnJump)
                m_camLookatToOffsetOnJump.localPosition = Vector3.Lerp(m_camLookatToOffsetOnJump.localPosition, m_camLookatOriginalLPos + Vector3.up * m_camLookatOffsetUpOnJump, Time.deltaTime * 3.0f);
            // While jumping
            m_jumpCountCoolDown = 0.0f; // reset jump counter cooldown, start afresh
            doJump(); // having this here allows the user to hold down button for a while for higher jump!
            // rotate
            if (m_transformToRotateOnJump != null)
            {
                switch (m_jumpCount)
                {
                    case 2:
                        m_transformToRotateOnJump.localRotation *= Quaternion.Euler(new Vector3(0.0f, 10.0f, 0.0f)); break;
                    case 3:
                        m_transformToRotateOnJump.localRotation *= Quaternion.Euler(new Vector3(-15.0f, 0.0f, 0.0f)); break;
                }
            }
        }
    }

    public bool isJumping()
    {
        return m_isJumping;
    }

    public float getPlaneMovementSpeed()
    {
        Vector3 move = m_rbody.velocity;
        move.y = 0.0f;
        return move.magnitude;
    }

    public Vector3 getVelocity()
    {
        Vector3 move = m_rbody.velocity;
        return move;
    }

    public void triggerJump(bool p_manualTriggerOption=false)
    {
        switch (m_jumpCount)
        {
            case 0:
                if (jump(m_jumpPower,p_manualTriggerOption)) {m_audioSource.PlayOneShot(m_jumpSnd1);} break;
            case 1:
                if (jump(m_doubleJumpPower,p_manualTriggerOption)) {m_audioSource.PlayOneShot(m_jumpSnd2);} break;
            case 2:
                if (jump(m_tripleJumpPower,p_manualTriggerOption)) {m_audioSource.PlayOneShot(m_jumpSnd3);} break;
            default:
                if (jump(m_jumpPower,p_manualTriggerOption)) {m_audioSource.PlayOneShot(m_jumpSnd1);} break;
        }
    }

    public bool isSteering()
    {
        return m_isSteering;
    }

    void setDirection()
    {
        if (m_isSteering)
        {
            updateSteerDir();
            alignMeshToSteerDir();
            m_moveDir = m_steerDir;
        }
        else if (m_isOnGround)
            m_moveDir = Vector3.zero;
    }

    void updateSteerDir()
    {
        if (m_pointOfViewRunning || m_pointOfViewJumping)
        {
            if (m_isJumping)
                m_steerDir = m_pointOfViewJumping.TransformDirection(m_inputDir);
            else
                m_steerDir = m_pointOfViewRunning.TransformDirection(m_inputDir);
        }
        else
            m_steerDir = m_inputDir;
    }

    void alignMeshToSteerDir()
    {
        m_playerSteerFacing.forward = m_steerDir;
    }


    void steer(float m_multiplier)
    {
        // Basic steering
        //m_inputDir = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
        float magnitude = m_inputDir.magnitude;
        if (magnitude > 0.001f) m_isSteering = true; else m_isSteering = false;
        if (magnitude > 1.0f) m_inputDir.Normalize();
        setDirection();
        m_velocity += new Vector3(m_acceleration * m_multiplier * m_moveDir.x, 0.0f, m_acceleration * m_multiplier * m_moveDir.z);
        if (m_velocity.magnitude > m_maxSpeed) m_velocity = m_velocity.normalized * m_maxSpeed;
        m_rbody.velocity = new Vector3(m_velocity.x,m_rbody.velocity.y,m_velocity.z);
        m_velocity *= m_velocityFallof;
    }

    bool jump(float p_pwr, bool p_force=false)
    {
        if (isJumpBtnDown() || p_force)
        {
            setToJumpStatus(p_force);
            m_currentJumpForce = p_pwr;
            m_rbody.velocity = new Vector3(m_rbody.velocity.x, m_currentJumpForce*0.03f, m_rbody.velocity.z);
            return true;
        }
        return false;
    }

    void setToJumpStatus(bool p_force=false)
    {
        if (!m_isJumping || p_force)
        {            
            if (m_jumpCount > 2) m_jumpCount = 0;
            m_isJumping = true;
            m_jumpCountCoolDown = 0.0f;
            m_jumpCount++;
            if (m_jumpSquisher)
            {
                m_jumpSquisher.setStart(new Vector3(1.0f, 0.7f, 1.0f));
                m_jumpSquisher.setGoal(new Vector3(1.0f, 1.3f, 1.0f), (3-m_jumpCount)*0.08f, true, m_jumpCount*0.15f);
            }
        }
    }

    void coolDownDoubleJumpCount()
    {
        if (!m_isJumping)
        {
            m_jumpCountCoolDown += Time.deltaTime;
            if (m_jumpCountCoolDown > m_doubleJumpCountCoolDownLim)
                m_jumpCount = 0;
        }
    }


    bool isJumpBtnDown()
    {
        return m_jumpButtonDown;
    }

    bool wasJumpBtnReleased()
    {
        return m_jumpButtonReleased;
    }

    void doJump()
    {
        if (isJumpBtnDown() && m_currentJumpForce>0.0f)
        {
            m_rbody.AddForce(Vector3.up * m_currentJumpForce);
            //m_rbody.velocity = new Vector3(m_rbody.velocity.x, m_currentJumpForce, m_rbody.velocity.z);
            m_currentJumpForce *= m_holdJumpDiminishForce; // diminish over time (if user holds button down, allow for small boost, but decrease it)
            if (m_currentJumpForce < 1.0f) m_currentJumpForce = 0.0f;
        }
    }

    void resetJumpForce()
    {
        m_currentJumpForce = 0.0f;
    }

    void groundCheck()
    {
        //RaycastHit hitInfo;
        //bool hit = Physics.SphereCast(new Ray(m_groundCheckPoint.position - Vector3.up * m_groundCheckRadius, Vector3.down), m_groundCheckRadius, out hitInfo, m_groundCheckRadius*3.0f, m_groundLayer);
            Collider[] hits = Physics.OverlapSphere(m_groundCheckPoint.position, m_groundCheckRadius, m_groundLayer);
        //if (hit)
        if (hits.Length > 0)
        {
            if (!m_isOnGround && m_rbody.velocity.y < -10.0f)
            {
                m_jumpSquisher.setGoal(new Vector3(1.0f, 1.0f - Mathf.Clamp(Mathf.Abs(m_rbody.velocity.y * 0.1f), 0.0f, 0.8f), 1.0f), 0.05f, true, 0.05f);
            }
            m_isOnGround = true;
        }
        else
            m_isOnGround = false;
    }

    void OnCollisionEnter(Collision p_coll)
    {
        if (p_coll.contacts.Length > 0)
        {
            if (p_coll.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                Debug.Log("Wall");
                Vector3 normal = p_coll.contacts[0].normal;
                if (p_coll.collider.gameObject.tag == "Bouncy")
                    m_rbody.AddForceAtPosition(p_coll.contacts[0].normal * 1000.0f, p_coll.contacts[0].point);
            }
        }
    }

    void OnDrawGizmos()
    {
        if (m_groundCheckPoint) Gizmos.DrawSphere(m_groundCheckPoint.position, m_groundCheckRadius);
    }
}
