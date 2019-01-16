using UnityEngine;
using FYFY;
using UnityEngine.SceneManagement;

public class GameEngineSystem : FSystem {
    private Family _GE = FamilyManager.getFamily(new AllOfComponents(typeof(FYFYGameEngine)));

    public GameEngineSystem()
    {
        /*
        GameObject GOE = _GE.First();
        if (GOE != null)
        {
            FYFYGameEngine GE = GOE.GetComponent<FYFYGameEngine>();
            GE.RaysReserve = GameObject.Find("RaysReserve").transform;  // find and deactivate
            GE.RaysReserve.gameObject.SetActive(false);
            for (int i = 0; i < GE.NRaysMax; i++)
            {
                int k = i;
                for (; i < k + 100; i++)
                {
                    GameObject ray = new GameObject("Ray");
                    ray.transform.SetParent(GE.RaysReserve);
                    ray.transform.localScale = Vector3.one;
                    ray.transform.localPosition = Vector3.zero;

                    LightRay r = ray.AddComponent<LightRay>();
                    r.Initiliaze();
                }
            }
        }*/
        foreach(GameObject go in _GE)
            GameObjectManager.dontDestroyOnLoadAndRebind(go);
        if (SceneManager.GetActiveScene().name == "BaseLevel")
            SceneManager.LoadScene("MiniMap");
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
                r.ComputeDir();
            }
        }

        foreach (Transform t in GE.Rays)
        {
            LightRay lr = t.GetComponent<LightRay>();
            Collision(GE, lr);
            lr.Draw();
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
            lr.Draw();
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
}