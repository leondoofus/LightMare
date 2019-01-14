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
                Initiliaze(r);
                GameObjectManager.bind(ray);
            }
            yield return null;
        }
    }

    private void Initiliaze(LightRay r)
    {
        MeshFilter mf = r.gameObject.AddComponent<MeshFilter>();

        //Material mat = Resources.Load("Materials/Material Line", typeof(Material)) as Material;
        Material mat = Resources.Load("Materials/RayDiv", typeof(Material)) as Material;
        MeshRenderer mr = r.gameObject.AddComponent<MeshRenderer>();
        mr.material = mat;
        mr.material.color = r.Col;
        mr.sortingLayerName = "Rays";

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

        mf.mesh = m;
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
            foreach (Target t in GE.Targets) ComputeScore(t);
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
        foreach (Target t in GE.Targets) ComputeScore(t);

    }

    private bool Collision(FYFYGameEngine GE, LightRay lr)
    {

        float lmin = -1;
        OpticalComponent opCollision = null;

        foreach (OpticalComponent op in GE.OpticalComponents)
        {
            if (lr.Origin == op) continue;

            //float l = op.Collision2(lr);
            float l;
            if (op.GetType() == typeof(Target))
                l = Collision2Target(op, lr);
            else l = Collision2(op, lr);

            if (l > 0 && (l < lmin || lmin < 0)) // trouve la plus proche collision
            {
                lmin = l;
                opCollision = op;
            }
        }

        if (lmin > 0)
        {
            lr.End = opCollision;
            //opCollision.Deflect(lr);
            Deflect(opCollision, lr);

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
        //float l = op.Collision2(lr);
        float l;
        if (op.GetType() == typeof(Target))
            l = Collision2Target(op, lr);
        else l = Collision2(op, lr);
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

    private bool FastCollision(OpticalComponent oc, LightRay lr)
    {
        float p = -lr.sin1 * oc.x + lr.cos1 * oc.y;
        if (p > lr.param1 + oc.radius || p < lr.param1 - oc.radius)
            return false;

        p = lr.cos1 * (oc.x - lr.StartPosition1.x) + lr.sin1 * (oc.y - lr.StartPosition1.y);
        if (p < -oc.radius || p > lr.Length1 + oc.radius)
            return false;

        return true;
    }

    private float Collision(OpticalComponent oc, LightRay lr, int i)
    {
        float cosr, sinr, xr, yr, br;
        if (i == 1)
        {
            if (FastCollision(oc, lr) == false) return -1;  // Pas de collision
            cosr = lr.cos1;
            sinr = lr.sin1;
            xr = lr.StartPosition1.x;
            yr = lr.StartPosition1.y;
            br = lr.param1;
        }
        else
        {
            cosr = lr.cos2;
            sinr = lr.sin2;
            xr = lr.StartPosition2.x;
            yr = lr.StartPosition2.y;
            br = lr.param2;
        }

        float b = oc.param;

        float det = -cosr * oc.sin + sinr * oc.cos;

        if (det == 0) return -1;


        oc.xc = (cosr * b - oc.cos * br) / det;
        oc.yc = (sinr * b - oc.sin * br) / det;


        if ((cosr > 0 && oc.xc > xr) || (cosr < 0 && oc.xc < xr) || (sinr > 0 && oc.yc > yr) || (sinr < 0 && oc.yc < yr))
        {
            float r2 = (oc.xc - oc.x) * (oc.xc - oc.x) + (oc.yc - oc.y) * (oc.yc - oc.y);
            if (r2 < oc.radius * oc.radius)
                return (oc.xc - xr) * (oc.xc - xr) + (oc.yc - yr) * (oc.yc - yr);
        }
        return -1;
    }

    private float Collision2(OpticalComponent oc, LightRay lr)
    {
        float l1 = Collision(oc, lr, 1);
        oc.xc1 = oc.xc; oc.yc1 = oc.yc;
        if (l1 < 0) return -1;
        float l2 = Collision(oc,lr, 2);
        oc.xc2 = oc.xc; oc.yc2 = oc.yc;
        if (l2 < 0) return -1;

        return l1;
    }

    private float Collision2Target(OpticalComponent oc, LightRay lr)
    {
        float l1, l2;

        if ((lr.cos1 > 0 && oc.x < lr.StartPosition1.x - oc.radius) || (lr.cos1 < 0 && oc.x > lr.StartPosition1.x + oc.radius) ||
             (lr.sin1 > 0 && oc.y < lr.StartPosition1.y - oc.radius) || (lr.sin1 < 0 && oc.y > lr.StartPosition1.y + oc.radius))

            return -1;

        {

            l1 = -lr.sin1 * oc.x + lr.cos1 * oc.y - lr.param1;


            if (l1 > oc.radius || l1 < -oc.radius)
                return -1;

            l2 = -lr.sin2 * oc.x + lr.cos2 * oc.y - lr.param2;

            if (l2 > oc.radius || l2 < -oc.radius)
                return -1;

            float xo1 = lr.StartPosition1.x;
            float yo1 = lr.StartPosition1.y;
            return (oc.x - xo1) * (oc.x - xo1) + (oc.y - yo1) * (oc.y - yo1);
        }
    }

    private void Deflect(OpticalComponent oc, LightRay r) {
        if (oc.GetType() == typeof(Target))
        {
            while (r.transform.childCount > 0)
                FreeLightRay(oc, r.transform.GetChild(0).GetComponent<LightRay>());

            float xo1 = r.StartPosition1.x;
            float yo1 = r.StartPosition1.y;
            float xo2 = r.StartPosition2.x;
            float yo2 = r.StartPosition2.y;

            r.Length1 = Mathf.Sqrt((oc.x - xo1) * (oc.x - xo1) + (oc.y - yo1) * (oc.y - yo1));
            r.Length2 = Mathf.Sqrt((oc.x - xo2) * (oc.x - xo2) + (oc.y - yo2) * (oc.y - yo2));
            return;
        }
        if (oc.GetType() == typeof(Wall))
        {
            while (r.transform.childCount > 0)
                FreeLightRay(oc, r.transform.GetChild(0).GetComponent<LightRay>());

            float xo1 = r.StartPosition1.x;
            float yo1 = r.StartPosition1.y;
            float ao1 = r.Direction1;
            float xo2 = r.StartPosition2.x;
            float yo2 = r.StartPosition2.y;
            float ao2 = r.Direction2;

            r.Length1 = (oc.xc1 - xo1) * r.cos1 + (oc.yc1 - yo1) * r.sin1;
            r.Length2 = (oc.xc2 - xo2) * r.cos2 + (oc.yc2 - yo2) * r.sin2;
            return;
        }
        if (oc.GetType() == typeof(Lens))
        {
            float xo1 = r.StartPosition1.x;
            float yo1 = r.StartPosition1.y;
            float ao1 = r.Direction1;
            float xo2 = r.StartPosition2.x;
            float yo2 = r.StartPosition2.y;
            float ao2 = r.Direction2;

            r.Length1 = Mathf.Sqrt((oc.xc1 - xo1) * (oc.xc1 - xo1) + (oc.yc1 - yo1) * (oc.yc1 - yo1));
            r.Length2 = Mathf.Sqrt((oc.xc2 - xo2) * (oc.xc2 - xo2) + (oc.yc2 - yo2) * (oc.yc2 - yo2));

            LightRay lr = null;
            if (r.transform.childCount == 0)
                lr = NewRayLightChild(oc, r);
            else if (r.transform.childCount == 1)
                lr = r.transform.GetChild(0).GetComponent<LightRay>();
            else
            {
                while (r.transform.childCount > 1)
                    FreeLightRay(oc, r.transform.GetChild(0).GetComponent<LightRay>());
                lr = r.transform.GetChild(0).GetComponent<LightRay>();
            }

            if (lr == null) return;

            lr.Col = r.Col;
            lr.Intensity = r.Intensity;
            lr.StartPosition1 = new Vector3(oc.xc1, oc.yc1, 0);
            lr.StartPosition2 = new Vector3(oc.xc2, oc.yc2, 0);
            lr.Direction1 = ao1;
            lr.Direction2 = ao2;
            lr.Length1 = 15.0f;
            lr.Length2 = 15.0f;
            lr.Origin = oc;

            // Pour une lentille
            float zz1, theta1, thetaP1;
            float zz2, theta2, thetaP2;


            if (oc.cos > 0.7f || oc.cos < -0.7f)
            {
                zz1 = (oc.xc1 - oc.x) / oc.cos;
                zz2 = (oc.xc2 - oc.x) / oc.cos;
            }
            else
            {
                zz1 = (oc.yc1 - oc.y) / Mathf.Sin(oc.angle);
                zz2 = (oc.yc2 - oc.y) / Mathf.Sin(oc.angle);
            }
            theta1 = ao1 - (oc.angle - Mathf.PI / 2);
            theta2 = ao2 - (oc.angle - Mathf.PI / 2);

            if (Mathf.Cos(theta1) < 0)  // Backside
            {
                thetaP1 = Mathf.Atan(zz1 / ((Lens)oc).focal + Mathf.Tan(theta1)) + Mathf.PI; // le nouvel angle
                thetaP2 = Mathf.Atan(zz2 / ((Lens)oc).focal + Mathf.Tan(theta2)) + Mathf.PI; // le nouvel angle
            }
            else
            {
                thetaP1 = Mathf.Atan(-zz1 / ((Lens)oc).focal + Mathf.Tan(theta1)); // le nouvel angle
                thetaP2 = Mathf.Atan(-zz2 / ((Lens)oc).focal + Mathf.Tan(theta2)); // le nouvel angle
            }

            lr.Direction1 = thetaP1 + (oc.angle - Mathf.PI / 2);
            lr.Direction2 = thetaP2 + (oc.angle - Mathf.PI / 2);
            //lr.ComputeDir();
            lr.cos1 = Mathf.Cos(lr.Direction1);
            lr.sin1 = Mathf.Sin(lr.Direction1);
            lr.proj1 = lr.StartPosition1.x * lr.cos1 + lr.StartPosition1.y * lr.sin1;
            lr.param1 = -lr.StartPosition1.x * lr.sin1 + lr.StartPosition1.y * lr.cos1;
            lr.cos2 = Mathf.Cos(lr.Direction2);
            lr.sin2 = Mathf.Sin(lr.Direction2);
            lr.proj2 = lr.StartPosition2.x * lr.cos2 + lr.StartPosition2.y * lr.sin2;
            lr.param2 = -lr.StartPosition2.x * lr.sin2 + lr.StartPosition2.y * lr.cos2;

            lr.div = lr.Direction2 - lr.Direction1;
            if (lr.div < 0) lr.div = -lr.div;
            if (lr.div > 2 * Mathf.PI) lr.div -= 2 * Mathf.PI;
            return;
        }
        if (oc.GetType() == typeof(LameSemi))
        {
            float xo1 = r.StartPosition1.x;
            float yo1 = r.StartPosition1.y;
            float ao1 = r.Direction1;
            float xo2 = r.StartPosition2.x;
            float yo2 = r.StartPosition2.y;
            float ao2 = r.Direction2;

            r.Length1 = (oc.xc1 - xo1) * r.cos1 + (oc.yc1 - yo1) * r.sin1;
            r.Length2 = (oc.xc2 - xo2) * r.cos2 + (oc.yc2 - yo2) * r.sin2;

            LightRay lr = null;
            LightRay lt = null;
            if (r.transform.childCount == 0)
            {
                lr = NewRayLightChild(oc, r);
                lt = NewRayLightChild(oc, r);
            }
            else if (r.transform.childCount == 1)
            {
                lt = r.transform.GetChild(0).GetComponent<LightRay>();
                lr = NewRayLightChild(oc, r);
            }
            else if (r.transform.childCount == 2)
            {
                lt = r.transform.GetChild(0).GetComponent<LightRay>();
                lr = r.transform.GetChild(1).GetComponent<LightRay>();
            }
            else
            {
                while (r.transform.childCount > 2)
                    FreeLightRay(oc, r.transform.GetChild(0).GetComponent<LightRay>());

                lt = r.transform.GetChild(0).GetComponent<LightRay>();
                lr = r.transform.GetChild(1).GetComponent<LightRay>();
            }

            // Rayon transmis
            if (lt == null) return;

            lt.Col = r.Col;
            lt.Intensity = r.Intensity * (1 - ((LameSemi)oc).ReflectionCoef);
            lt.StartPosition1 = new Vector3(oc.xc1, oc.yc1, 0);
            lt.StartPosition2 = new Vector3(oc.xc2, oc.yc2, 0);
            lt.Direction1 = r.Direction1;
            lt.Direction2 = r.Direction2;
            lt.Origin = oc;
            lt.cos1 = Mathf.Cos(lt.Direction1);
            lt.sin1 = Mathf.Sin(lt.Direction1);
            lt.proj1 = lt.StartPosition1.x * lt.cos1 + lt.StartPosition1.y * lt.sin1;
            lt.param1 = -lt.StartPosition1.x * lt.sin1 + lt.StartPosition1.y * lt.cos1;
            lt.cos2 = Mathf.Cos(lt.Direction2);
            lt.sin2 = Mathf.Sin(lt.Direction2);
            lt.proj2 = lt.StartPosition2.x * lt.cos2 + lt.StartPosition2.y * lt.sin2;
            lt.param2 = -lt.StartPosition2.x * lt.sin2 + lt.StartPosition2.y * lt.cos2;

            lt.div = lt.Direction2 - lt.Direction1;
            if (lt.div < 0) lt.div = -lt.div;
            if (lt.div > 2 * Mathf.PI) lt.div -= 2 * Mathf.PI;


            //Rayon reflechi
            if (lr == null) return;

            lr.Col = r.Col;
            lr.Intensity = r.Intensity * ((LameSemi)oc).ReflectionCoef;
            lr.StartPosition1 = new Vector3(oc.xc1, oc.yc1, 0);
            lr.StartPosition2 = new Vector3(oc.xc2, oc.yc2, 0);
            lr.Direction1 = -ao1 + 2 * oc.angle;
            lr.Direction2 = -ao2 + 2 * oc.angle;
            lr.Origin = oc;
            lr.cos1 = Mathf.Cos(lr.Direction1);
            lr.sin1 = Mathf.Sin(lr.Direction1);
            lr.proj1 = lr.StartPosition1.x * lr.cos1 + lr.StartPosition1.y * lr.sin1;
            lr.param1 = -lr.StartPosition1.x * lr.sin1 + lr.StartPosition1.y * lr.cos1;
            lr.cos2 = Mathf.Cos(lr.Direction2);
            lr.sin2 = Mathf.Sin(lr.Direction2);
            lr.proj2 = lr.StartPosition2.x * lr.cos2 + lr.StartPosition2.y * lr.sin2;
            lr.param2 = -lr.StartPosition2.x * lr.sin2 + lr.StartPosition2.y * lr.cos2;

            lr.div = lr.Direction2 - lr.Direction1;
            if (lr.div < 0) lr.div = -lr.div;
            if (lr.div > 2 * Mathf.PI) lr.div -= 2 * Mathf.PI;
            return;
        }
        if (oc.GetType() == typeof(Mirror))
        {
            float xo1 = r.StartPosition1.x;
            float yo1 = r.StartPosition1.y;
            float ao1 = r.Direction1;
            float xo2 = r.StartPosition2.x;
            float yo2 = r.StartPosition2.y;
            float ao2 = r.Direction2;

            r.Length1 = Mathf.Sqrt((oc.xc1 - xo1) * (oc.xc1 - xo1) + (oc.yc1 - yo1) * (oc.yc1 - yo1));
            r.Length2 = Mathf.Sqrt((oc.xc2 - xo2) * (oc.xc2 - xo2) + (oc.yc2 - yo2) * (oc.yc2 - yo2));

            LightRay lr = null;
            if (r.transform.childCount == 0)
                lr = NewRayLightChild(oc,r);
            else if (r.transform.childCount == 1)
                lr = r.transform.GetChild(0).GetComponent<LightRay>();
            else
            {
                while (r.transform.childCount > 1)
                    FreeLightRay(oc, r.transform.GetChild(0).GetComponent<LightRay>());
                lr = r.transform.GetChild(0).GetComponent<LightRay>();
            }
            
            if (lr == null) return;
            

            lr.Col = r.Col;
            lr.Intensity = r.Intensity;
            lr.StartPosition1 = new Vector3(oc.xc1, oc.yc1, 0);
            lr.StartPosition2 = new Vector3(oc.xc2, oc.yc2, 0);
            lr.Direction1 = ao1;
            lr.Direction2 = ao2;
            lr.Length1 = 15.0f;
            lr.Length2 = 15.0f;
            lr.Origin = oc;

            // Pour un miroir

            lr.Direction1 = -ao1 + 2 * oc.angle;
            lr.Direction2 = -ao2 + 2 * oc.angle;
            //lr.ComputeDir();
            lr.cos1 = Mathf.Cos(lr.Direction1);
            lr.sin1 = Mathf.Sin(lr.Direction1);
            lr.proj1 = lr.StartPosition1.x * lr.cos1 + lr.StartPosition1.y * lr.sin1;
            lr.param1 = -lr.StartPosition1.x * lr.sin1 + lr.StartPosition1.y * lr.cos1;
            lr.cos2 = Mathf.Cos(lr.Direction2);
            lr.sin2 = Mathf.Sin(lr.Direction2);
            lr.proj2 = lr.StartPosition2.x * lr.cos2 + lr.StartPosition2.y * lr.sin2;
            lr.param2 = -lr.StartPosition2.x * lr.sin2 + lr.StartPosition2.y * lr.cos2;

            lr.div = lr.Direction2 - lr.Direction1;
            if (lr.div < 0) lr.div = -lr.div;
            if (lr.div > 2 * Mathf.PI) lr.div -= 2 * Mathf.PI;
            return;
        }
    }

    protected LightRay NewRayLightChild(OpticalComponent oc, LightRay lr)
    {
        if (lr.depth >= oc.DepthMax || oc.RaysReserve.childCount == 0) return null; // Plus de rayons disponible !!

        // Preparation du rayon
        LightRay r = oc.RaysReserve.GetChild(0).GetComponent<LightRay>();
        r.transform.parent = lr.transform;
        r.transform.localScale = Vector3.one;
        r.depth = lr.depth + 1;
        return r;
    }

    protected void FreeLightRay(OpticalComponent oc, LightRay ray) // remove child recursively
    {
        foreach (LightRay r in ray.GetComponentsInChildren<LightRay>())
        {
            r.transform.parent = oc.RaysReserve;
            r.End = null;
            r.Origin = null;
        }
    }

    private void ComputeScore(Target t)
    {
        t.CollectedIntensity = 0;
        foreach (LightRay lr in t.Rays.GetComponentsInChildren<LightRay>())
        {
            if (lr.End == t)
            {
                t.CollectedIntensity += lr.Intensity;
            }
        }
    }
}