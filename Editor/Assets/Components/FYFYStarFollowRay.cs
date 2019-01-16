using UnityEngine;
using UnityEngine.UI;

public class FYFYStarFollowRay : MonoBehaviour {
    // Advice: FYFY component aims to contain only public members (according to Entity-Component-System paradigm).
    public LightRay Ray;
    public float Pos; // position on the Ray = lenght from startpos
    public Color col;
    public float velocity = 10f;
    public Vector2 Direction;
    public float attenuation = 0.02f;
    public float intensity;
}