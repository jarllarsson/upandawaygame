using UnityEngine;
using System.Collections;

public class OnWin : MonoBehaviour {
    public EndStateEffect m_winFx;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider p_coll)
    {
        Debug.Log(p_coll.gameObject.tag);
        if (p_coll.gameObject.tag == "Player")
        {
            m_winFx.activate();
        }
    }

}
