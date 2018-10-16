using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarFollowRay : MonoBehaviour {

    public LightRay Ray;
    float Pos; // position on the Ray = lenght from startpos
    Color col;
    public float velocity=10f;
    Vector2 Direction;
    public float attenuation=0.02f;
    float intensity;

	public void Initialize(Transform Parent,LightRay lr) {   
        Pos = 0;
        transform.SetParent(Parent);
        Ray = lr;
        transform.localScale = Vector3.one;
        Vector3 SPos;
        SPos.x = Ray.StartPosition1.x;
        SPos.y = Ray.StartPosition1.y;
        SPos.z = 0;
        transform.localPosition = SPos;
        intensity = 1;
        SetColor(10.0f);
    }

    void SetColor()
    {
        Color c = GetComponent<Image>().color;
        GetComponent<Image>().color = c * (1 - attenuation);
    }

    void SetColor(float I)
    {
        GetComponent<Image>().color = Ray.Col * I;
    }

    void Update () {
        Pos += velocity * Time.deltaTime;

        if (Pos > Ray.Length1)
        {
            JumpOnChild();
        }
        else
        {
            Vector3 SPos;
            SPos.x = Ray.StartPosition1.x + Ray.cos1 * Pos;
            SPos.y = Ray.StartPosition1.y + Ray.sin1 * Pos;
            SPos.z = 0;
            transform.localPosition = SPos;
            SetColor();
        }
    }


    void JumpOnChild()
    {
        int NChild = Ray.transform.childCount;
        if (NChild > 0)
        {
            Ray = Ray.transform.GetChild(Random.Range(0,NChild)).GetComponent<LightRay>();

            Pos = 0;
        }
        else
        {
            GameObject.Destroy(gameObject);
        }
    }
}
