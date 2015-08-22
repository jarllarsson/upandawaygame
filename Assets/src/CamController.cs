using UnityEngine;
using System.Collections;

public class CamController : MonoBehaviour {
    public float m_rotationTime = 1.0f;
    public float m_rotationStep = 1.0f;
    private float m_goalAngleY;
    private float m_rotationVelocityY;
    public Transform m_cameraRotationPivot;
    public float m_distance;
    public Transform m_internalRotationContainer; // the absolute lookat
    public Transform m_lookAt;
	// Use this for initialization
	void Start () 
    {
	
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
        position += Quaternion.Euler(0, yAngle, 0) * new Vector3(0, 0, -m_distance);
        transform.position = position;
        transform.LookAt(m_lookAt);
        // note! We want this rig to not rotate along its x-axis
        transform.localRotation = Quaternion.Euler(0.0f, transform.localRotation.eulerAngles.y, 0.0f);
        m_internalRotationContainer.LookAt(m_lookAt); // however, this internal controller can do that
	}
}
