using UnityEngine;
using FYFY;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameEngineSystem : FSystem {
    private Family _GE = FamilyManager.getFamily(new AllOfComponents(typeof(FYFYGameEngine)));
    private Family _mainLoop = FamilyManager.getFamily(new AllOfComponents(typeof(MainLoop)));

    public GameEngineSystem()
    {
        if (SceneManager.GetActiveScene().name == "BaseLevel")
        {
            foreach (GameObject go in _GE)
            {
                FYFYGameEngine ge = go.GetComponent<FYFYGameEngine>();
                ge.RaysReserve = GameObject.Find("RaysReserve").transform;  // find and deactivate
                ge.RaysReserve.gameObject.SetActive(false);

                _mainLoop.First().GetComponent<MainLoop>().StartCoroutine(FillRaysReserve(ge));

                ge.coroutineStarted = true;

                GameObjectManager.dontDestroyOnLoadAndRebind(go);
            }
            SceneManager.LoadScene("MiniMap");
        }
        else
            foreach (GameObject go in _GE)
            {
                FYFYGameEngine ge = go.GetComponent<FYFYGameEngine>();
                _mainLoop.First().GetComponent<MainLoop>().StartCoroutine(FillRaysReserve(ge));
                
            }
    }

    private IEnumerator FillRaysReserve(FYFYGameEngine ge)
    {
        for (int i = 0; i < ge.NRaysMax; i++)
        {
            int k = i;
            for (; i < k + 100; i++)
            {
                GameObject ray = new GameObject("Ray");
                ray.transform.SetParent(ge.RaysReserve);
                ray.transform.localScale = Vector3.one;
                ray.transform.localPosition = Vector3.zero;

                LightRay r = ray.AddComponent<LightRay>();
                r.Initiliaze();
                GameObjectManager.bind(ray);
            }
            yield return null;
        }
    }

    // Use to process your families.
    protected override void onProcess(int familiesUpdateCount) {
        GameObject GE = _GE.First();
        if (GE != null)
        {
            Draw(GE.GetComponent<FYFYGameEngine>());
        }
            
    }

    private void Draw(FYFYGameEngine GE)
    {
        //Profiler.BeginSample("MyPieceOfCode");

        if (!GE.running) return;


        //if (i++ == 1) { i = 0; } else return;

        bool update = false;

        foreach (LightSource ls in GE.LightSources)
        {
            if (ls.hasChanged)
            {
                update = true;
                break;
            }
        }

        if (update)
        {
            UpdateAllRays(GE);
        }

        foreach (OpticalComponent op in GE.OpticalComponents)
        {
            if (op.hasChanged)
            {
                UpdateLightRays1OP(GE, op);
                update = true;
            }
        }

        if (update)
        {
            foreach (Target t in GE.Targets) t.ComputeScore();
        }

        //Profiler.EndSample();
    }

    private void UpdateAllRays(FYFYGameEngine GE)
    {

        foreach (LightSource ls in GE.LightSources)
        {
            //ls.EmitLight();
            float angle = ls.transform.localRotation.eulerAngles.z * 2 * Mathf.PI / 360;
            Vector3 pos = ls.transform.localPosition;

            for (int i = 0; i < ls.N; i++)
            {

                LightRay r = ls.LightRays[i];

                // Couleur et intensité
                Color c = ls.Color;
                c.a = c.a * (1 - (i + 0.5f - ls.N / 2f) * (i + 0.5f - ls.N / 2f) / (float)ls.N / ls.N * 4.0f);
                r.Col = c;
                r.Intensity = ls.Intensity / ls.N;

                // Calculs des positions et directions
                float l1 = -ls.radius * (-0.5f + i / (float)ls.N);
                float l2 = -ls.radius * (-0.5f + (i + 1) / (float)ls.N);

                r.StartPosition1 = pos + new Vector3(Mathf.Sin(angle) * l1, -Mathf.Cos(angle) * l1, 0);
                r.StartPosition2 = pos + new Vector3(Mathf.Sin(angle) * l2, -Mathf.Cos(angle) * l2, 0);

                r.Direction1 = ls.Div * (-0.5f + i / (float)ls.N) + angle;
                r.Direction2 = ls.Div * (-0.5f + (i + 1) / (float)ls.N) + angle;

                // Précalcul de paramètres géométriques utiles pour le calcul de collision
                //r.ComputeDir();
                r.cos1 = Mathf.Cos(r.Direction1);
                r.sin1 = Mathf.Sin(r.Direction1);
                r.proj1 = r.StartPosition1.x * r.cos1 + r.StartPosition1.y * r.sin1;
                r.param1 = -r.StartPosition1.x * r.sin1 + r.StartPosition1.y * r.cos1;
                r.cos2 = Mathf.Cos(r.Direction2);
                r.sin2 = Mathf.Sin(r.Direction2);
                r.proj2 = r.StartPosition2.x * r.cos2 + r.StartPosition2.y * r.sin2;
                r.param2 = -r.StartPosition2.x * r.sin2 + r.StartPosition2.y * r.cos2;

                r.div = r.Direction2 - r.Direction1;
                if (r.div < 0) r.div = -r.div;
                if (r.div > 2 * Mathf.PI) r.div -= 2 * Mathf.PI;
            }
        }

        foreach (Transform t in GE.Rays)
        {
            LightRay lr = t.GetComponent<LightRay>();
            Collision(GE, lr);
            Draw(lr);
        }

        foreach (LightSource ls in GE.LightSources) ls.hasChanged = false;
        foreach (OpticalComponent op in GE.OpticalComponents) op.hasChanged = false;
        foreach (Target t in GE.Targets) t.ComputeScore();

    }

    private bool Collision(FYFYGameEngine GE, LightRay lr)
    {

        float lmin = -1;
        OpticalComponent opCollision = null;

        foreach (OpticalComponent op in GE.OpticalComponents)
        {
            if (lr.Origin == op) continue;

            float l = op.Collision2(lr);

            if (l > 0 && (l < lmin || lmin < 0)) // trouve la plus proche collision
            {
                lmin = l;
                opCollision = op;
            }
        }

        if (lmin > 0)
        {
            lr.End = opCollision;
            opCollision.Deflect(lr);

            foreach (Transform lchild in lr.transform)
                Collision(GE, lchild.GetComponent<LightRay>());

        }
        else  // Si on touche personne
        {
            lr.Length1 = lr.Length2 = GE.LengthMax;
            lr.End = null;

            // On retire tous les rayon enfants
            while (lr.transform.childCount > 0) // Attention le foreach ne marche pas car on change le nombre de child !
                ResetLightRay(GE, lr.transform.GetChild(0).GetComponent<LightRay>());

        }

        return false;
    }

    private void ResetLightRay(FYFYGameEngine GE)
    {
        foreach (LightRay r in GE.Rays.GetComponentsInChildren<LightRay>())
        {
            r.transform.parent = GE.RaysReserve;
            r.Origin = r.End = null;
        }
    }

    private void UpdateLightRays1OP(FYFYGameEngine GE, OpticalComponent op)
    {
        foreach (Transform t in GE.Rays)
        {
            LightRay lr = t.GetComponent<LightRay>();
            Update1LightRay1OP(GE, lr, op);
        }

        op.hasChanged = false;

    }

    private void Update1LightRay1OP(FYFYGameEngine GE, LightRay lr, OpticalComponent op)
    {
        if (Collision1OP(GE, lr, op)) // si nouvelle collision ou perte de collision
        {
            Collision(GE, lr);
            Draw(lr);
        }
        else
        {
            foreach (Transform lchild in lr.transform)
            {
                Update1LightRay1OP(GE, lchild.GetComponent<LightRay>(), op);
            }
        }
    }

    private bool Collision1OP(FYFYGameEngine GE, LightRay lr, OpticalComponent op) // test la collision avec 1 optical component
    {
        if (lr.End == op) // si l'op touchait le rayon, on l'update
        {
            lr.Length1 = GE.LengthMax; // On redonne une long max pour le test Fast collision 
            return true;
        }
        float l = op.Collision2(lr);
        if (l > 0) return true; // si l'op touche le rayon on l'update
        return false;
    }

    private void ResetLightRay(FYFYGameEngine GE, LightRay ray) // remove child recursively
    {

        /*while (ray.transform.childCount > 0) // Attention le foreach ne marche pas car on change le nombre de child !
        {
            ResetLightRay(ray.transform.GetChild(0).GetComponent<LightRay>());
        }
        ray.transform.parent = RaysReserve;
        ray.End = null;
        ray.Origin = null;*/


        foreach (LightRay r in ray.GetComponentsInChildren<LightRay>())
        {
            r.transform.parent = GE.RaysReserve;
            r.End = null;
            r.Origin = null;
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