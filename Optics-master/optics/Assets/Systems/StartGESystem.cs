using UnityEngine;
using FYFY;

public class StartGESystem : FSystem {
    private Family _GE = FamilyManager.getFamily(new AllOfComponents(typeof(FYFYGameEngine)));

    public StartGESystem()
    {
        foreach (GameObject go in _GE)
            DrawRays(go);
        _GE.addEntryCallback(DrawRays);
    }
    
    private void DrawRays(GameObject go)
    {
        FYFYGameEngine GE = go.GetComponent<FYFYGameEngine>();
        GE.LightSources = Object.FindObjectsOfType<LightSource>();
        GE.OpticalComponents = Object.FindObjectsOfType<OpticalComponent>();
        GE.Targets = Object.FindObjectsOfType<Target>();
        GE.Rays = GameObject.Find("Rays").transform;
        Transform PlayGround = GameObject.Find("Playground").transform;

        foreach (LightSource ls in GE.LightSources)
        {
            ls.Rays = GE.Rays;
            ls.RaysReserve = GE.RaysReserve;
            //InitializeSource();
            ls.LightRays = new LightRay[ls.N];
            for (int i = 0; i < ls.N; i++)
            {

                if (ls.RaysReserve.childCount == 0) return; // Plus de rayons disponible !!


                // Preparation du rayon
                LightRay r = ls.RaysReserve.GetChild(0).GetComponent<LightRay>();
                r.transform.parent = ls.Rays;
                r.transform.localScale = Vector3.one;
                r.transform.position = Vector3.zero;
                r.Origin = null;
                r.depth = 0;

                ls.LightRays[i] = r;
            }
            ls.PlayGround = PlayGround;
        }

        foreach (OpticalComponent op in GE.OpticalComponents)
        {
            op.DepthMax = GE.DepthMax;
            op.Rays = GE.Rays;
            op.RaysReserve = GE.RaysReserve;
            op.PlayGround = PlayGround;
        }

        GE.running = true;
        
    }


    // Use to process your families.
    protected override void onProcess(int familiesUpdateCount) {
	}
}