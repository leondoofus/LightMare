using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpticalComponent : MonoBehaviour {

    public float x=0,y=0; // position
    public float angle = 0;
    public float radius=0.5f;
    public float cos, sin, param; // vecteur directeur
    public bool hasChanged = true;
    public Transform Rays;
    public Transform RaysReserve;
    public int DepthMax = 10;
    public Transform PlayGround;


    Vector3 OldPosition;
    Quaternion OldRotation;
    virtual public void Update()
    {
        if (OldPosition != transform.position || transform.rotation != OldRotation)
        {
            ComputeDir();
            OldPosition = transform.position;
            OldRotation = transform.rotation;
            hasChanged = true;
        }
     }

    public bool FastCollision(LightRay lr)
    {
        float p = -lr.sin1 * x + lr.cos1 * y;
        if (p > lr.param1 + radius || p < lr.param1 - radius)
            return false;

        p = lr.cos1 * (x - lr.StartPosition1.x) + lr.sin1 * (y - lr.StartPosition1.y);
        if (p < -radius || p > lr.Length1 + radius) 
            return false;

        return true;
    }

    public void ComputeDir()
    {
        Vector3 pos = PlayGround.InverseTransformPoint(transform.position); // Position relative par rapport au playground
        x = pos.x;
        y = pos.y;

        angle = (transform.rotation.eulerAngles.z + 90) * Mathf.PI / 180f;
        cos = Mathf.Cos(angle);
        sin = Mathf.Sin(angle);
        param = -sin * x + cos * y;
    }


    float xc, yc;
    public float Collision(LightRay lr, int i)
    {
        float cosr, sinr, xr, yr, br;
        if (i == 1)
        {
            if (FastCollision(lr) == false) return -1;  // Pas de collision
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

        float b = param;

        float det = -cosr * sin + sinr * cos;

        if (det == 0) return -1;


        xc = (cosr * b - cos * br) / det;
        yc = (sinr * b - sin * br) / det;


        if ((cosr > 0 && xc > xr) || (cosr < 0 && xc < xr) || (sinr > 0 && yc > yr) || (sinr < 0 && yc < yr))
        {
            float r2 = (xc - x) * (xc - x) + (yc - y) * (yc - y);
            if (r2 < radius * radius)
                return (xc - xr) * (xc - xr) + (yc - yr) * (yc - yr);
        }
        return -1;
    }
    
    protected float xc1, yc1, xc2, yc2;
    virtual public float Collision2(LightRay lr)
    {
        float l1 = Collision(lr, 1);
        xc1 = xc; yc1 = yc;
        if (l1 < 0) return -1;
        float l2 = Collision(lr, 2);
        xc2 = xc; yc2 = yc;
        if (l2 < 0) return -1;

        return l1;
    }

    public virtual void Deflect(LightRay r) { }

    protected LightRay NewRayLightChild(LightRay lr)
    {
        if (lr.depth>=DepthMax || RaysReserve.childCount == 0) return null; // Plus de rayons disponible !!

        // Preparation du rayon
        LightRay r = RaysReserve.GetChild(0).GetComponent<LightRay>();
        r.transform.parent = lr.transform;
        r.transform.localScale = Vector3.one;
        r.depth = lr.depth+1;
        return r;
    }

    protected void FreeLightRay(LightRay ray) // remove child recursively
    {
        foreach (LightRay r in ray.GetComponentsInChildren<LightRay>())
        {
            r.transform.parent = RaysReserve;
            r.End = null;
            r.Origin = null;
        }
    }

}
