using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatePoligon : MonoBehaviour {

    public Material mat;

    public float width, height;
	// Use this for initialization
	void Start () {
        MeshFilter mf = gameObject.AddComponent<MeshFilter>();
        //MeshFilter mf = GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        mf.mesh = mesh;
        //Material mat = Resources.Load("Materials/Material Line", typeof(Material)) as Material;
        
        MeshRenderer mr = gameObject.AddComponent<MeshRenderer>();
        //MeshRenderer mr = GetComponent<MeshRenderer>();
        mr.material = mat;
        //mr.material.color= new Color(1, 1, 0.8f, 0.5f);



        //Vertices
        Vector3[] vertices = new Vector3[4] {
            new Vector3(0,-0.5f*height,1),
            new Vector3(width,-2*height,1),
            new Vector3(width,2*height,1),
            new Vector3(0,0.5f*height,1)
        };

        //Triangle
        int[] tri = new int[6] {0,2,1,0,3,2};

        // Normal
        Vector3[] normals = new Vector3[4] {
            -Vector3.one,-Vector3.forward,-Vector3.forward,-Vector3.forward
        };

        //UV
        Vector2[] uv = new Vector2[4] {
             //new Vector2(0.45f,0.5f),new Vector2(0.55f,0.5f),new Vector2(0.55f,0.5f),new Vector2(0.45f,0.5f)
             new Vector2(0,0.5f),new Vector2(0.5f,0.5f),new Vector2(0.5f,0.5f),new Vector2(0,0.5f)
        };

        mesh.vertices = vertices;
        mesh.triangles = tri;
        mesh.normals = normals;
        mesh.uv = uv;

    }
	
	// Update is called once per frame
	/*void Update () {
        width += Time.deltaTime;
        Vector3[] vertices = new Vector3[4] {
            new Vector3(0,0,0),
            new Vector3(width,0,0),
            new Vector3(width,height,0),
            new Vector3(0,height,0)
        };

        GetComponent<MeshFilter>().mesh.vertices = vertices; 
    }*/
}
