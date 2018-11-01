using UnityEngine;
using FYFY;

public class GameEngineSystem : FSystem {
    FYFYGameEngine GE = GameObject.Find("FYFYGameEngine").GetComponent<FYFYGameEngine>();

    // Use to process your families.
    protected override void onProcess(int familiesUpdateCount) {
        Draw();
    }

    private void Draw()
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
            UpdateAllRays();
        }

        foreach (OpticalComponent op in GE.OpticalComponents)
        {
            if (op.hasChanged)
            {
                UpdateLightRays1OP(op);
                update = true;
            }
        }

        if (update)
        {
            foreach (Target t in GE.Targets) t.ComputeScore();
        }

        //Profiler.EndSample();
    }

    private void UpdateAllRays()
    {

        foreach (LightSource ls in GE.LightSources)
        {
            ls.EmitLight();
        }

        foreach (Transform t in GE.Rays)
        {
            LightRay lr = t.GetComponent<LightRay>();
            Collision(lr);
            lr.Draw();
        }

        foreach (LightSource ls in GE.LightSources) ls.hasChanged = false;
        foreach (OpticalComponent op in GE.OpticalComponents) op.hasChanged = false;
        foreach (Target t in GE.Targets) t.ComputeScore();

    }

    private bool Collision(LightRay lr)
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
                Collision(lchild.GetComponent<LightRay>());

        }
        else  // Si on touche personne
        {
            lr.Length1 = lr.Length2 = GE.LengthMax;
            lr.End = null;

            // On retire tous les rayon enfants
            while (lr.transform.childCount > 0) // Attention le foreach ne marche pas car on change le nombre de child !
                ResetLightRay(lr.transform.GetChild(0).GetComponent<LightRay>());

        }

        return false;
    }

    public void ResetLightRay()
    {
        foreach (LightRay r in GE.Rays.GetComponentsInChildren<LightRay>())
        {
            r.transform.parent = GE.RaysReserve;
            r.Origin = r.End = null;
        }
    }

    private void UpdateLightRays1OP(OpticalComponent op)
    {
        foreach (Transform t in GE.Rays)
        {
            LightRay lr = t.GetComponent<LightRay>();
            Update1LightRay1OP(lr, op);
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

    private bool Collision1OP(LightRay lr, OpticalComponent op) // test la collision avec 1 optical component
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
            r.transform.parent = GE.RaysReserve;
            r.End = null;
            r.Origin = null;
        }
    }
}