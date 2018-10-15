using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Target : MonoBehaviour {
    // Advice: FYFY component aims to contain only public members (according to Entity-Component-System paradigm).
    public float CollectedIntensity=0;
    public float TargetIntensity;

    public GameObject Shine;
    public GameObject ScoreText;

    public float score = 0, scoreSpeed = 0.5f;
}