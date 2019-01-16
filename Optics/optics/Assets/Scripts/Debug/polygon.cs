using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class polygon : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Vector3[] theVertices = new Vector3[4];

        theVertices[0]=new Vector3(0, 0, 0);
        theVertices[1]=new Vector3(1, 0, 0);
        theVertices[2]=new Vector3(1, 1, 0);
        theVertices[3]=new Vector3(0, 1, 0);

        //DrawPoly(theVertices);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
