using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour 
{
    public Rigidbody m_rbody;
    public float m_maxSpeed = 1.0f;
    public Transform m_groundCheckPoint;
    public float m_groundCheckRadius = 0.3f;
    public LayerMask m_groundLayer;
    private bool m_isOnGround;
    private Vector3 m_inputDir;
    private Vector3 m_steerDir;
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

    public Material m_debugMaterial;
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
	// Use this for initialization
	void Start () 
    {
        if (m_camPivotToOffsetOnJump) m_camPivotOriginalLPos = m_camPivotToOffsetOnJump.localPosition;
        if (m_camLookatToOffsetOnJump) m_camLookatOriginalLPos = m_camLookatToOffsetOnJump.localPosition;
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (m_debugMaterial)
        {
            switch (m_jumpCount)
            {
                case 0:
                    m_debugMaterial.color = Color.white; break;
                case 1:
                    m_debugMaterial.color = Color.green; break;
                case 2:
                    m_debugMaterial.color = Color.blue; break;
                case 3:
                    m_debugMaterial.color = Color.red; break;
            }
        }


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
        }
    }

    public void triggerJump(bool p_manualTriggerOption=false)
    {
        switch (m_jumpCount)
        {
            case 0:
                jump(m_jumpPower,p_manualTriggerOption); break;
            case 1:
                jump(m_doubleJumpPower,p_manualTriggerOption); break;
            case 2:
                jump(m_tripleJumpPower,p_manualTriggerOption); break;
            default:
                jump(m_jumpPower,p_manualTriggerOption); break;
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
        m_rbody.velocity = new Vector3(m_maxSpeed * m_multiplier * m_moveDir.x, m_rbody.velocity.y, m_maxSpeed * m_multiplier * m_moveDir.z);
    }

    void jump(float p_pwr, bool p_force=false)
    {
        if (isJumpBtnDown() || p_force)
        {
            setToJumpStatus(p_force);
            m_currentJumpForce = p_pwr;
            m_rbody.velocity = new Vector3(m_rbody.velocity.x, m_currentJumpForce*0.03f, m_rbody.velocity.z);
        }
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
                m_jumpSquisher.setGoal(new Vector3(1.0f, 1.8f, 1.0f), (3-m_jumpCount)*0.08f, true, m_jumpCount*0.15f);
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
        Collider[] hits = Physics.OverlapSphere(m_groundCheckPoint.position, m_groundCheckRadius, m_groundLayer);
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
        if (p_coll.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Debug.Log("Wall");
            Vector3 normal=p_coll.contacts[0].normal;
            if (p_coll.collider.gameObject.tag=="Bouncy")
                m_rbody.AddForceAtPosition(p_coll.contacts[0].normal*1000.0f, p_coll.contacts[0].point);
        }
    }

    void OnDrawGizmos()
    {
        if (m_groundCheckPoint) Gizmos.DrawSphere(m_groundCheckPoint.position, m_groundCheckRadius);
    }
}
