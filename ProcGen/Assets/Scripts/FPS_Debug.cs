using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FPS_Debug : MonoBehaviour {
    Text txt;
	// Use this for initialization
	void Start () {
        txt = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
        txt.text = ( 1f / Time.deltaTime ).ToString();
        if ( Input.GetKeyDown( KeyCode.Backspace ) )
            Application.Quit();
	}
}
