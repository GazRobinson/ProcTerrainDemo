using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinTester : MonoBehaviour {
    Perlin perlin = new Perlin();
    public float arg = 0.1f;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
       // Debug.Log( perlin.noise1( arg ) );
    }
}
