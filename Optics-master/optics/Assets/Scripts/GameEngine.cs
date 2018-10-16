using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Profiling;

public class GameEngine : MonoBehaviour {

    public LightSource[] LightSources;
    public OpticalComponent[] OpticalComponents;
    public Target[] Targets;
    public int NRaysMax=1000;
    public float LengthMax=15.0f;

    public Transform Rays;
    public Transform RaysReserve;
    public int DepthMax = 10;
    public bool running = false;
    public bool levelLoaded = false;


    void Start()
    {
        RaysReserve = GameObject.Find("RaysReserve").transform;  // find and deactivate
        RaysReserve.gameObject.SetActive(false);

        StartCoroutine("FillRaysReserve");
    }

    IEnumerator FillRaysReserve()
    {
        for (int i = 0; i < NRaysMax; i++)
        {
            int k = i;
            for (; i < k + 100; i++)
            {
                GameObject ray = new GameObject("Ray");
                ray.transform.SetParent(RaysReserve);
                ray.transform.localScale = Vector3.one;
                ray.transform.localPosition = Vector3.zero;

                LightRay r = ray.AddComponent<LightRay>();
                r.Initiliaze();
            }
            yield return null;
        }
        
    }

    public void StartGameEngine()
    {

        LightSources = FindObjectsOfType<LightSource>();
        OpticalComponents = FindObjectsOfType<OpticalComponent>();
        Targets = FindObjectsOfType<Target>();
        Rays = GameObject.Find("Rays").transform;
        Transform PlayGround = GameObject.Find("Playground").transform;

        foreach (LightSource ls in LightSources)
        {
            ls.Rays = Rays;
            ls.RaysReserve = RaysReserve;
            ls.InitializeSource();
            ls.PlayGround = PlayGround;
        }

        foreach (OpticalComponent op in OpticalComponents)
        {
            op.DepthMax = DepthMax;
            op.Rays = Rays;
            op.RaysReserve = RaysReserve;
            op.PlayGround = PlayGround;
        }

        running = true;
    }


    int i = 0;
    void Update() {
        //Profiler.BeginSample("MyPieceOfCode");

        if (!running)  return;
       

        //if (i++ == 1) { i = 0; } else return;

        bool update = false;

        foreach (LightSource ls in LightSources)
        {
            if (ls.hasChanged)
            {
                update = true;
                break;
            }
        }

        if (update)
        {
            UpdateAllRays();
        }

        foreach (OpticalComponent op in OpticalComponents)
        {
            if (op.hasChanged)
            {
                UpdateLightRays1OP(op);
                update = true;
            }
        }

        if (update) {
            foreach (Target t in Targets) t.ComputeScore();
        }

        //Profiler.EndSample();
    }

    private void UpdateAllRays()
    {
 
        foreach (LightSource ls in LightSources)
        {
            ls.EmitLight();
        }

        foreach (Transform t in Rays)
        {
            LightRay lr = t.GetComponent<LightRay>();
            Collision(lr);
            lr.Draw();
        }

        foreach (LightSource ls in LightSources) ls.hasChanged = false;
        foreach (OpticalComponent op in OpticalComponents) op.hasChanged = false;
        foreach (Target t in Targets) t.ComputeScore();
        
    }

    bool Collision(LightRay lr) {

        float lmin = -1;
        OpticalComponent opCollision = null;

        foreach (OpticalComponent op in OpticalComponents)
        {
            if (lr.Origin == op) continue;

            float l = op.Collision2(lr);

            if (l > 0 && ( l < lmin || lmin <0 ) ) // trouve la plus proche collision
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
                Collision(lchild.GetComponent<LightRay>());

        }
        else  // Si on touche personne
        {
            lr.Length1 = lr.Length2 = LengthMax;
            lr.End = null;

            // On retire tous les rayon enfants
            while (lr.transform.childCount > 0) // Attention le foreach ne marche pas car on change le nombre de child !
                ResetLightRay(lr.transform.GetChild(0).GetComponent<LightRay>());
            
        }

        return false;
    }

    public void ResetLightRay()
    {
      foreach (LightRay r in Rays.GetComponentsInChildren<LightRay>())
        {
            r.transform.parent = RaysReserve;
            r.Origin = r.End = null;
        }
    }

    private void UpdateLightRays1OP(OpticalComponent op)
    {
        foreach (Transform t in Rays)
        {
            LightRay lr = t.GetComponent<LightRay>();
            Update1LightRay1OP(lr,op);
        }

        op.hasChanged = false;

    }

    private void Update1LightRay1OP(LightRay lr, OpticalComponent op)
    {
        if (Collision1OP(lr, op)) // si nouvelle collision ou perte de collision
        {
            Collision(lr);
            lr.Draw();
        }
        else
        {
            foreach (Transform lchild in lr.transform)
            {
                Update1LightRay1OP(lchild.GetComponent<LightRay>(), op);
            }
        }
    }

    bool Collision1OP(LightRay lr,OpticalComponent op) // test la collision avec 1 optical component
    {
        if (lr.End == op) // si l'op touchait le rayon, on l'update
        {
            lr.Length1=LengthMax; // On redonne une long max pour le test Fast collision 
            return true;
        } 
        float l = op.Collision2(lr);
        if (l > 0) return true; // si l'op touche le rayon on l'update
        return false;
    }

    private void ResetLightRay(LightRay ray) // remove child recursively
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
            r.transform.parent = RaysReserve;
            r.End = null;
            r.Origin = null;
        }
    }

}
