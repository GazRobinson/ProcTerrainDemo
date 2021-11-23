using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainManager : MonoBehaviour {

    public Vector2Int terrainPolySize = new Vector2Int( 100, 100 );
    public float tileUnitSize = 1.0f;
    public WallBuilder n_Wall, s_Wall, e_Wall, w_Wall;
    private TerrainMesh tMesh;
	// Use this for initialization
	void Start () {
        tMesh = GetComponentInChildren<TerrainMesh>();
        tMesh.InitilaiseMesh( terrainPolySize.x, terrainPolySize.y, tileUnitSize);
        CamControl.Instance.SetFocus( new Vector3((terrainPolySize.x/2 * tileUnitSize),0.0f, ( terrainPolySize.y / 2 )*tileUnitSize ));
	}
	
	// Update is called once per frame
	void Update () {
        if ( Input.GetKeyDown( KeyCode.R ) ) {
            ResetTerrain();
        }
        if ( Input.GetKeyDown( KeyCode.P ) ) {
           // DoNoise();
        }
        if ( Input.GetKeyDown( KeyCode.F ) ) {
            DoFault();
        }
        if ( Input.GetKeyDown( KeyCode.S ) ) {
            DoSmooth();
        }
        n_Wall.BuildWall( tMesh.GetHeights(0) );
        s_Wall.BuildWall( tMesh.GetHeights(1) );
        e_Wall.BuildWall( tMesh.GetHeights(2) );
        w_Wall.BuildWall( tMesh.GetHeights(3) );
    }

    public void ResetTerrain() {
        tMesh.ResetMesh( terrainPolySize.x, terrainPolySize.y, 1.0f );
    }

    public void DoNoise(float smoothness, float amplitude) {
        tMesh.Noise(smoothness, amplitude);
    }
    public void DoFBM( float frequency, float amplitude , int octave, float pow) {
        StartCoroutine(tMesh.FBM( frequency, amplitude, octave, pow));
    }
    public void DoRidge( float frequency, float amplitude, int octave, float pow ) {
        StartCoroutine( tMesh.Ridged( frequency, amplitude, octave, pow ) );
    }
    public void DoSmooth() {
        tMesh.Smooth();
    }
    public void DoFault() {
        tMesh.Fault();
    }
    public void DoMidPoint( float amplitude ) {
        StartCoroutine(tMesh.MidPointDisplacement( amplitude));
    }
    public void Randomise(float amplitude) {
        tMesh.Randomise( amplitude );
    }
}
