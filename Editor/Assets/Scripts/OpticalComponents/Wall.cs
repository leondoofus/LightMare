using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : OpticalComponent {

    override public void Deflect(LightRay r)
    {

        while (r.transform.childCount > 0)
            FreeLightRay(r.transform.GetChild(0).GetComponent<LightRay>());

        float xo1 = r.StartPosition1.x;
        float yo1 = r.StartPosition1.y;
        float ao1 = r.Direction1;
        float xo2 = r.StartPosition2.x;
        float yo2 = r.StartPosition2.y;
        float ao2 = r.Direction2;

        r.Length1 = (xc1 - xo1) * r.cos1 + (yc1 - yo1) * r.sin1;
        r.Length2 = (xc2 - xo2) * r.cos2 + (yc2 - yo2) * r.sin2;

    }

}
