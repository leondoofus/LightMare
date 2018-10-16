using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightRay : MonoBehaviour {

    public Vector3 StartPosition1, StartPosition2;
    public float Direction1, Direction2;
    public float Length1,Length2;
    public Color Col;
    //public float Width;
    //public float Divergence;
    public float Intensity=0.05f;
    public bool isVisible;
    public bool HasWaist;
    //Vector3 WaistPos;
    public float cos1, sin1, cos2, sin2, proj1, proj2, param1, param2, div; // vecteur directeur, proj et parametre
    public OpticalComponent Origin;
    public OpticalComponent End;
    public int depth;

    const float EPSILON = 0.00001f; // pour les erreurs d'arrondis

    //Mesh mesh;
    void InitilizedMesh()
    {
        MeshFilter mf = gameObject.AddComponent<MeshFilter>();
        
        //Material mat = Resources.Load("Materials/Material Line", typeof(Material)) as Material;
        Material mat = Resources.Load("Materials/RayDiv", typeof(Material)) as Material;
        MeshRenderer mr = gameObject.AddComponent<MeshRenderer>();
        mr.material = mat;
        mr.material.color = Col;
        mr.sortingLayerName="Rays";

        Mesh m = new Mesh
        {
            vertices = new Vector3[6],
            triangles = new int[6] { 0, 1, 2, 3, 4, 5 }
        };
        
        m.normals = new Vector3[6] {
                -Vector3.forward,-Vector3.forward,-Vector3.forward,-Vector3.forward,-Vector3.forward,-Vector3.forward
            };
        m.uv = new Vector2[6] {
                 new Vector2(0f,0f),new Vector2(0f,0f),new Vector2(1f,0f),new Vector2(1f,0f),new Vector2(0f,0f),new Vector2(1f,0f)
            }; 

        mf.mesh=m;

    }

   

    public void Initiliaze()
    {
        InitilizedMesh();
    }

    public void Draw() {
        // Draw the rays recursively;
        //if (!isVisible) return;
        DrawMesh();
        foreach(Transform child in transform)
        {
            child.GetComponent<LightRay>().Draw();
        }
    }

 
    public void ComputeDir() { //Calcule les vecteurs directeurs et paramètres
        cos1 = Mathf.Cos(Direction1);
        sin1 = Mathf.Sin(Direction1);
        proj1 = StartPosition1.x * cos1 + StartPosition1.y * sin1;
        param1 = -StartPosition1.x * sin1 + StartPosition1.y * cos1;
        cos2 = Mathf.Cos(Direction2);
        sin2 = Mathf.Sin(Direction2);
        proj2 = StartPosition2.x * cos2 + StartPosition2.y * sin2;
        param2 = -StartPosition2.x * sin2 + StartPosition2.y * cos2;

        div = Direction2 - Direction1;
        if (div < 0) div = -div;
        if (div > 2 * Mathf.PI) div -= 2 * Mathf.PI;
    }

    //public bool colimated;

    //public float p2start;
    void DrawMesh() {

        Vector3[] vertices;
        Vector2[] uv;

        //Test if a waist exists
        Vector3 EndPosition1 = StartPosition1 + Length1 * new Vector3(cos1, sin1, 0);
        Vector3 EndPosition2 = StartPosition2 + Length2 * new Vector3(cos2, sin2, 0);

        float p1 = param1;  
        float p2start = -StartPosition2.x * sin1 + StartPosition2.y * cos1;
        float p2end = -EndPosition2.x * sin1 + EndPosition2.y * cos1;

        if (p2start<p1 && p2end> p1 || p2start > p1 && p2end<p1)
        {
            Vector3 WaistPos = ((p2start - p1) * EndPosition2 - (p2end - p1) * StartPosition2) / (p2start - p2end);

            float cs1, cs2, ce1, ce2; //Couleur des vertex ( Shader uv en 1/u )
            
            float Ilum = div / Intensity;

            cs1 = (cos1 * (WaistPos.x - StartPosition1.x) + sin1 * (WaistPos.y - StartPosition1.y)) * Ilum;
            if (cs1 < 0) cs1 = -cs1;
            cs2 = (cos2 * (WaistPos.x - StartPosition2.x) + sin2 * (WaistPos.y - StartPosition2.y)) * Ilum;
            if (cs2 < 0) cs2 = -cs2;
            ce1 = (cos1 * (WaistPos.x - EndPosition1.x) + sin1 * (WaistPos.y - EndPosition1.y)) * Ilum;
            if (ce1 < 0) ce1 = -ce1;
            ce2 = (cos2 * (WaistPos.x - EndPosition2.x) + sin2 * (WaistPos.y - EndPosition2.y)) * Ilum;
            if (ce2 < 0) ce2 = -ce2;

            //cs1 = Vector3.Distance(StartPosition1,WaistPos) * div ; 
            //cs2 = Vector3.Distance(StartPosition2,WaistPos) * div; 
            //ce1 = Vector3.Distance(EndPosition1,WaistPos) * div; 
            //ce2 = Vector3.Distance(EndPosition2,WaistPos) * div;

            vertices = new Vector3[6] {
                StartPosition1,
                StartPosition2,
                WaistPos,
                WaistPos,
                EndPosition1,
                EndPosition2
            };

            uv = new Vector2[6] {
                 new Vector2(cs1,0f),new Vector2(cs2,0f),new Vector2(0f,0f),new Vector2(0f,0f),new Vector2(ce1,0f),new Vector2(ce2,0f)
            };

        }
        else
        {
            float cs1, cs2, ce1, ce2; //Couleur des vertex ( Shader uv en 1/u )

            float Ilum = div / Intensity;

            float p = p2start - p2end;
            if (p < 0) p = -p;

            if (p > EPSILON)
            {
                Vector3 WaistPos = ((p2start - p1) * EndPosition2 - (p2end - p1) * StartPosition2) / (p2start - p2end);

                cs1 = (cos1 * (WaistPos.x - StartPosition1.x) + sin1 * (WaistPos.y - StartPosition1.y)) * Ilum;
                if (cs1 < 0) cs1 = -cs1;
                cs2 = (cos2 * (WaistPos.x - StartPosition2.x) + sin2 * (WaistPos.y - StartPosition2.y)) * Ilum;
                if (cs2 < 0) cs2 = -cs2;
                ce1 = (cos1 * (WaistPos.x - EndPosition1.x) + sin1 * (WaistPos.y - EndPosition1.y)) * Ilum;
                if (ce1 < 0) ce1 = -ce1;
                ce2 = (cos2 * (WaistPos.x - EndPosition2.x) + sin2 * (WaistPos.y - EndPosition2.y)) * Ilum;
                if (ce2 < 0) ce2 = -ce2;
                
                //cs1 = Vector3.Distance(StartPosition1, WaistPos) * div;
                //cs2 = Vector3.Distance(StartPosition2, WaistPos) * div;
                //ce1 = Vector3.Distance(EndPosition1, WaistPos) * div;
                //ce2 = Vector3.Distance(EndPosition2, WaistPos) * div;
            }
            else
            {   // Faisceau collimaté
                float cc = (p2start - p1);
                if (cc < 0) cc = -cc;
                cs1 = cs2 = ce1 = ce2 = cc / Intensity;
            }

            vertices = new Vector3[6] {
                StartPosition1,
                StartPosition2,
                EndPosition1,
                EndPosition1,
                StartPosition2,
                EndPosition2
                
            };

            uv = new Vector2[6] {
                 new Vector2(cs1,0f),new Vector2(cs2,0f),new Vector2(ce1,0f),new Vector2(ce1,0f),new Vector2(cs2,0f),new Vector2(ce2,0f)
            };


        }

        Mesh m = GetComponent<MeshFilter>().mesh;

        m.vertices = vertices;
        m.uv = uv;
      
        GetComponent<MeshRenderer>().material.color = Col;

    }


    
}
