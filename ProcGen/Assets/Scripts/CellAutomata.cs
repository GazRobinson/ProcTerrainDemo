using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellAutomata : MonoBehaviour {
    public int GridSize = 128;
    [Range( 0f, 1f )]
    public float seedThreshold = 0.5f;
    public Sprite sprite;
    private SpriteRenderer[,] cells;
    private int[,] cellData;
    private int state = 0;
	// Use this for initialization
	void Start () {
        GenerateTiles();
        Camera.main.orthographic = true;
        Camera.main.orthographicSize = GridSize / 2;
        Camera.main.transform.position = new Vector3( GridSize / 2, GridSize / 2, -5f );
	}
	
	// Update is called once per frame
	void Update () {
        if ( Input.GetKeyDown( KeyCode.Space ) ) {
            if ( state == 0 )
                GenerateInitialState();
            else {
                Iterate();
            }
            RenderCellData();
            state++;
        }
        if ( Input.GetKeyDown( KeyCode.R ) ) {
            state = 1;
            GenerateInitialState();
            RenderCellData();
        }
        
	}

    void GenerateTiles() {
        cells = new SpriteRenderer[GridSize, GridSize];
        SpriteRenderer prefab = new GameObject( "Cell", typeof( SpriteRenderer ), typeof( RectTransform ) ).GetComponent<SpriteRenderer>();
        prefab.sprite = sprite;
        for ( int y = 0; y < GridSize; y++ )
            for ( int x = 0; x < GridSize; x++ ) {
                cells[x, y] = Instantiate<SpriteRenderer>( prefab, transform );
                cells[x, y].GetComponent<RectTransform>().anchoredPosition = new Vector2( x, y );
            }
        Destroy( prefab.gameObject );
    }
    void GenerateInitialState() {
        cellData = new int[GridSize, GridSize];
        for ( int y = 0; y < GridSize; y++ )
            for ( int x = 0; x < GridSize; x++ ) {
                cellData[x, y] = Random.value > seedThreshold ? 1 : 0;
            }
    }
    void RenderCellData() {
        for ( int y = 0; y < GridSize; y++ )
            for ( int x = 0; x < GridSize; x++ ) {
                cells[x, y].color = cellData[x, y] == 1 ? Color.white : Color.black;
            }
    }

    void Iterate() {
        int[,] newData = new int[GridSize, GridSize];

        for ( int y = 0; y < GridSize; y++ )
            for ( int x = 0; x < GridSize; x++ ) {
                newData[x, y] = CountNeighbours( x, y ) > 4 ? 1 : 0;
            }
        cellData = (int[,])newData.Clone();
    }
    int CountNeighbours( int xP, int yP ) {
        int c = 0;
        for ( int y = yP - 1; y < yP + 2; y++ )
            for ( int x = xP - 1; x < xP + 2; x++ ) {
                if ( x < 0 || y < 0 || x >= GridSize || y >= GridSize )
                    continue;
                c += cellData[x, y];
            }

        return c;
    }
}
