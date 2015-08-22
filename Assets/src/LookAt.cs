using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class LookAt : MonoBehaviour 
{
    public Transform m_target;

	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (m_target) transform.LookAt(m_target);
	}
}
