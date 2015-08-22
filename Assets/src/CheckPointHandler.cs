using UnityEngine;
using System.Collections;

public class CheckPointHandler : MonoBehaviour 
{
    private Transform m_latestRespawnPoint;
    private Vector3 m_origin;
	// Use this for initialization
	void Start () {
        m_origin = transform.position;
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    public Vector3 getRespawnPos()
    {
        if (m_latestRespawnPoint)
            return m_latestRespawnPoint.position;
        else
            return m_origin;
    }

    void OnTriggerEnter(Collider p_coll)
    {
        if (p_coll.gameObject.tag=="CheckPoint")
        {
            m_latestRespawnPoint = p_coll.gameObject.transform;
        }
    }
}
