using UnityEngine;
using System.Collections;

public class CamController : MonoBehaviour {
    public float m_rotationTimeY = 1.0f;
    public float m_rotationStep = 1.0f;
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
        // Get new angle
        Vector2 mouseMovement = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        m_goalAngleY += mouseMovement.x * m_rotationStep;
        m_goalAngleX -= mouseMovement.y * m_rotationStep;
        m_goalAngleX = Mathf.Clamp(m_goalAngleX, m_minXAngle, m_maxXAngle);
        // Get smoothed angle
        float yAngle = Mathf.SmoothDampAngle(transform.localRotation.eulerAngles.y, m_goalAngleY, ref m_rotationVelocityY, m_rotationTimeY);
        float xAngle = Mathf.SmoothDampAngle(m_camXAngleContainer.localRotation.eulerAngles.x, m_goalAngleX, ref m_rotationVelocityX, m_rotationTimeX);
        // Calculate new position, using distance value
        Vector3 position = m_cameraRotationPivot.position;
        float goaldist = m_distanceFacingForward;
        // Distance depends on facing and if player is walking:
        if (Vector3.Dot(transform.forward, m_lookAtInternalFacing.forward) < -0.5f && m_playerController.isSteering()) goaldist = m_distanceFacingBackward;
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
