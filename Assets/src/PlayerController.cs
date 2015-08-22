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

    public Transform m_playerSteerFacing;
    public Transform m_pointOfView;
    private bool m_isSteering=false;

    public float m_jumpPower = 1.0f;
    public float m_doubleJumpPower = 2.0f;
    public float m_tripleJumpPower = 3.0f;

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

	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
        switch (m_jumpCount)
        {
            case 0:
                m_debugMaterial.color=Color.white; break;
            case 1:
                m_debugMaterial.color=Color.green; break;
            case 2:
                m_debugMaterial.color=Color.blue; break;
            case 3:
                m_debugMaterial.color = Color.red; break;
        }


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
        m_jumpButtonDown = Input.GetAxis("Jump") > 0.0f;


        // Check if we're on ground, only when we're still(on y-axis) or falling down
        if (m_rbody.velocity.y <= 0.0f) groundCheck();
        // Reset jump if we're falling to ground
        if (m_isOnGround && m_rbody.velocity.y <= 0.0f)
            m_isJumping = false;
        //
        if (m_isOnGround && !m_isJumping)
        {
            m_isJumping = false;
            steer(1.0f);
            coolDownDoubleJumpCount(); // Count the 
            if (wasJumpBtnReleased())
            {
                switch (m_jumpCount)
                {
                    case 0:
                        jump(m_jumpPower); break;
                    case 1:
                        jump(m_doubleJumpPower); break;
                    case 2:
                        jump(m_tripleJumpPower); break;
                    default:
                        jump(m_jumpPower); break;
                }
            }
        }
        if (m_isJumping)
        {
            steer(0.5f);
            // While jumping
            m_jumpCountCoolDown = 0.0f; // reset jump counter cooldown, start afresh
            doJump(); // having this here allows the user to hold down button for a while for higher jump!
        }
    }

    void setDirection()
    {
        if (m_isSteering)
        {
            m_steerDir = m_pointOfView.TransformDirection(m_inputDir);
            m_playerSteerFacing.forward = m_steerDir;
        }
        else
            m_steerDir = Vector3.zero;
    }

    void steer(float m_multiplier)
    {
        // Basic steering
        m_inputDir = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
        float magnitude = m_inputDir.magnitude;
        if (magnitude > 0.001f) m_isSteering = true; else m_isSteering = false;
        if (magnitude > 1.0f) m_inputDir.Normalize();
        setDirection();
        m_rbody.velocity = new Vector3(m_maxSpeed * m_multiplier * m_steerDir.x, m_rbody.velocity.y, m_maxSpeed * m_multiplier * m_steerDir.z);
    }

    void jump(float p_pwr)
    {
        if (isJumpBtnDown())
        {
            setToJumpStatus();
            m_currentJumpForce = p_pwr;
        }
    }

    void setToJumpStatus()
    {
        if (!m_isJumping)
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
            if (!m_isOnGround) 
                m_jumpSquisher.setGoal(new Vector3(1.0f, 1.0f-Mathf.Clamp(Mathf.Abs(m_rbody.velocity.y*0.1f),0.0f,0.8f), 1.0f), 0.05f, true, 0.05f);
            m_isOnGround = true;
        }
        else
            m_isOnGround = false;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawSphere(m_groundCheckPoint.position, m_groundCheckRadius);
    }
}
