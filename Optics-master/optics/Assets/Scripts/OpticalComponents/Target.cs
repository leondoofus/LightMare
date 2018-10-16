using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Target : OpticalComponent {

    public float CollectedIntensity;
    public float TargetIntensity;

    public GameObject Shine;
    public GameObject ScoreText;

    float score=0, scoreSpeed=0.5f;

    public void ResetTarget()
    {
        CollectedIntensity = 0;
    }

    override public float Collision2(LightRay lr)
    {
        float l1, l2;

        if ((lr.cos1 > 0 && x < lr.StartPosition1.x - radius) || (lr.cos1 < 0 && x > lr.StartPosition1.x + radius) ||
             (lr.sin1 > 0 && y < lr.StartPosition1.y - radius) || (lr.sin1 < 0 && y > lr.StartPosition1.y + radius)) 

            return -1;

        {

            l1 = -lr.sin1 * x + lr.cos1 * y - lr.param1;


            if (l1 > radius || l1 < -radius)
                return -1;

            l2 = -lr.sin2 * x + lr.cos2 * y - lr.param2;

            if (l2 > radius || l2 < -radius)
                return -1;

            float xo1 = lr.StartPosition1.x;
            float yo1 = lr.StartPosition1.y;
            return (x - xo1) * (x - xo1) + (y - yo1) * (y - yo1);
        }
       
    }

    override public void Deflect(LightRay r)
    {

        while (r.transform.childCount > 0)
            FreeLightRay(r.transform.GetChild(0).GetComponent<LightRay>());

        float xo1 = r.StartPosition1.x;
        float yo1 = r.StartPosition1.y;
        float xo2 = r.StartPosition2.x;
        float yo2 = r.StartPosition2.y;

        r.Length1 = Mathf.Sqrt((x - xo1) * (x - xo1) + (y - yo1) * (y - yo1));
        r.Length2 = Mathf.Sqrt((x - xo2) * (x - xo2) + (y - yo2) * (y - yo2));
       
    }


    override public void Update()
    {
        base.Update();
        Color c = Shine.GetComponent<Image>().color;
        float I =  Mathf.Clamp01((CollectedIntensity / TargetIntensity));

        if (score < I)
        {
            score += Time.deltaTime * scoreSpeed;
            if (score > I) score = I;
        }
        else if (score > I)
        {
            score -= Time.deltaTime * scoreSpeed;
            if (score < 0) score = 0;
        }

        c.a = Mathf.Sqrt(score) * (0.6f + 0.4f * Mathf.Cos(1.5f*Mathf.PI * Time.time));
        Shine.GetComponent<Image>().color = c;
        ScoreText.GetComponent<Text>().text = Mathf.RoundToInt(score * 100) + "%";
        ScoreText.GetComponent<Text>().fontSize= (int) (20+40*score);
    }


    public void ComputeScore()
    {
        CollectedIntensity = 0;
        foreach (LightRay lr in Rays.GetComponentsInChildren<LightRay>())
        {
            if (lr.End == this)
            {
                CollectedIntensity += lr.Intensity;
            }
        }
    }

}
