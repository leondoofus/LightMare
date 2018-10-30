using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSource : MonoBehaviour
{

    // Use this for initialization

    const int MaxDepth = 10;

    public Transform Rays;
    public Transform RaysReserve;

    public int N = 50;
    public float Div = 0;
    public float Length = 15;
    public float radius = 0.6f;
    LightRay[] LightRays;
    public bool hasChanged = true;
    public Color Color = new Color(1, 1, 0.8f, 0.5f);
    public float Intensity = 1;
    public Transform PlayGround;


    private void Start()
    {
    }

    Vector3 OldPosition;
    Quaternion OldRotation;
    void Update()
    {
       
        LaunchStar();
        

        if (OldPosition == transform.localPosition && transform.localRotation == OldRotation)
            return;

        hasChanged = true;
        OldPosition = transform.localPosition;
        OldRotation = transform.localRotation;
        
    }

    public void InitializeSource()
    {
        LightRays = new LightRay[N];
        for (int i = 0; i < N; i++)
        {

            if (RaysReserve.childCount == 0) return; // Plus de rayons disponible !!


            // Preparation du rayon
            LightRay r = RaysReserve.GetChild(0).GetComponent<LightRay>();
            r.transform.parent = Rays;
            r.transform.localScale = Vector3.one;
            r.transform.position = Vector3.zero;
            r.Origin = null;
            r.depth = 0;

            LightRays[i]=r;
        }

    }

    public void EmitLight()
    {
        float angle = transform.localRotation.eulerAngles.z * 2 * Mathf.PI / 360;
        Vector3 pos = transform.localPosition;

        for (int i = 0; i < N; i++)
        {

            LightRay r = LightRays[i];

            // Couleur et intensité
            Color c = Color;
            c.a = c.a * (1 - (i + 0.5f - N / 2f) * (i + 0.5f - N / 2f) / (float)N / N * 4.0f);
            r.Col = c;
            r.Intensity = Intensity / N;

            // Calculs des positions et directions
            float l1 = -radius * (-0.5f + i / (float)N);
            float l2 = -radius * (-0.5f + (i + 1) / (float)N);

            r.StartPosition1 = pos + new Vector3(Mathf.Sin(angle) * l1, -Mathf.Cos(angle) * l1, 0);
            r.StartPosition2 = pos + new Vector3(Mathf.Sin(angle) * l2, -Mathf.Cos(angle) * l2, 0);

            r.Direction1 = Div * (-0.5f + i / (float)N) + angle;
            r.Direction2 = Div * (-0.5f + (i + 1) / (float)N) + angle;

            // Précalcul de paramètres géométriques utiles pour le calcul de collision
            r.ComputeDir();
        }

        

    }

    public void EmitLight2()
    {
        float angle = transform.localRotation.eulerAngles.z * 2 * Mathf.PI / 360;
        Vector3 pos = transform.localPosition;//+Random.Range(0,0.001f)*Vector3.one;

        int i = 0;
        foreach (LightRay r in LightRays)
        {
            float l1 = -radius * (-0.5f + i / (float)N);
            float l2 = -radius * (-0.5f + (i + 1) / (float)N);

            r.StartPosition1 = pos + new Vector3(Mathf.Sin(angle) * l1, -Mathf.Cos(angle) * l1, 0);
            r.StartPosition2 = pos + new Vector3(Mathf.Sin(angle) * l2, -Mathf.Cos(angle) * l2, 0);

            r.Direction1 = Div * (-0.5f + i / (float)N) + angle;
            r.Direction2 = Div * (-0.5f + (i + 1) / (float)N) + angle;

            r.Length1 = r.Length2 = Length;

            Color c = Color;
            c.a = c.a * (1 - (i + 0.5f - N / 2f) * (i + 0.5f - N / 2f) / (float)N / N * 4.0f);
            r.Col = c;
            r.Intensity = Intensity / N;

            r.ComputeDir();
            i++;
        }

    }

    void LaunchStar()
    {
        const float proba = 0.01f;

        if (Random.Range(0.0f, 1.0f) < proba)
        {
            GameObject Star = Instantiate(Resources.Load("Star", typeof(GameObject)) as GameObject);
            StarFollowRay sf = Star.GetComponent<StarFollowRay>();
            sf.Initialize(PlayGround, LightRays[Random.Range(0, N)]);
        }

    }

}
