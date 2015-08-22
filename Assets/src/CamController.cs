using UnityEngine;
using System.Collections;

public class CamController : MonoBehaviour {
    public float m_rotationTime = 1.0f;
    public float m_rotationStep = 1.0f;
    private float m_goalAngleY;
    private float m_rotationVelocityY;
    public Transform m_cameraRotationPivot;
    public float m_distanceFacingForward;
    public float m_distanceFacingBackward;
    private float m_currentDist;
    private float m_currentDistVel;
    public float m_distChangingTime = 0.2f;
    public Transform m_internalRotationContainer; // the absolute lookat
    public Transform m_lookAt;
    public Transform m_lookAtInternalFacing; // to determine when player is facing towards camera, so we have to move back a little
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
        m_goalAngleY+=mouseMovement.x;
        // Get smoothed angle
        float yAngle=Mathf.SmoothDampAngle(transform.localRotation.eulerAngles.y, m_goalAngleY, ref m_rotationVelocityY, m_rotationTime);
        // Calculate new position
        Vector3 position = m_cameraRotationPivot.position;
        float goaldist = m_distanceFacingForward;
        if (Vector3.Dot(transform.forward, m_lookAtInternalFacing.forward) < -0.5f) goaldist = m_distanceFacingBackward;
        m_currentDist = Mathf.SmoothDamp(m_currentDist, goaldist, ref m_currentDistVel, m_distChangingTime);
        position += Quaternion.Euler(0, yAngle, 0) * new Vector3(0, 0, -m_currentDist);
        transform.position = position;
        transform.LookAt(m_lookAt);
        // note! We want this rig to not rotate along its x-axis
        transform.localRotation = Quaternion.Euler(0.0f, transform.localRotation.eulerAngles.y, 0.0f);
        m_internalRotationContainer.LookAt(m_lookAt); // however, this internal controller can do that
	}
}
