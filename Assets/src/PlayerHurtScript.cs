using UnityEngine;
using System.Collections;

public class PlayerHurtScript : MonoBehaviour 
{
    public MonoBehaviour[] m_scriptsToDisableOnDeath;
    public MonoBehaviour[] m_scriptsToDisableOnMonster;
    public string m_killAreaTag, m_monsterTag;
    public CheckPointHandler m_checkPoints;
    public float m_waitTimeForRespawn = 1;
    public Transform m_cameraRig;
    public Rigidbody m_rbody;
    public float m_hurtTime = 0;
    private float m_hurtCounter = 0.0f;
    public float m_hurtForce = 100.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
	    if (m_hurtCounter>0.0f)
        {
            m_hurtCounter -= Time.deltaTime;
            if (m_hurtCounter<=0.0f)
            {
                foreach (MonoBehaviour script in m_scriptsToDisableOnMonster)
                    script.enabled = true;
            }
        }
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

    void OnCollisionEnter(Collision p_coll)
    {
        
        Debug.Log(p_coll.gameObject.tag);
        if (p_coll.gameObject.tag == m_monsterTag && m_hurtCounter<=0.0f)
        {
            m_hurtCounter=m_hurtTime;
            if (p_coll.contacts.Length>0)
            {
                ContactPoint p = p_coll.contacts[0];
                Vector3 normal = p.normal;
                m_rbody.AddForceAtPosition(normal * m_hurtForce + Vector3.up * m_hurtForce, p.point, ForceMode.Impulse);
                foreach (MonoBehaviour script in m_scriptsToDisableOnMonster)
                    script.enabled = false;
            }
        }
         
    }

    public bool isHurting()
    {
        return m_hurtCounter > 0.0f;
    }

    IEnumerator respawn()
    {
        Debug.Log("Player fell!");
        // Disable scripts when falling
        foreach (MonoBehaviour script in m_scriptsToDisableOnDeath)
            script.enabled = false;
        m_hurtCounter = 0.0f;
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
