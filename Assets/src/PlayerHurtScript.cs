using UnityEngine;
using System.Collections;
using UnityEngine.UI;

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
    public Renderer m_playerRenderer;
    public Color m_hurtCol;
    private Color m_origCol;
    public float m_blinkSpd;
    private Vector3 m_hurtVec;
    public int m_hp;
    public EndStateEffect m_gameOver;
    public Text m_healthText;
    public string m_healTag;
	// Use this for initialization
	void Start () {
        m_origCol = m_playerRenderer.material.color;
        m_healthText.text = m_hp.ToString();
	}
	
	// Update is called once per frame
	void Update () 
    {
	    if (m_hurtCounter>0.0f)
        {
            m_hurtCounter -= Time.deltaTime;
            m_playerRenderer.material.color = Color.Lerp(m_hurtCol, m_origCol, Mathf.Sin(m_blinkSpd * (1.0f - m_hurtCounter / m_hurtTime)));
            if (m_hurtCounter<=0.0f)
            {
                m_playerRenderer.material.color = m_origCol;
                //m_rbody.velocity = new Vector3(m_hurtVec.x, m_rbody.velocity.y, m_hurtVec.z);
                //foreach (MonoBehaviour script in m_scriptsToDisableOnMonster)
                //    script.enabled = true;
            }
        }
	}

    void FixedUpdate()
    {
        if (m_hurtCounter > 0.0f)
        {
            m_rbody.velocity = new Vector3(m_hurtVec.x, m_rbody.velocity.y, m_hurtVec.z);
        }
    }

    void LateUpdate()
    {
        if (m_hurtCounter > 0.0f)
        {
            m_rbody.velocity = new Vector3(m_hurtVec.x, m_rbody.velocity.y, m_hurtVec.z);
        }
    }


    public void hurt(int p_hp)
    {
        Debug.Log("Player hurt by "+p_hp);
        m_hp -= p_hp;
        m_healthText.text = m_hp.ToString();
        if (m_hp <= 0)
            m_gameOver.activate();
    }


    public void heal(int p_hp)
    {
        m_hp += p_hp;
        m_healthText.text = m_hp.ToString();
        if (m_hp <= 0)
            m_gameOver.activate();
    }


    void OnTriggerEnter(Collider p_coll)
    {
        Debug.Log(p_coll.gameObject.tag);
        if (p_coll.gameObject.tag == m_killAreaTag)
        {
            StartCoroutine(respawn());
        }
        else if (p_coll.gameObject.tag == m_healTag)
        {
            heal(1);
            Destroy(p_coll.gameObject);
        }
    }

    void OnCollisionEnter(Collision p_coll)
    {
        
        Debug.Log(p_coll.gameObject.tag);
        if (p_coll.gameObject.tag == m_monsterTag && m_hurtCounter<=0.0f && m_rbody.velocity.y>=-0.01f)
        {
            m_hurtCounter=m_hurtTime;
            if (p_coll.contacts.Length>0)
            {
                ContactPoint p = p_coll.contacts[0];
                Vector3 normal = p.normal;
                //m_rbody.AddForceAtPosition(normal * m_hurtForce + Vector3.up * m_hurtForce, p.point, ForceMode.Impulse);
                if (normal.magnitude > 0) m_hurtVec = normal; else m_hurtVec = new Vector3(Random.Range(-1.0f, 1.0f), 0.1f, Random.Range(-1.0f, 1.0f));
                //m_rbody.MovePosition(m_rbody.position + m_hurtVec * 3.0f);
                m_hurtVec*=m_hurtForce;
                hurt(1);
                //foreach (MonoBehaviour script in m_scriptsToDisableOnMonster)
                //    script.enabled = false;
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
        m_playerRenderer.material.color = m_origCol;
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
