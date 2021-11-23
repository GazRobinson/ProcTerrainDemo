using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterManager : MonoBehaviour {
    private PlaneManager plane;
    Vector2 offset = Vector2.zero;
    public float freq = 0.01214f;
    public float amp = 5.0f;
    public float speed = 10.0f;
    public float height = 5.0f;
	// Use this for initialization
	void Start () {
        plane = GetComponent<PlaneManager>();
	}
	
	// Update is called once per frame
	void Update () {
        float[,] h = plane.GetHeightMap();
        offset.x += speed * Time.deltaTime;
        offset.y = Mathf.Sin( speed * Time.time );
        for ( int y = 0; y < h.GetLength( 1 ); y++ ) {
            for ( int x = 0; x < h.GetLength( 0 ); x++ ) {
                h[x,y] = height + ( ( float )ImprovedNoise.noise( ( float )(x+offset.x) * freq, ( float )( y + offset.y ) * freq, 0.0f ) ) * amp;
            }
        }
        plane.SetHeightMap( h );
	}
}
