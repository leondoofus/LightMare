using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lens : OpticalComponent {
    public float focal=1.0f;

    override public void Deflect(LightRay r)
    {

        float xo1 = r.StartPosition1.x;
        float yo1 = r.StartPosition1.y;
        float ao1 = r.Direction1;
        float xo2 = r.StartPosition2.x;
        float yo2 = r.StartPosition2.y;
        float ao2 = r.Direction2;

        r.Length1 = Mathf.Sqrt((xc1 - xo1) * (xc1 - xo1) + (yc1 - yo1) * (yc1 - yo1));
        r.Length2 = Mathf.Sqrt((xc2 - xo2) * (xc2 - xo2) + (yc2 - yo2) * (yc2 - yo2));

        LightRay lr = null;
        if (r.transform.childCount == 0)
            lr = NewRayLightChild(r);
        else if (r.transform.childCount == 1)
            lr = r.transform.GetChild(0).GetComponent<LightRay>();
        else {
            while (r.transform.childCount > 1)
                FreeLightRay(r.transform.GetChild(0).GetComponent<LightRay>());
            lr = r.transform.GetChild(0).GetComponent<LightRay>();
        }

        if (lr == null) return;

        lr.Col = r.Col;
        lr.Intensity = r.Intensity;
        lr.StartPosition1 = new Vector3(xc1, yc1, 0);
        lr.StartPosition2 = new Vector3(xc2, yc2, 0);
        lr.Direction1 = ao1;
        lr.Direction2 = ao2;
        lr.Length1 = 15.0f;
        lr.Length2 = 15.0f;
        lr.Origin = this;

        // Pour une lentille
        float zz1, theta1, thetaP1;
        float zz2, theta2, thetaP2;


        if (cos > 0.7f || cos < -0.7f)
        {
            zz1 = (xc1 - x) / cos;
            zz2 = (xc2 - x) / cos;
        }
        else
        {
            zz1 = (yc1 - y) / Mathf.Sin(angle);
            zz2 = (yc2 - y) / Mathf.Sin(angle);
        }
        theta1 = ao1 - (angle - Mathf.PI / 2);
        theta2 = ao2 - (angle - Mathf.PI / 2);

        if (Mathf.Cos(theta1) < 0)  // Backside
        {
            thetaP1 = Mathf.Atan(zz1 / focal + Mathf.Tan(theta1)) + Mathf.PI; // le nouvel angle
            thetaP2 = Mathf.Atan(zz2 / focal + Mathf.Tan(theta2)) + Mathf.PI; // le nouvel angle
        }
        else
        {
            thetaP1 = Mathf.Atan(-zz1 / focal + Mathf.Tan(theta1)); // le nouvel angle
            thetaP2 = Mathf.Atan(-zz2 / focal + Mathf.Tan(theta2)); // le nouvel angle
        }

        lr.Direction1 = thetaP1 + (angle - Mathf.PI / 2);
        lr.Direction2 = thetaP2 + (angle - Mathf.PI / 2);
        lr.ComputeDir();
    }
}
