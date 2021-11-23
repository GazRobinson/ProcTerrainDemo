using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxMover : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Mathf.PerlinNoise
        transform.position = Vector3.zero + Vector3.up * Mathf.Sin(Time.time);
    }
}
