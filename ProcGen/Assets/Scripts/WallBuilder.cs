using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallBuilder : MonoBehaviour {
    Mesh mesh;
	// Use this for initialization
	void Start () {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
	}
	

    public void BuildWall( Vector3[] heights ) {
        List<Vector3> verts = new List<Vector3>();
        Vector2[] uvs = new Vector2[heights.Length * 2];
        float s = 1f / uvs.Length;
        float maxF = 1f;
        for ( int i = 0; i < heights.Length; i++ ) {
            if ( heights[i].y > maxF )
                maxF = heights[i].y;
        }
        for ( int x = 0; x < heights.Length; x++ ) {
            Vector3 h = heights[x];
            verts.Add( new Vector3( h.x, 0.0f, h.z ) );
            verts.Add( new Vector3( h.x, h.y, h.z ) );
            uvs[x*2] = new Vector2( s * (float)x, 0.0f );
            uvs[x * 2+1] = new Vector2( s * (float)x, h.y / maxF );
        }
        mesh.SetVertices( verts );
        mesh.uv = uvs;
        int xvCount = heights.Length;
        int[] triangles = new int[(xvCount-1)*6];
        int triCount = triangles.Length;
        triCount = 0;
        for ( int x = 0; x < (xvCount-1); x++ ) {
            triangles[triCount] = x*2;
            triangles[triCount + 1] = x*2+1;
            triangles[triCount + 2] = x*2 + 2;
            triangles[triCount + 3] = x * 2 + 1;
            triangles[triCount + 4] = x * 2 + 3;
            triangles[triCount + 5] = x * 2 + 2;
            triCount += 6;
        }
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

}
