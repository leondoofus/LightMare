using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectWall : MonoBehaviour {

    public float width,height;
    public Transform Diag1, Diag2;
    const float EPSILON = 0.01f;

    void Start () {
        RectTransform rt = (RectTransform)transform;
        width = rt.rect.width;
        height = rt.rect.height;

        Transform W = Diag1;
        if (W)
        {
            W.localPosition = new Vector3(0, 0, 0);
            W.localRotation = Quaternion.Euler(0, 0, Mathf.Atan2(width,height) * 180 / Mathf.PI);
            W.GetComponent<OpticalComponent>().radius = Mathf.Sqrt(height*height+width*width)/2;
        }

        W = Diag2;
        if (W)
        {
            W.localPosition = new Vector3(0, 0, 0);
            W.localRotation = Quaternion.Euler(0, 0, -Mathf.Atan2(width,height) * 180 / Mathf.PI);
            W.GetComponent<OpticalComponent>().radius = Mathf.Sqrt(height*height+width*width)/2;
        }

    }

}
