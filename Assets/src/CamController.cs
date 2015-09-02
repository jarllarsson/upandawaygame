using UnityEngine;
using System.Collections;

public class CamController : MonoBehaviour {
    public float m_rotationTimeY = 1.0f;
    public float m_rotationStepMouse = 1.0f;
    public float m_rotationStepJoyY = 1.0f;
    public float m_rotationStepJoyX = 1.0f;
    private float m_goalAngleY;
    private float m_rotationVelocityY;

    public float m_rotationTimeX = 1.0f;
    private float m_goalAngleX;
    private float m_rotationVelocityX;
    public float m_minXAngle, m_maxXAngle;
    public Transform m_camXAngleContainer;

    public Transform m_cameraRotationPivot;
    public float m_distanceFacingForward;
    public float m_distanceFacingBackward;
    private float m_currentDist;
    private float m_currentDistVel;
    public float m_distChangingTime = 0.2f;
    public float m_turnbackTime = 3.0f;
    private float m_turnbackTimeTick = 3.0f;


    public Transform m_internalRotationContainer; // the absolute lookat
    public Transform m_lookAt;
    public Transform m_lookAtInternalFacing; // to determine when player is facing towards camera, so we have to move back a little
    public PlayerController m_playerController; // to know when player is moving etc
	// Use this for initialization
	void Start () 
    {
        m_currentDist = m_distanceFacingForward;
	}
	
	// Update is called once per frame
	void FixedUpdate () 
    {
        float playerDirDotCameraDir=Vector3.Dot(transform.forward, m_lookAtInternalFacing.forward);
        // Get new angle
        Vector2 mouseMovement = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        Vector2 joyMovement = new Vector2(Input.GetAxis("Joy X2"), Input.GetAxis("Joy Y2"));
        // automatically center camera behind player after a while of no input
        if (mouseMovement.magnitude > 0.0f || joyMovement.magnitude > 0.0f || m_playerController.isJumping() || playerDirDotCameraDir<-0.7f || !m_playerController.isSteering())
            m_turnbackTimeTick = m_turnbackTime;
        else
            m_turnbackTimeTick -= Time.deltaTime;
        if (m_turnbackTimeTick<=0.0f)
        {
            m_goalAngleY=Mathf.LerpAngle(m_goalAngleY,m_lookAtInternalFacing.rotation.eulerAngles.y,0.02f);
        }
        // slowly turn camera behind player if no input, and not facing toward camera


        // regular input driven movement
        m_goalAngleY += mouseMovement.x * m_rotationStepMouse;
        m_goalAngleX -= mouseMovement.y * m_rotationStepMouse;
        m_goalAngleY -= joyMovement.x * m_rotationStepJoyX;
        m_goalAngleX += joyMovement.y * m_rotationStepJoyY;
        m_goalAngleX = Mathf.Clamp(m_goalAngleX, m_minXAngle, m_maxXAngle);
        // Get smoothed angle
        float yAngle = Mathf.SmoothDampAngle(transform.localRotation.eulerAngles.y, m_goalAngleY, ref m_rotationVelocityY, m_rotationTimeY);
        float xAngle = Mathf.SmoothDampAngle(m_camXAngleContainer.localRotation.eulerAngles.x, m_goalAngleX, ref m_rotationVelocityX, m_rotationTimeX);
        // Calculate new position, using distance value
        Vector3 position = m_cameraRotationPivot.position;
        float goaldist = m_distanceFacingForward;
        // Distance depends on facing and if player is walking:
        if (playerDirDotCameraDir < -0.5f && m_playerController.isSteering())
        {
            goaldist = m_distanceFacingBackward;
        }
        m_currentDist = Mathf.Lerp(m_currentDist, goaldist, 0.01f);
            //Mathf.SmoothDamp(m_currentDist, goaldist, ref m_currentDistVel, m_distChangingTime);
        if (System.Double.IsNaN(xAngle)) Debug.Log("xnan");
        if (System.Double.IsNaN(yAngle)) Debug.Log("ynan");
        // Update new position
        Quaternion rot = Quaternion.Euler(xAngle, yAngle, 0);
        position += rot * new Vector3(0, 0, -m_currentDist);
        transform.position = position;
        transform.LookAt(m_lookAt);
        // note! We want this rig to only rotate along y
        float internalXAngle = transform.localRotation.eulerAngles.x;
        transform.localRotation = Quaternion.Euler(0.0f, transform.localRotation.eulerAngles.y, 0.0f);
        // the x container then takes x
        m_camXAngleContainer.localRotation = Quaternion.Euler(internalXAngle, 0.0f, 0.0f);
        // And the rest on the lookat
        m_internalRotationContainer.LookAt(m_lookAt); // however, this internal controller can do that
        
	}
}
