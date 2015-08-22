using UnityEngine;
using System.Collections;

public class PlayerStompMonster : MonoBehaviour 
{
    public Transform m_monsterStompCheckPoint;
    public float m_monsterCheckRadius = 0.3f;
    public LayerMask m_monsterLayer;
    public PlayerController m_playerController;
    public Rigidbody m_rbody;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void FixedUpdate()
    {
        if (m_rbody.velocity.y<-0.01f) monsterStompCheck();
    }

    void monsterStompCheck()
    {
        Collider[] hits = Physics.OverlapSphere(m_monsterStompCheckPoint.position, m_monsterCheckRadius, m_monsterLayer);
        if (hits.Length > 0)
        {
            bool gotAHit = false;
            foreach (Collider coll in hits)
            {
                Transform parentObj = coll.gameObject.transform.parent;
                if (parentObj) parentObj = parentObj.parent;
                if (parentObj) // controller is two steps up in monsters
                {
                    MonsterController monster = parentObj.GetComponent<MonsterController>();
                    if (monster)
                    {
                        gotAHit = true;
                        monster.hitByPlayer();
                        break; // break loop
                    }
                }
            }
            if (gotAHit)
            {
                m_playerController.triggerJump(true);
            }
        }
    }

}
