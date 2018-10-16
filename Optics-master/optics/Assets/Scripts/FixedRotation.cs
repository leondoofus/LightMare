using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedRotation : MonoBehaviour {

    public bool rotating = true;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (rotating)
            transform.rotation = Quaternion.Euler(0, 0, Time.time * 36);
        else
            transform.rotation = Quaternion.identity;

    }
}
