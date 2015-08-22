using UnityEngine;
using System.Collections;

public class MonsterController : MonoBehaviour 
{
    public PlayerController m_controller;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
       if (Random.Range(0,100)>95)
        m_controller.setSteeringInput(new Vector3(Random.Range(-1.0f, 1.0f), 0.0f, Random.Range(-1.0f, 1.0f)));
	}

    public void hitByPlayer()
    {
        Destroy(gameObject);
    }
}
