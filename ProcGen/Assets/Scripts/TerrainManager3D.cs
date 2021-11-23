using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class Chunk {
    public Chunk( int _x, int _y, int _z ) {
        cx = _x; cy = _y; cz = _z;
        densityMap = new int[16, 16, 16];
        renderMap = new int[16, 16, 16];
        typeMap = new int[16* 16* 16];
    }
    public int cx, cy, cz;
    public int[,,] densityMap;
    public int[,,] renderMap;
    public int[] typeMap;
    public bool hasBlock = true;
    public Matrix4x4[] mArray;
    public void CheckActive() {
        hasBlock = false;
        for (int z = 0; z < 16; z++)
            for (int y = 0; y < 16; y++)
                for (int x = 0; x < 16; x++)
                    if ( densityMap[x, y, z] > 0 ) {
                        hasBlock = true;
                        return;
                    }
    }
    public void BuildMatrixArray() {
        List<Matrix4x4> mList = new List<Matrix4x4>();
        Quaternion q = Quaternion.identity;
        Vector3 s = Vector3.one;
        for ( int z = 0; z < 16; z++ )
            for ( int y = 0; y < 16; y++ )
                for ( int x = 0; x < 16; x++ )
                    if ( renderMap[x, y, z] > 0 ) {
                        typeMap[x + y * 16 + z * 16 * 16] = 1;// UnityEngine.Random.Range(0, 5);
                        mList.Add( Matrix4x4.TRS( new Vector3(cx*16 + x, cy * 16 + y, cz * 16 + z ), q, s ) );
                    }
        mArray = mList.ToArray();
    }
}

/// <summary>
/// Terrain Manager 3D is the Voxel Terrain Manager
/// </summary>
public class TerrainManager3D : MonoBehaviour {
    public static TerrainManager3D Instance;
    public GameObject blockFab;
    public Vector3Int size = new Vector3Int(64, 16, 64);
    public float frequency2d = 0.01241f;
    public float frequency3d = 0.01241f;
    public float threshold = 0.5f;
    public float amplitude = 8f;
    Chunk[,,] chunks;
    int[,,] densityMap;
    int[,,] renderMap;
    List<Vector3> rMap = new List<Vector3>();
    Matrix4x4[] mMap;// = new List<Matrix4x4>();
    MaterialPropertyBlock[] mpbMap;// = new List<Matrix4x4>();
                     // GameObject[,,] objMap;
    Transform root;

    public Mesh cube;
    public Material cubeMat;
    public Material altMat;
    private List<Matrix4x4[]> bufferedData = new List<Matrix4x4[]>();
    private List<MaterialPropertyBlock> bufferedMPB = new List<MaterialPropertyBlock>();
    public UnityEngine.UI.Text debugTxt;
    private Transform cam;
    private Vector4[] cols = new Vector4[1023];
    private Vector4[] uvs = new Vector4[4];
    // Use this for initialization
    void Start () {
        Instance = this;
        root = new GameObject( "Root" ).transform;
        chunks = new Chunk[size.x, size.y, size.z];
        densityMap = new int[size.x, size.y, size.z];
        renderMap = new int[size.x, size.y, size.z];
        mainCam = Camera.main;
        cam = mainCam.transform;
        uvs[0] = new Vector4(0f, 0f, 0.5f, 0.5f);
        uvs[1] = new Vector4(0.5f, 0f, 0.5f, 0.5f);
        uvs[2] = new Vector4(0f, 0.5f, 0.5f, 0.5f);
        uvs[3] = new Vector4(0.5f, 0.5f, 0.5f, 0.5f);
        for ( int i = 0; i < cols.Length; i++ ) {
            int a = i % 3;
            if (a == 1)
            {
                cols[i] = new Vector4(1f, 0f, 0f, 1.0f);
            }

            if (a == 0)
            {
                cols[i] = new Vector4(0f, 1f, 0f, 1.0f);
            }
            if (a == 2)
            {
                cols[i] = new Vector4(0f, 0f, 1f, 1.0f);
            }
        }
        Build();
	}
	// Update is called once per frame
	void Update () {
         if ( Input.GetKeyDown( KeyCode.B ) )
             Build();

        UpdateRenderChunkStyle();
        BatchRender();
	}
    private void BatchRender() {
        Stopwatch sw = new Stopwatch();

        sw.Start();

        Vector3 vec = Vector3.zero;
        Quaternion q = Quaternion.identity;
        Vector3 s = Vector3.one;
        Matrix4x4[] tBuffer = new Matrix4x4[1023];
        bufferedData.Clear();
        int i = 0;
        int sx = size.x, sy = size.y, sz = size.x;

        int c = mMap.Length;
        i = 0;
        while(i < c)
        {
            int co = Mathf.Min( 1023, c - i );

            tBuffer = new Matrix4x4[co];
            Array.Copy( mMap, i, tBuffer, 0, co );
            bufferedData.Add( tBuffer );

            Vector4[] uvs2 = new Vector4[co];
            for(int u=0;u<uvs2.Length; u++)
            {
                uvs2[u] = uvs[2];
            }
            MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            mpb.SetVectorArray("_UV", uvs2);
            bufferedMPB.Add(mpb);
            i += co;
        }

        /* MaterialPropertyBlock block = new MaterialPropertyBlock();
         block.SetVectorArray( "_Color", cols );
         block.SetVectorArray("_UV", uvs);*/
        //Draw each batch
        int bindex = 0;
        foreach (Matrix4x4[] batch in bufferedData)
        {
            Graphics.DrawMeshInstanced(cube, 0, altMat, batch, batch.Length, bufferedMPB[bindex]);
            bindex++;
        }
        debugTxt.text = sw.ElapsedMilliseconds.ToString() + "ms for " + i.ToString() + " objects.";
        sw.Stop();
    }
    void Build() {
        //Build chunks
        for ( int z = 0; z < size.z; z++ )
            for ( int y = 0; y < size.y; y++ )
                for ( int x = 0; x < size.x; x++ ) {
                    chunks[x, y, z] = new Chunk( x, y, z );
                }
                    // amplitude = size.y;
                    //   objMap = new GameObject[size.x, size.y, size.z];
        int i = 0;
        int sx = size.x * 16, sy = size.y * 16, sz = size.z * 16;
        for ( int z = 0; z < sz; z++ )
            for ( int y = 0; y < sy; y++ )
                for ( int x = 0; x < sx; x++ ) {
                    i++;
                    chunks[x/16, y/16,z/16].densityMap[x%16, y%16, z%16] = ((ImprovedNoise.noise(x*frequency2d, z*frequency2d, 0.0f)*amplitude*0.25 + (amplitude*0.75f))> y && ImprovedNoise.noise( x * frequency3d, y * frequency3d, z * frequency3d ) > threshold) ?  1 : 0;
                }


        //Check chunks
        for ( int z = 0; z < size.z; z++ )
            for ( int y = 0; y < size.y; y++ )
                for ( int x = 0; x < size.x; x++ ) {
                    chunks[x, y, z].CheckActive();
                }

        //Check renderable blocks
        for ( int z = 0; z < size.z; z++ )
            for ( int y = 0; y < size.y; y++ )
                for ( int x = 0; x < size.x; x++ ) {

                    for ( int cz = 0; cz < 16; cz++ )
                        for ( int cy = 0; cy < 16; cy++ )
                            for ( int cx = 0; cx < 16; cx++ ) {
                                if ( chunks[x, y, z].densityMap[cx, cy, cz] > 0 )
                                    chunks[x, y, z].renderMap[cx, cy, cz] = CheckSurrondingCh( x * 16 + cx, y * 16 + cy, z * 16 + cz ) ? 1 : 0;
                                else
                                    chunks[x, y, z].renderMap[cx, cy, cz] = 0;
                            }
                    chunks[x, y, z].BuildMatrixArray();
                }
        print( i.ToString() + " total spaces." );
       // BuildRender();
        //UpdateRender();
    }
    void BuildRender() {
        if ( root.childCount > 0 ) {
            Destroy( root.GetChild( 0 ).gameObject );
        }
        Transform lvl = new GameObject( "Lvl" ).transform;
        lvl.SetParent( root );
        lvl.localPosition = Vector3.zero;
        lvl.localRotation = Quaternion.identity;
        lvl.SetAsFirstSibling();
        for ( int z = 0; z < size.z; z++ )
            for ( int y = 0; y < size.y; y++ )
                for ( int x = 0; x < size.x; x++ ) {
                    GameObject go = new GameObject("Block", typeof(BoxCollider));//Instantiate( blockFab, lvl );
                    go.transform.position = new Vector3( x, y, z );
                    go.transform.parent = lvl;
                   // objMap[x, y, z] = go;
                    if ( densityMap[x, y, z] == 1 ) {
                        if ( !CheckSurronding( x, y, z ) ) {
                            go.SetActive( false );
                        }
                    }
                    else {
                        go.SetActive( false );
                    }
                }

    }
    Camera mainCam;
    bool IsInFrustum( Vector3 p ) {
        Vector3 c = mainCam.WorldToViewportPoint( p );
        return ( c.z >= 0 && c.x >= 0 &&c.x <= 1 && c.y >= 0 && c.y <= 1  );
    }
    void UpdateRenderChunkStyle() {
        UpdateChunkRender();
        UpdateBlocksRender();
    }
    List<Chunk> visibleChunks = new List<Chunk>();
    List<Matrix4x4> mList = new List<Matrix4x4>();
    List<int> mpbList = new List<int>();
    void UpdateChunkRender() {
        cam.position -= cam.forward * 16f;
        int c = 0;
        visibleChunks.Clear();
        int sx = size.x, sy = size.y, sz = size.z;
        Vector3 f = cam.forward;
        Vector3 pos = cam.position;
        Vector3 cPos = Vector3.zero;
        for ( int z = 0; z < sz; z++ )
            for ( int y = 0; y < sy; y++ )
                for ( int x = 0; x < sx; x++ ) {
                    cPos = new Vector3( x * 16+8, y * 16 + 8, z * 16+8 );
                    if (chunks[x,y,z].hasBlock && IsInFrustum(cPos)/*Vector3.Dot( f, cPos - pos ) > 0.9 */)
                        visibleChunks.Add( chunks[x, y, z] );
                }
        cam.position += cam.forward * 16f;
        // print( visibleChunks.Count );
    }
    void UpdateBlocksRender() {
        mList.Clear();
        foreach ( Chunk c in visibleChunks ) {
            mList.AddRange( c.mArray );
            mpbList.AddRange(c.typeMap);
        }
        
        mMap = mList.ToArray();
    }
    void UpdateRender() {
        /*
        for ( int z = 0; z < size.z; z++ )
            for ( int y = 0; y < size.y; y++ )
                for ( int x = 0; x < size.x; x++ ) {
                    if ( densityMap[x, y, z] == 1 ) {
                        objMap[x, y, z].SetActive( CheckSurronding( x, y, z ) );
                    }
                    else {
                        objMap[x, y, z].SetActive( false );
                    }
                }
        */
        int i = 0;
        int sx = size.x, sy = size.y, sz = size.z;
        for ( int z = 0; z < sz; z++ )
            for ( int y = 0; y < sy; y++ )
                for ( int x = 0; x < sx; x++ ) {
                    if ( densityMap[x, y, z] == 1 ) {
                        if ( CheckSurronding( x, y, z ) ) {
                            renderMap[x, y, z] = 1; i++;
                        }
                        else {
                            renderMap[x, y, z] = 0;
                        }
                    }
                    else {
                        renderMap[x, y, z] = 0;
                    }
                    //objMap[x, y, z].SetActive( renderMap[x, y, z] > 0 );
                }
        print( i.ToString() + " total renderable." );
        UpdateRmap();
        UpdateMmap();
    }
    void UpdateRmap() {
        rMap.Clear();
        for ( int z = 0; z < size.z; z++ )
            for ( int y = 0; y < size.y; y++ )
                for ( int x = 0; x < size.x; x++ ) {
                    if(renderMap[x,y,z] > 0)
                            rMap.Add( new Vector3( x, y, z ) );
                }
    }
    void UpdateMmap() {
        mMap = new Matrix4x4[rMap.Count];
        Quaternion q = Quaternion.identity;
        Vector3 s = Vector3.one;
        int i = 0;
        for ( int z = 0; z < size.z; z++ )
            for ( int y = 0; y < size.y; y++ )
                for ( int x = 0; x < size.x; x++ ) {
                    if ( renderMap[x, y, z] > 0 ) {
                        mMap[i] = Matrix4x4.TRS( new Vector3( x, y, z ), q, s );
                        i++;
                    }
                }
    }
    void NiceUpdateRender(int _x, int _y, int _z) {
        /*
        for ( int z = _z-1; z < _z+2; z++ )
            for ( int y = _y-1; y < _y+2; y++ )
                for ( int x = _x-1; x < _x+2; x++ ) {
                    if ( densityMap[x, y, z] == 1 ) {
                        objMap[x, y, z].SetActive( CheckSurronding( x, y, z ) );
                    }
                    else {
                        objMap[x, y, z].SetActive( false );
                    }
                }
*/
        for ( int z = _z - 1; z < _z + 2; z++ )
            for ( int y = _y - 1; y < _y + 2; y++ )
                for ( int x = _x - 1; x < _x + 2; x++ ) {
                    if ( densityMap[x, y, z] == 1 ) {
                        if ( CheckSurronding( x, y, z ) ) {
                            renderMap[x, y, z] =  1;
                        }
                        else {
                            renderMap[x, y, z] = 0;
                        }
                    }
                    else {
                        renderMap[x, y, z] = 0;
                    }
                   // objMap[x, y, z].SetActive( renderMap[x,y,z]>0 );
                }
        UpdateRmap();
        UpdateMmap();
    }

    bool CheckSurrondingCh( int x, int y, int z ) {
        if ( x == 0 || x == size.x * 16 - 1 || y == 0 || y == size.y * 16 - 1 || z == 0 || z == size.z* 16 - 1 )
            return false; //Edges must not be rendered
        int cx = x % 16, cy = y % 16, cz = z % 16;
        if ( chunks[(x+1) / 16, y / 16, z / 16].densityMap[(x + 1)%16, cy, cz] > 0 )
        if ( chunks[( x - 1 ) / 16, y / 16, z / 16].densityMap[( x - 1 ) % 16, cy, cz] > 0 )
        if ( chunks[x / 16, (y-1) / 16, z / 16].densityMap[cx, ( y - 1 ) % 16, cz] > 0 )
        if ( chunks[x / 16, (y+1) / 16, z / 16].densityMap[cx, ( y + 1 ) % 16, cz] > 0 )
        if ( chunks[x / 16, y / 16, (z+1) / 16].densityMap[cx, cy, ( z - 1 ) % 16] > 0 )
        if ( chunks[x / 16, y / 16,( z+1) / 16].densityMap[cx, cy, ( z + 1 ) % 16] > 0 )
                                return false;
        return true;
    }
    bool CheckSurronding(int x, int y, int z) {
        if ( x == 0 || x == size.x - 1 || y == 0 || y == size.y - 1 || z == 0 || z == size.z - 1 )
            return false; //Edges must not be rendered

        if (densityMap[x+1,y,z] > 0)
            if ( densityMap[x - 1, y, z] > 0 )
                if ( densityMap[x, y-1, z] > 0 )
                    if ( densityMap[x , y+1, z] > 0 )
                        if ( densityMap[x, y, z-1] > 0 )
                            if ( densityMap[x, y, z+1] > 0 )
                                return false;
        return true;
    }
    void RenderPass() {

    }
    public void ChangeBlockState( int x, int y, int z, int state, GameObject blockObj ) {
       /* if ( state == 0 )
            Destroy( blockObj );
        if ( state == 1 ) {
            GameObject go = Instantiate( blockFab, root.GetChild(0) );
            go.transform.position = new Vector3( x, y, z );
        }*/
        densityMap[x, y, z] = state;
        NiceUpdateRender(x,y,z);
    }
}
