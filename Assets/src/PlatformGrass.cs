using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class PlatformGrass : MonoBehaviour 
{
    private MeshFilter m_mesh;
    private static float yScaleGlobal=1.0f;
    private float m_yScale;
    public Vector3[] m_sides;
	// Use this for initialization
	void Awake() 
    {
        m_yScale = yScaleGlobal / transform.localScale.y;
        m_mesh = gameObject.GetComponent<MeshFilter>();
        foreach (Vector3 v in m_sides)
            createGrass(v);
        combine();
	}

    void Start()
    {
        List<Transform> children = new List<Transform>(GetComponentsInChildren<Transform>());
        foreach (Transform m in children)
        {
            if (m.gameObject!=gameObject)
                Destroy(m.gameObject);
        }
    }
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    void combine()
    {
        List<MeshFilter> meshFilters = new List<MeshFilter>(GetComponentsInChildren<MeshFilter>());
        Vector3 opos=transform.position;
        Quaternion orot=transform.rotation;
        Vector3 oscale=transform.localScale;
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;
        meshFilters.Add(m_mesh);
        CombineInstance[] combine = new CombineInstance[meshFilters.Count];
        int i = 0;
        while (i < meshFilters.Count)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            i++;
        }
        transform.GetComponent<MeshFilter>().mesh = null;
        transform.GetComponent<MeshFilter>().mesh = new Mesh();
        transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
        transform.position = opos;
        transform.rotation = orot;
        transform.localScale = oscale;

        //transform.gameObject.active = true;
    }

    GameObject createGrass(Vector3 side)
    {
        GameObject grass = GameObject.CreatePrimitive(PrimitiveType.Quad);
        Destroy(grass.GetComponent<Collider>());
        grass.transform.parent = gameObject.transform;
        grass.transform.localPosition = new Vector3(side.x*0.5f,0.0f,side.z*0.5f); // position exactly on side
        grass.transform.position += transform.rotation * side * 0.001f; // add extra small offset that's unaffected by scale (to avoid z-fighting)
        grass.transform.forward = transform.rotation * -side; // point outwards
        grass.transform.localScale = new Vector3(1.0f, m_yScale, 1.0f); // fill whole side, but keep y-scale global

        grass.transform.position = new Vector3(grass.transform.position.x, grass.transform.position.y + transform.localScale.y * 0.5f - transform.localScale.y * grass.transform.localScale.y * 0.5f, grass.transform.position.z);
        return grass;
    }
}
