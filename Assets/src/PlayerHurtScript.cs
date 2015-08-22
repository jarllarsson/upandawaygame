using UnityEngine;
using System.Collections;

public class PlayerHurtScript : MonoBehaviour 
{
    public MonoBehaviour[] m_scriptsToDisableOnDeath;
    public string m_killAreaTag;
    public CheckPointHandler m_checkPoints;
    public float m_waitTimeForRespawn = 1;
    public Transform m_cameraRig;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    public void hurt(int p_hp)
    {
        Debug.Log("Player hurt by "+p_hp);
    }


    void OnTriggerEnter(Collider p_coll)
    {
        Debug.Log(p_coll.gameObject.tag);
        if (p_coll.gameObject.tag == m_killAreaTag)
        {
            StartCoroutine(respawn());
        }
    }


    IEnumerator respawn()
    {
        Debug.Log("Player fell!");
        // Disable scripts when falling
        foreach (MonoBehaviour script in m_scriptsToDisableOnDeath)
            script.enabled = false;
        // Wait a little...
        yield return new WaitForSeconds(m_waitTimeForRespawn);
        Debug.Log("Respawn");
        // Respawn player
        Vector3 pos = m_checkPoints.getRespawnPos();
        transform.position = pos;
        Rigidbody rbody = transform.GetComponent<Rigidbody>();
        if (rbody) rbody.velocity = Vector3.zero;
        m_cameraRig.position = pos + Vector3.up * 5.0f + transform.forward*-8.0f;
        hurt(1);
        // reenable scripts
        foreach (MonoBehaviour script in m_scriptsToDisableOnDeath)
            script.enabled = true;
    }
}
