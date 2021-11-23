using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneManager : MonoBehaviour {

    private MeshFilter m_filter;
    private Mesh mesh;
    private float m_UnitScale = 1.0f;

    private Vector3[,] Verts;
    private Vector2[] m_uv;
    private int[] tris;
    private Vector2Int vSize;
	// Use this for initialization
	void Start () {
        m_filter = GetComponent<MeshFilter>();
        InitilaiseMesh( 129, 129, 1f );
    }
    public float[,] GetHeightMap( ) {
        float[,] h = new float[vSize.x, vSize.y];
        for ( int y = 0; y < vSize.y; y++ ) {
            for ( int x = 0; x < vSize.x; x++ ) {
                h[x, y] = Verts[x, y].y;
            }
        }
        return h;
    }
    public void SetHeightMap( float[,] h ) {
        if ( h.GetLength( 0 ) != Verts.GetLength( 0 ) || h.GetLength( 1 ) != Verts.GetLength( 1 ) ) {
            Debug.LogError( "Trying to set a height map of a different size!" );
        }
        for ( int y = 0; y < vSize.y; y++ ) {
            for ( int x = 0; x < vSize.x; x++ ) {
                Verts[x, y].y = h[x,y];
            }
        }
        SetMesh();
    }
    //Create the mesh
    public void InitilaiseMesh( int x, int y, float scale ) {
        mesh = new Mesh();
        m_filter.mesh = mesh;
        m_UnitScale = scale;
        vSize = new Vector2Int( x, y );
        Verts = new Vector3[x, y];

        ResetMesh( );

    }
    public void ResetMesh() {
        Verts = CreateVertices( );
        tris = CreateBetterTriangles( );

        SetMesh();

    }
    private void SetMesh() {
        Vector3[] tVerts = new Vector3[Verts.Length];
        for ( int i = 0; i < Verts.Length; i++ ) {
            tVerts[i] = Verts[i % vSize.x, i / vSize.x];
        }
        mesh.vertices = tVerts;
        mesh.uv = m_uv;
        mesh.triangles = tris;
        mesh.RecalculateNormals();
    }

    private Vector3[,] CreateVertices( ) {
        Vector3[,] vertices = new Vector3[vSize.x, vSize.y];
        m_uv = new Vector2[( vSize.x ) * ( vSize.y  )];
        int i = 0;
        float s = 1f / m_uv.Length;
        for ( int y = 0; y < vSize.y; y++ ) {
            for ( int x = 0; x < vSize.x; x++ ) {
                vertices[x,y] = new Vector3( x * m_UnitScale, 0.0f, y * m_UnitScale );
                m_uv[i] = new Vector2( s * x, s * y );
                i++;
            }
        }
        return vertices;
    }

    private int[] CreateBetterTriangles( ) {
        int xCount = vSize.x, yCount = vSize.y;
        int[] triangles = new int[(xCount-1) * (yCount-1) * 6];
        int triCount = 0;
        for ( int y = 0; y < yCount-1; y++ ) {
            for ( int x = 0; x < xCount-1; x++ ) {
                if ( y % 2 == 0 ) {
                    if ( x % 2 == 0 ) {
                        triangles[triCount] = triangles[triCount + 4] = x + ( y * xCount );
                        triangles[triCount + 1] = x + ( ( y + 1 ) * xCount );
                        triangles[triCount + 3] = x + ( y * xCount ) + 1;
                        triangles[triCount + 2] = triangles[triCount + 5] = x + ( ( y + 1 ) * xCount ) + 1;
                        triCount += 6;
                    }
                    else {
                        triangles[triCount] = x + ( y * xCount );
                        triangles[triCount + 1] = triangles[triCount + 4] = x + ( ( y + 1 ) * xCount );
                        triangles[triCount + 2] = triangles[triCount + 3] = x + ( y * xCount ) + 1;
                        triangles[triCount + 5] = x + ( ( y + 1 ) * xCount ) + 1;
                        triCount += 6;
                    }
                }
                else {
                    if ( x % 2 == 0 ) {
                        triangles[triCount] = x + ( y * xCount );
                        triangles[triCount + 1] = triangles[triCount + 4] = x + ( ( y + 1 ) * xCount );
                        triangles[triCount + 2] = triangles[triCount + 3] = x + ( y * xCount ) + 1;
                        triangles[triCount + 5] = x + ( ( y + 1 ) * xCount ) + 1;
                        triCount += 6;
                    }
                    else {

                        triangles[triCount] = triangles[triCount + 4] = x + ( y * xCount );
                        triangles[triCount + 1] = x + ( ( y + 1 ) * xCount );
                        triangles[triCount + 3] = x + ( y * xCount ) + 1;
                        triangles[triCount + 2] = triangles[triCount + 5] = x + ( ( y + 1 ) * xCount ) + 1;
                        triCount += 6;
                    }
                }
            }
        }
        return triangles;
    }
}
