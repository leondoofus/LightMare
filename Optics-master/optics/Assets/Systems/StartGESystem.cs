using UnityEngine;
using FYFY;

public class StartGESystem : FSystem {
    FYFYGameEngine GE = GameObject.Find("FYFYGameEngine").GetComponent<FYFYGameEngine>();

    // Use to process your families.
    protected override void onProcess(int familiesUpdateCount) {
        if (GE.levelLoaded)
        {
            GE.LightSources = Object.FindObjectsOfType<LightSource>();
            GE.OpticalComponents = Object.FindObjectsOfType<OpticalComponent>();
            GE.Targets = Object.FindObjectsOfType<Target>();
            GE.Rays = GameObject.Find("Rays").transform;
            Transform PlayGround = GameObject.Find("Playground").transform;

            foreach (LightSource ls in GE.LightSources)
            {
                ls.Rays = GE.Rays;
                ls.RaysReserve = GE.RaysReserve;
                ls.InitializeSource();
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
            GE.levelLoaded = false;
        }
        
	}
}