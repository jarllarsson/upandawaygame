using UnityEngine;
using System.Collections;

public class MonsterController : MonoBehaviour 
{
    public PlayerController m_controller;
    public GameObject[] m_collidersToUntag;
    public Rigidbody m_rbody;
    public Transform m_animateOnDeath;
    public Transform m_player;
    private bool m_die = false;

    public Renderer m_renderAnimOnDeath;
    public Color m_hurtCol;
    private Color m_origCol;
    public float m_blinkSpd;

    public AudioSource m_audioSource;
    public AudioClip m_dieSound;
    public string m_killAreaTag;
    public float m_atkDist;

	// Use this for initialization
	void Start () {
        Random.seed = (int)(1000.0f*Time.time);
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (!m_die)
        {
            Vector3 dirPlayer = m_player.position - transform.position;
            float distToPlayer = Vector3.Magnitude(dirPlayer);
            if (distToPlayer>m_atkDist)
            {
                m_controller.m_maxSpeed = 2;
                // normal walk
                if (Random.Range(0, 200) > 195)
                    m_controller.setSteeringInput(new Vector3(Random.Range(-1.0f, 1.0f), 0.0f, Random.Range(-1.0f, 1.0f)));
            }
            else // attack
            {
                m_controller.m_maxSpeed = 4;
                Vector3 dirPlane = dirPlayer.normalized;
                dirPlane.y = 0.0f;
                m_controller.setSteeringInput(dirPlane.normalized);
            }
        }
        else
        {
            m_renderAnimOnDeath.material.color = Color.Lerp(m_hurtCol, m_origCol, Mathf.Sin(m_blinkSpd * Time.time));
            m_animateOnDeath.localRotation *= Quaternion.Euler(new Vector3(0.0f, Time.deltaTime * 640.0f, 0.0f));
        }
	}

    IEnumerator die()
    {
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }

    public void hitByPlayer()
    {
        m_controller.enabled = false;
        m_rbody.AddForce(10.0f * new Vector3(Random.Range(-1.0f, 1.0f), 0.0f, Random.Range(-1.0f, 1.0f)), ForceMode.Impulse);
        m_die = true;
        m_audioSource.PlayOneShot(m_dieSound);
        foreach (GameObject g in m_collidersToUntag)
        {
            g.tag = "Untagged";
            g.layer = LayerMask.NameToLayer("Default");
        }
        StartCoroutine(die());
    }

    void OnTriggerEnter(Collider p_coll)
    {
        Debug.Log(p_coll.gameObject.tag);
        if (p_coll.gameObject.tag == m_killAreaTag)
        {
            Destroy(gameObject);
        }
    }

}
