using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightController : MonoBehaviour {
    public Transform highlightCube;
    public float maxRange = 3.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        RaycastHit info;
        bool target = Physics.Raycast( transform.position, transform.forward, out info, maxRange );
        highlightCube.position = target ? info.transform.position : Vector3.one * -100f;
        if ( target ) {
            if ( Input.GetMouseButtonDown( 0 ) )
                TerrainManager3D.Instance.ChangeBlockState( Mathf.FloorToInt( highlightCube.position.x ), Mathf.FloorToInt( highlightCube.position.y ), Mathf.FloorToInt( highlightCube.position.z ), 0, info.collider.gameObject );
            if ( Input.GetMouseButtonDown( 1 ) ) {
                Vector3 pos = highlightCube.position + info.normal;
                TerrainManager3D.Instance.ChangeBlockState( Mathf.FloorToInt( pos.x ), Mathf.FloorToInt( pos.y ), Mathf.FloorToInt( pos.z ), 1, info.collider.gameObject );
            
            }
        }
    }
}
