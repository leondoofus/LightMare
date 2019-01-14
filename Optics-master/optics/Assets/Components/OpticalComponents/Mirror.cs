using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mirror : OpticalComponent {


    /*override public void Deflect(LightRay r)
    {

        float xo1 = r.StartPosition1.x;
        float yo1 = r.StartPosition1.y;
        float ao1 = r.Direction1;
        float xo2 = r.StartPosition2.x;
        float yo2 = r.StartPosition2.y;
        float ao2 = r.Direction2;

        r.Length1 = Mathf.Sqrt((xc1 - xo1) * (xc1 - xo1) + (yc1 - yo1) * (yc1 - yo1));
        r.Length2 = Mathf.Sqrt((xc2 - xo2) * (xc2 - xo2) + (yc2 - yo2) * (yc2 - yo2));

        LightRay lr=null;
        if (r.transform.childCount == 0)
            lr = NewRayLightChild(r);
        else if (r.transform.childCount == 1)
            lr = r.transform.GetChild(0).GetComponent<LightRay>();
        else
        {
            while (r.transform.childCount > 1)
                FreeLightRay(r.transform.GetChild(0).GetComponent<LightRay>());
            lr = r.transform.GetChild(0).GetComponent<LightRay>();
        }

        //Transform nextRay = r.transform.GetChild(0);
        //LightRay lr = nextRay.GetComponent<LightRay>();
        //LightRay lr = NewRayLightChild(r);
        if (lr == null) return;

        //lr.isVisible = true;
        //lr.gameObject.SetActive(true);

        lr.Col = r.Col;
        lr.Intensity = r.Intensity;
        //lr.Col.r = lr.Col.r * 0.2f;
        lr.StartPosition1 = new Vector3(xc1, yc1, 0);
        lr.StartPosition2 = new Vector3(xc2, yc2, 0);
        lr.Direction1 = ao1;
        lr.Direction2 = ao2;
        lr.Length1 = 15.0f;
        lr.Length2 = 15.0f;
        lr.Origin = this;

        // Pour un miroir
       
        lr.Direction1 = -ao1 + 2*angle;
        lr.Direction2 = -ao2 + 2*angle;
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
    }*/

}
