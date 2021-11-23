using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainMesh : MonoBehaviour {
    public bool continuous = false;
    public bool betterTris = false;
    private MeshFilter m_filter;
    private MeshRenderer m_renderer;
    private Mesh mesh;
    public List<Vector3> vert = new List<Vector3>();
    Vector2[] m_uv;
    public int[] tris;
    Vector3 line = Vector3.right;
    private int xWidth, yWidth;
    private float scale = 1.0f;
    Vector3 middle = new Vector3( 50f, 0f, 50f );
    float perlinOffset;
    public float stepTime = 0.1f;
    Perlin perlin = new Perlin();
    Material m_Mat;

    private void Awake() {
        m_renderer = GetComponent<MeshRenderer>();
        m_filter = GetComponent<MeshFilter>();
        perlinOffset = Random.value;
        m_Mat = m_renderer.material;
    }
    public float perlinFactor = 0.02134f;
    //Create the mesh
    public void InitilaiseMesh(int x, int y, float scale) {
        mesh = new Mesh();
        xWidth = x; yWidth = y;
        this.scale = scale;
        ResetMesh( x, y, scale );
       
        m_filter.mesh = mesh;
    }
    public void ResetMesh( ) {
        vert = CreateVertices( xWidth, yWidth, scale );
        mesh.SetVertices( vert );
        mesh.uv = m_uv;
        tris = betterTris? CreateBetterTriangles( xWidth, yWidth ) : CreateTriangles( xWidth, yWidth );
        mesh.triangles = tris;
        Vector3[] norms = new Vector3[( xWidth + 1 ) * ( yWidth + 1 )];
        for ( int i = 0; i < norms.Length; i++ ) {
            norms[i] = Vector3.up;
        }
        mesh.normals = norms;

    }
    public void ResetMesh(int x, int y, float scale) {
        vert = CreateVertices( x, y, scale );
        mesh.SetVertices( vert );
        mesh.uv = m_uv;
        tris = betterTris ? CreateBetterTriangles( xWidth, yWidth ) : CreateTriangles( xWidth, yWidth );
        mesh.triangles = tris;
        Vector3[] norms = new Vector3[( x + 1 ) * ( y + 1 )];
        for ( int i = 0; i < norms.Length; i++ ) {
            norms[i] = Vector3.up;
        }
        mesh.normals = norms;

    }

    private List<Vector3> CreateVertices( int xCount, int yCount, float scale ) {
        List<Vector3> verts = new List<Vector3>( ( xCount + 1 ) * ( yCount + 1 ) );
        Vector2[] uvs = new Vector2[( xCount + 1 ) * ( yCount + 1 )];
        int i = 0;
        float s = 1f / uvs.Length;
        for ( int y = 0; y < yCount + 1; y++ ) {
            for ( int x = 0; x < xCount + 1; x++ ) {
                verts.Add( new Vector3( x * scale, 0.0f, y * scale ) );
                uvs[i] = new Vector2( s * x, s * y );
                i++;
            }
        }
        m_uv = uvs;
        return verts;
    }


    public void Fault() {
        middle = new Vector3( Random.Range( 1f, xWidth - 1 ),0f, Random.Range( 1f, yWidth - 1f ) );
         line = Quaternion.AngleAxis( Random.Range( 0f, 360f ), Vector3.up ) * Vector3.right;
        int c = 0, u = 0, d = 0;
        Vector3 cross;
        for ( int i = 0; i < vert.Count; i++){
            Vector3 v = vert[i];
            cross = Vector3.Cross( line, v - middle );
            if ( cross.y > 0 ) {
                v += Vector3.up;
                u++;
            }
            else {
                d++;
                v -= Vector3.up;
            }
            vert[i] = v;
            c++;
        }
//        Debug.Log( c + ", " + u + ", " + d);
        mesh.SetVertices( vert );
        Rebuild();
    }

    public void Randomise( float amplitude ) {
        vert = new List<Vector3>( mesh.vertices );
        Vector3 v;
        int i;
        for ( int y = 0; y < yWidth + 1; y++ ) {
            for ( int x = 0; x < xWidth + 1; x++ ) {
                i = ( x + ( y * ( xWidth + 1 ) ) );
                v = vert[i];
                v.y = Random.Range( -amplitude*0.5f, amplitude*0.5f );
                vert[i] = v;
            }
        }
        mesh.SetVertices( vert );
        Rebuild();
    }

    private void Rebuild() {
        tris = betterTris ? CreateBetterTriangles( xWidth, yWidth ) : CreateTriangles( xWidth, yWidth );
        mesh.triangles = tris;
        mesh.RecalculateNormals();
    }
    float[,] heightMap;
    bool GoOn = false;
    public IEnumerator MidPointDisplacement(float amplitude) {
        float rand = 1f;
        
        int width = xWidth + 1;
        int height = yWidth + 1;
        heightMap = new float[width, height];
        int squareSize = width;
        int halfSize = (squareSize-1) / 2;
        Vector3 tempPos;
        ResetMesh();
        if ( width % 2 < 1 || height % 2 < 1 ) {
            Debug.LogError( "Tile count must be even!" );
            yield break;
           //return null;
        }
        //Set four corners
        heightMap[0, 0] = 0;
        heightMap[0, height-1] = 1;
        heightMap[width-1, 0] = 3;
        heightMap[width-1, height-1] = 5;

       /* heightMap[0, 0] = 1;
        heightMap[0, height - 1] = Random.Range( 0f, amplitude );
        heightMap[width - 1, 0] = Random.Range( 0f, amplitude );
        heightMap[width - 1, height - 1] = Random.Range( 0f, amplitude );*/
        // bool GoOn = false;
        while ( squareSize >= 3 ) {
            int segments = width / (squareSize-1);


            Debug.Log( "Full square: " + squareSize + ", Half: " + halfSize + ", Segments: " + segments);
            for ( int subY = 0; subY < segments; subY++ ) {
                for ( int subX = 0; subX < segments; subX++ ) {

                   // Debug.Log( "SubX: " + subX + ", SubY: " + subY);
                    //Diamond
                    Vector2Int tl = new Vector2Int( 0 + (subX*(squareSize-1)), 0 + ( subY * (squareSize -1)) );
                    Vector2Int mid = new Vector2Int( squareSize-1, squareSize-1 );
                    Vector2Int br = tl + mid;
                    mid = new Vector2Int( tl.x + halfSize, tl.y + halfSize );


                  //  Debug.Log( "TL: " + tl + ", Mid: " + mid + ", BR: " + br );

                    heightMap[mid.x, mid.y] = Average( heightMap[tl.x, tl.y], heightMap[br.x, tl.y], heightMap[tl.x, br.y], heightMap[br.x, br.y] ) + Random.Range( -0.5f, 0.5f ) * rand*amplitude;

                }
            }
            for ( int subY = 0; subY < segments; subY++ ) {
                for ( int subX = 0; subX < segments; subX++ ) {

                  //  Debug.Log( "SubX: " + subX + ", SubY: " + subY );
                    //Diamond
                    Vector2Int tl = new Vector2Int( 0 + ( subX * ( squareSize - 1 ) ), 0 + ( subY * ( squareSize - 1 ) ) );
                    Vector2Int mid = new Vector2Int( squareSize - 1, squareSize - 1 );
                    Vector2Int br = tl + mid;
                    mid = new Vector2Int( tl.x + halfSize, tl.y + halfSize );


                  //  Debug.Log( "TL: " + tl + ", Mid: " + mid + ", BR: " + br );

               

                    //Square
                    int px, py;

                    //Left
                    px = tl.x; py = mid.y;
                    Square( px, py, halfSize, Random.Range(-0.5f, 0.5f) * rand * amplitude  );
                    //Right
                    px = br.x; py = mid.y;
                    Square( px, py, halfSize, Random.Range( -0.5f, 0.5f ) * rand * amplitude );
                    //Up
                    px = mid.x; py = tl.y;
                    Square( px, py, halfSize, Random.Range( -0.5f, 0.5f ) * rand * amplitude );
                    //Down
                    px = mid.x; py = br.y;
                    Square( px, py, halfSize, Random.Range( -0.5f, 0.5f ) * rand * amplitude);
                }

            }


            //Send heigh data
            for ( int y = 0; y < height; y++ ) {
                for ( int x = 0; x < height; x++ ) {
                    tempPos = vert[x + width * y];
                    tempPos.y = heightMap[x, y];
                    vert[x + width * y] = tempPos;
                }
            }
            mesh.SetVertices( vert );
            Rebuild();
            yield return new WaitForSeconds( stepTime );
            squareSize = ( squareSize + 1 ) / 2;
            halfSize = ( squareSize - 1 ) / 2;

            rand *= 0.5f;
        }

        //Send heigh data
        for ( int y = 0; y < height; y++ ) {
            for ( int x = 0; x < height; x++ ) {
                tempPos = vert[x + width * y];
                tempPos.y = heightMap[x, y];
                vert[x + width * y] = tempPos;
            }
        }
        mesh.SetVertices( vert );
        Rebuild();
    }
    private float Average( params float[] input ) {
        float total = 0;
        foreach ( float f in input ) {
            total += f;
        }
        return total / input.Length;
    }
    private void Diamond() {

    }
    private float Square(int x, int y, int hSize, float rand) {
        float total = 0f; int i = 0;
        // List<float> heights = new List<float>();

            if ( x - hSize >= 0 ) {
                total += heightMap[x - hSize, y];
                i++;
            }
            if ( x + hSize < heightMap.GetLength( 0 ) ) {
                total += heightMap[x + hSize, y];
                i++;
            }
            if ( y - hSize >= 0 ) {
                total += heightMap[x, y - hSize];
                i++;
            }
            if ( y + hSize < heightMap.GetLength( 1 ) ) {
                total += heightMap[x, y + hSize];
                i++;
            }

        heightMap[x, y] = total / ( float )i;
        heightMap[x, y] += rand;
        return total / ( float )i;
    }

    public void Noise(float smoothness, float amplitude, bool additive = false) {
        vert = new List<Vector3>( mesh.vertices );
        Vector3 v;
        int i;
        float low = 5f;
        float h = 0f;
        for ( int y = 0; y < yWidth + 1; y++ ) {
            for ( int x = 0; x < xWidth + 1; x++ ) {
                i = ( x + ( y * ( xWidth + 1 ) ));
                v = vert[i];
                h = (0.5f * (1.0f + GetRawPerlinY(x, y, smoothness))) * amplitude;
                if ( h < low )
                    low = h;
                v.y = ( amplitude  ) + h;
                vert[i] = v;
            }
        }
        print( "LOW: " + low );
        mesh.SetVertices( vert );
        Rebuild();
    }
    public IEnumerator FBM( float frequency, float amplitude, int octaveCount, float pow = 2) {
        float freq = frequency;
        float amp = 1.0f;

        float maxAmp = 0;
        for (int j = 0; j < octaveCount; j++)
        {
            maxAmp += amp;
            amp *= ampReduction;
        }
        float maxHeight = amplitude * maxAmp;
        amp = 1.0f;
        m_Mat.SetFloat("_HeightRange", maxHeight * 0.6f);
        ResetMesh();
        vert = new List<Vector3>( mesh.vertices );
        Vector3 v;
        int i;

        /*for ( int y = 0; y < yWidth + 1; y++ ) {
            for ( int x = 0; x < xWidth + 1; x++ ) {
                i = ( x + ( y * ( xWidth + 1 ) ) );
                v = vert[i];
                v.y = amplitude;
               // vert[i] = v;
            }
        }*/
        for ( int y = 0; y < yWidth + 1; y++ ) {
            for ( int x = 0; x < xWidth + 1; x++ ) {

                amp = 1.0f;// amplitude;
                freq = frequency;
                i = (x + (y * (xWidth + 1)));
                v = vert[i];
                for ( int j = 0; j < octaveCount; j++ ) {
                    v.y+=( 0.5f * (1.0f + GetRawPerlinY( x, y, freq))) * amp;

                    amp *= ampReduction;
                    freq *= frequencyInc;
                }
                v.y /= maxAmp;
               // v.y += maxAmp * 0.5f;
                v.y = Mathf.Pow(v.y, pow);
                v.y *= amplitude;
                vert[i] = v;
            }
        }

        mesh.SetVertices( vert );
        Rebuild();
        yield return new WaitForSeconds( stepTime );
    }

    public IEnumerator Ridged( float frequency, float amplitude, int octaveCount, float pow = 2 ) {
        float freq = frequency;
        float amp = 1.0f;
        float maxAmp = 0;
        for (int j = 0; j < octaveCount; j++)
        {
            maxAmp += amp;
            amp *= ampReduction;
        }
        float maxHeight = amplitude;
        m_Mat.SetFloat("_HeightRange", maxHeight * 0.4f);
        amp = 1.0f;
        ResetMesh();
        vert = new List<Vector3>( mesh.vertices );
        Vector3 v;
        int i;
        /*for ( int y = 0; y < yWidth + 1; y++ ) {
            for ( int x = 0; x < xWidth + 1; x++ ) {
                i = ( x + ( y * ( xWidth + 1 ) ) );
                v = vert[i];
                v.y = amplitude;
                vert[i] = v;
            }
        }*/
        for ( int y = 0; y < yWidth + 1; y++ ) {
            for ( int x = 0; x < xWidth + 1; x++ ) {

                i = (x + (y * (xWidth + 1)));
                v = vert[i];

                for ( int j = 0; j < octaveCount; j++ ) {


                    v.y +=  (1.0f - Mathf.Abs(GetRawPerlinY( x, y, freq ))) * amp;
                   // v.y += GetPerlinY(x, y, freq);
                    amp *= ampReduction;
                    freq *= frequencyInc;
                }
                v.y /= maxAmp;
                v.y = Mathf.Pow(v.y, pow);
                v.y *= amplitude;
               // v.y += amplitude*0.5f;
                vert[i] = v;
                amp = 1.0f;
                freq = frequency;
            }
        }

        mesh.SetVertices( vert );
        Rebuild();
        yield return new WaitForSeconds( stepTime );
    }
    public float ampReduction = 0.5f;
    public float frequencyInc = 2.0f;
    private void Update() {
        if ( continuous ) {
            Noise( UIManager.frequency, UIManager.amp );
        }
    }
    public void Smooth() {
        List<Vector3> newVert = new List<Vector3>(  );
        int width = xWidth + 1;
        int height = yWidth + 1;
        for ( int y = 0; y < height; y++ ) {
            for ( int x = 0; x < width; x++ ) {
                int i = x + ( y *(width) );
              //  Debug.Log( x + ", " + y + "..  " + i );
                float yTotal = 0.0f;
                int adjacentSections = 0;

                if ( ( x - 1 ) > 0 ) // Check to left
                {
                    yTotal += vert[GetIndex(x-1, y)].y;
                    adjacentSections++;
                    if ( ( y - 1 ) > 0 ) // Check up and to the left
                    {
                        yTotal += vert[GetIndex( x - 1, y - 1 )].y;
                        adjacentSections++;
                    }

                    if ( ( y + 1 ) < height ) // Check down and to the left
                    {
                        yTotal += vert[GetIndex( x - 1, y + 1 )].y;
                        adjacentSections++;
                    }
                }
                if ( ( x + 1 ) < width ) // Check to right
                {
                    yTotal += vert[GetIndex( x + 1, y )].y;
                    adjacentSections++;
                    if ( ( y - 1 ) > 0 ) // Check up and to the right
                    {
                        yTotal += vert[GetIndex( x + 1, y - 1 )].y;
                        adjacentSections++;
                    }

                    if ( ( y + 1 ) < height) // Check down and to the right
                    {
                        yTotal += vert[GetIndex( x + 1, y + 1 )].y;
                        adjacentSections++;
                    }
                }

                if ( ( y - 1 ) > 0 ) // Check above
                {
                    yTotal += vert[GetIndex( x, y - 1)].y;
                    adjacentSections++;
                }

                if ( ( y + 1 ) < height ) // Check below
                {
                    yTotal += vert[GetIndex( x, y + 1 )].y;
                    adjacentSections++;
                }

                newVert.Add( new Vector3(vert[i].x,(vert[i].y + (yTotal/(float)adjacentSections))*0.5f, vert[i].z) );
            }
        }
        vert = new List<Vector3>( newVert );
        mesh.SetVertices( vert );
        Rebuild();
    }

    int GetIndex( int x, int y ) {
        return x + ( y * ( xWidth + 1 ) );
    }

    private int[] CreateTriangles(int xCount, int yCount) {
        //xCount++; yCount++;
        int xvCount = xCount + 1;
        int yvCount = yCount + 1;
        int[] triangles = new int[xCount * yCount * 6];
        int triCount = 0;
        for ( int y = 0; y < yCount; y++ ) {
            for ( int x = 0; x < xCount; x++ ) {
                triangles[triCount] = x + ( y * xvCount );
                triangles[triCount + 1] = triangles[triCount + 4] = x + ( ( y + 1 ) * xvCount );
                triangles[triCount + 2] = triangles[triCount + 3] = x + ( y * xvCount ) + 1;
                triangles[triCount + 5] = x + ( ( y + 1 ) * xvCount ) + 1;
                triCount += 6;
            }
        }
        return triangles;
    }
    private int[] CreateBetterTriangles( int xCount, int yCount ) {
        //xCount++; yCount++;
        int xvCount = xCount + 1;
        int yvCount = yCount + 1;
        int[] triangles = new int[xCount * yCount * 6];
        int triCount = 0;
        for ( int y = 0; y < yCount; y++ ) {
            for ( int x = 0; x < xCount; x++ ) {
                if ( y % 2 == 0 ) {
                    if ( x % 2 == 0 ) {
                        triangles[triCount] = triangles[triCount + 4] = x + ( y * xvCount );
                        triangles[triCount + 1] = x + ( ( y + 1 ) * xvCount );
                        triangles[triCount + 3] = x + ( y * xvCount ) + 1;
                        triangles[triCount + 2] = triangles[triCount + 5] = x + ( ( y + 1 ) * xvCount ) + 1;
                        triCount += 6;
                    }
                    else {
                        triangles[triCount] = x + ( y * xvCount );
                        triangles[triCount + 1] = triangles[triCount + 4] = x + ( ( y + 1 ) * xvCount );
                        triangles[triCount + 2] = triangles[triCount + 3] = x + ( y * xvCount ) + 1;
                        triangles[triCount + 5] = x + ( ( y + 1 ) * xvCount ) + 1;
                        triCount += 6;
                    }
                }
                else {
                    if ( x % 2 == 0 ) {
                        triangles[triCount] = x + ( y * xvCount );
                        triangles[triCount + 1] = triangles[triCount + 4] = x + ( ( y + 1 ) * xvCount );
                        triangles[triCount + 2] = triangles[triCount + 3] = x + ( y * xvCount ) + 1;
                        triangles[triCount + 5] = x + ( ( y + 1 ) * xvCount ) + 1;
                        triCount += 6;
                    }
                    else {

                        triangles[triCount] = triangles[triCount + 4] = x + ( y * xvCount );
                        triangles[triCount + 1] = x + ( ( y + 1 ) * xvCount );
                        triangles[triCount + 3] = x + ( y * xvCount ) + 1;
                        triangles[triCount + 2] = triangles[triCount + 5] = x + ( ( y + 1 ) * xvCount ) + 1;
                        triCount += 6;
                    }
                }
            }
        }
        return triangles;
    }

    private float GetPerlinY(int x, int y, float smoothness, float amplitude = 1.0f, float shift = 0.1337f) {
        float fx, fy;
        perlinFactor = smoothness;
        fx = (float)x; 
        fy = (float)y; 

        fx *= smoothness;
        fy *= smoothness;

        /*
        Vector2 vec = new Vector2( ( float )x * perlinFactor, ( float )y * perlinFactor );
        return (perlin.noise2( vec ) - 0.5f ) *amplitude;*/
        return Mathf.PerlinNoise((float)fx * perlinFactor, (float)fy * perlinFactor);
        return (( float )ImprovedNoise.noise( fx , fy , 0.0f ) ) * amplitude;
       //return ( ( Mathf.PerlinNoise( ( float )fx * perlinFactor , ( float )fy * perlinFactor ) - 0.5f ) * amplitude );
    }

    private float GetRawPerlinY(int x, int y, float smoothness)
    {
        float fx, fy;
        fx = (float)x * smoothness;
        fy = (float)y * smoothness;
        fx += UIManager.offset.x;
        fy += UIManager.offset.y;
        return (Mathf.PerlinNoise((float)fx * perlinFactor, (float)fy * perlinFactor) - 0.5f) * 2.0f;
        return ((float)ImprovedNoise.noise((float)fx * perlinFactor, (float)fy * perlinFactor, 0.0f));
    }

    public Vector3[] GetHeights(int dir) {
        Vector3[] pos = new Vector3[xWidth + 1];
        switch ( dir ) {
            case 0: //N
                for ( int x = 0; x < xWidth + 1; x++ ) {
                    pos[x] = vert[x];
                }
                break;
            case 1: //S
                for ( int x = 0; x < xWidth+1; x++ ) {
                    int p = (( xWidth + 1 ) * (yWidth+1)) - (x) -1;
                    pos[x] = vert[p];
                }
                break;
            case 2: //E
                for ( int y = 0; y < yWidth + 1; y++ ) {

                    int p = ( y * ( xWidth + 1 ) );
                    pos[yWidth - y] = vert[p];
                }
                break;
            case 3: //W
                for ( int y = 0; y < yWidth + 1; y++ ) {

                    int p = xWidth + ( y * ( xWidth + 1 ) );
                    pos[y] = vert[p];
                }
                break;
        }

        return pos;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        /*foreach ( Vector3 v in vert ) {
            Gizmos.DrawSphere( v, 0.1f );
        }*/
        Gizmos.color = Color.green;
        Gizmos.DrawLine( middle, middle + ( line * 100f ) );
        Gizmos.DrawLine( middle, middle - ( line * 100f ) );
    }
}
