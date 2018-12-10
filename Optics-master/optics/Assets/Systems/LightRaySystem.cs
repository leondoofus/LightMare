using UnityEngine;
using FYFY;

public class LightRaySystem : FSystem {
    private Family _rayGO = FamilyManager.getFamily(new AllOfComponents(typeof(LightRay)));

    // Use to process your families.
    protected override void onProcess(int familiesUpdateCount) {
        foreach (GameObject go in _rayGO)
        {
            LightRay r = go.GetComponent<LightRay>();
            if (r != null)
            {
                Draw(r);
            }
        }
	}

    private void Draw(LightRay r)
    {
        DrawMesh(r);
        foreach (Transform child in r.transform)
        {
            Draw(child.GetComponent<LightRay>());
        }
    }

    private void DrawMesh(LightRay r)
    {

        Vector3[] vertices;
        Vector2[] uv;

        //Test if a waist exists
        Vector3 EndPosition1 = r.StartPosition1 + r.Length1 * new Vector3(r.cos1, r.sin1, 0);
        Vector3 EndPosition2 = r.StartPosition2 + r.Length2 * new Vector3(r.cos2, r.sin2, 0);

        float p1 = r.param1;
        float p2start = -r.StartPosition2.x * r.sin1 + r.StartPosition2.y * r.cos1;
        float p2end = -EndPosition2.x * r.sin1 + EndPosition2.y * r.cos1;

        if (p2start < p1 && p2end > p1 || p2start > p1 && p2end < p1)
        {
            Vector3 WaistPos = ((p2start - p1) * EndPosition2 - (p2end - p1) * r.StartPosition2) / (p2start - p2end);

            float cs1, cs2, ce1, ce2; //Couleur des vertex ( Shader uv en 1/u )

            float Ilum = r.div / r.Intensity;

            cs1 = (r.cos1 * (WaistPos.x - r.StartPosition1.x) + r.sin1 * (WaistPos.y - r.StartPosition1.y)) * Ilum;
            if (cs1 < 0) cs1 = -cs1;
            cs2 = (r.cos2 * (WaistPos.x - r.StartPosition2.x) + r.sin2 * (WaistPos.y - r.StartPosition2.y)) * Ilum;
            if (cs2 < 0) cs2 = -cs2;
            ce1 = (r.cos1 * (WaistPos.x - EndPosition1.x) + r.sin1 * (WaistPos.y - EndPosition1.y)) * Ilum;
            if (ce1 < 0) ce1 = -ce1;
            ce2 = (r.cos2 * (WaistPos.x - EndPosition2.x) + r.sin2 * (WaistPos.y - EndPosition2.y)) * Ilum;
            if (ce2 < 0) ce2 = -ce2;

            //cs1 = Vector3.Distance(StartPosition1,WaistPos) * div ; 
            //cs2 = Vector3.Distance(StartPosition2,WaistPos) * div; 
            //ce1 = Vector3.Distance(EndPosition1,WaistPos) * div; 
            //ce2 = Vector3.Distance(EndPosition2,WaistPos) * div;

            vertices = new Vector3[6] {
                r.StartPosition1,
                r.StartPosition2,
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

            float Ilum = r.div / r.Intensity;

            float p = p2start - p2end;
            if (p < 0) p = -p;

            if (p > LightRay.EPSILON)
            {
                Vector3 WaistPos = ((p2start - p1) * EndPosition2 - (p2end - p1) * r.StartPosition2) / (p2start - p2end);

                cs1 = (r.cos1 * (WaistPos.x - r.StartPosition1.x) + r.sin1 * (WaistPos.y - r.StartPosition1.y)) * Ilum;
                if (cs1 < 0) cs1 = -cs1;
                cs2 = (r.cos2 * (WaistPos.x - r.StartPosition2.x) + r.sin2 * (WaistPos.y - r.StartPosition2.y)) * Ilum;
                if (cs2 < 0) cs2 = -cs2;
                ce1 = (r.cos1 * (WaistPos.x - EndPosition1.x) + r.sin1 * (WaistPos.y - EndPosition1.y)) * Ilum;
                if (ce1 < 0) ce1 = -ce1;
                ce2 = (r.cos2 * (WaistPos.x - EndPosition2.x) + r.sin2 * (WaistPos.y - EndPosition2.y)) * Ilum;
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
                cs1 = cs2 = ce1 = ce2 = cc / r.Intensity;
            }

            vertices = new Vector3[6] {
                r.StartPosition1,
                r.StartPosition2,
                EndPosition1,
                EndPosition1,
                r.StartPosition2,
                EndPosition2

            };

            uv = new Vector2[6] {
                 new Vector2(cs1,0f),new Vector2(cs2,0f),new Vector2(ce1,0f),new Vector2(ce1,0f),new Vector2(cs2,0f),new Vector2(ce2,0f)
            };


        }

        Mesh m = r.GetComponent<MeshFilter>().mesh;

        m.vertices = vertices;
        m.uv = uv;

        r.GetComponent<MeshRenderer>().material.color = r.Col;

    }
}