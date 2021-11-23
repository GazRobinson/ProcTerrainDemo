using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoomType
{
    SQUARE,
    CELL_AUTOMATA
}

public class DungeonMaster : MonoBehaviour {
    public Vector2Int maxSize = new Vector2Int( 1024, 512 );
    public List<DCell> cellList = new List<DCell>();
    public SpriteRenderer background = null;

    public Material effSprite;
    public Sprite sprite;
    private SpriteRenderer[,] cells;

    public Vector2 fullSize = new Vector2( 16f, 9f );

    public RoomType r_Type = RoomType.CELL_AUTOMATA;

    DCell CreateCell() {
        GameObject go = new GameObject( "Cell", typeof( DCell ) );
        go.transform.parent = transform;
        return go.GetComponent<DCell>();
    }

    void GenerateTiles() {
        cells = new SpriteRenderer[maxSize.x, maxSize.y];
        SpriteRenderer prefab = new GameObject( "Cell", typeof( SpriteRenderer ), typeof( RectTransform ) ).GetComponent<SpriteRenderer>();
        prefab.sharedMaterial = effSprite;
        prefab.sprite = sprite;
        for ( int y = 0; y < maxSize.y; y++ )
            for ( int x = 0; x < maxSize.x; x++ ) {
                cells[x, y] = Instantiate<SpriteRenderer>( prefab, transform );
                cells[x, y].GetComponent<RectTransform>().anchoredPosition = new Vector2( x + 0.5f, y + 0.5f );
            }
        Destroy( prefab.gameObject );
    }

    // Use this for initialization
    void Start () {
        fullSize = maxSize;
        background.transform.localScale = new Vector3( fullSize.x, fullSize.y, 1.0f );
        Camera.main.orthographic = true;
        Camera.main.orthographicSize = maxSize.y * (9f/16f);
        Camera.main.transform.position = new Vector3( maxSize.x / 2, maxSize.y / 2, -10f );

        background.transform.position = new Vector3( maxSize.x / 2, maxSize.y / 2, 0f );

        GenerateTiles();

        cellList.Add( CreateCell() );
        cellList[0].Init( new Vector2(maxSize.x/2, maxSize.y/2), fullSize );
        UpdateRender();
	}

    DCell[] SplitCell( DCell cell ) {
        cellList.Remove( cell );
        DCell[] returnArray = new DCell[4];
        Vector2 hSize = cell.size * 0.5f;
        Vector2 qSize = hSize * 0.5f;
        Vector2 p = cell.position - qSize;
        for ( int i = 0; i < 4; i++ ) {
            DCell newCell = CreateCell();
            newCell.Init( new Vector2( p.x + ( i % 2 ) * hSize.x, p.y + ( i / 2 ) * hSize.y ), hSize );
            returnArray[i] = newCell;
        }
        Destroy( cell.gameObject );
        return returnArray;
    }

    void UpdateRender() {
        foreach ( DCell cell in cellList ) {
            Vector2Int tl = Vector2Int.FloorToInt( cell.position ) -( Vector2Int.FloorToInt( cell.size /2));
            Vector2Int s = Vector2Int.FloorToInt( cell.size );
            for ( int y = tl.y; y < ( tl.y + s.y ); y++ )
                for ( int x = tl.x; x < ( tl.x + s.x ); x++ ) {
                    int cx = x - tl.x;
                    int cy = y - tl.y;
                    cells[x, y].color = cell.room[cx,cy] == 1 ? cell.color : Color.black;
                }
        }
    }

    void MakeRooms() {
        for(int i=0; i < cellList.Count; i++) {
            DCell cell = cellList[i];
            if ( r_Type == RoomType.CELL_AUTOMATA )
                DoCA( cell );
            else if (r_Type == RoomType.SQUARE) {
                SquareRooms( ref cell );
            }
        }
    }
    void DoCA(DCell cell) {
        int[,] rData;// = new int[Mathf.FloorToInt(cell.size.x), Mathf.FloorToInt(cell.size.y)];
        GenerateInitialRoomState( out rData, Mathf.FloorToInt( cell.size.x ), Mathf.FloorToInt( cell.size.y ) );
        for ( int i = 0; i < 4; i++ ) {
            Iterate( ref rData );
        }
        for ( int i = 0; i < 3; i++ ) {
            IterateB( ref rData );
        }
        cell.room = rData.Clone() as int[,];
    }
    void SquareRooms(ref DCell cell) {
        for ( int j = 0; j < cell.room.GetLength( 1 ); j++ )
            for ( int i = 0; i < cell.room.GetLength( 0 ); i++ ) {
                cell.room[i, j] =  1; //1 is a wall
            }

        int maxW, maxH;
        maxW = cell.room.GetLength( 0 ) - 2;
        maxH = cell.room.GetLength( 1 ) - 2;

        int w = Random.Range( Mathf.Max(2,maxW/2), maxW );
        int h = Random.Range( Mathf.Max( 2, maxH / 2 ), maxH );

        int x = Random.Range( 1, maxW - w );
        int y = Random.Range( 1, maxH - h );
        print( "W: " + w + ", H: " + h + ", X: " + x + ", Y:" + y );
        for ( int _y = y; _y < y+h; _y++ ) {
            for ( int _x = x; _x < x+w; _x++ ) {
                cell.room[_x, _y] = 0;
            }
        }
    }

    void GenerateInitialRoomState(out int[,] room, int sx, int sy ) {
        room = new int[sx, sy];
        for ( int y = 0; y < room.GetLength(1); y++ )
            for ( int x = 0; x < room.GetLength( 0 ); x++ ) {
                room[x, y] = Random.value < 0.45f ? 1 : 0; //1 is a wall
            }
    }


    void Iterate(ref int[,] room) {
        int[,] newData = new int[room.GetLength(0), room.GetLength( 1 )];

        for ( int y = 0; y < room.GetLength( 1 ); y++ )
            for ( int x = 0; x < room.GetLength( 0 ); x++ ) {
                newData[x, y] = (CountNeighbours( room, x, y ) > 4 || CountNeighbours(room,x,y,2) < 2) ? 1 : 0;
            }
        room = ( int[,] )newData.Clone();
    }
    void IterateB( ref int[,] room ) {
        int[,] newData = new int[room.GetLength( 0 ), room.GetLength( 1 )];

        for ( int y = 0; y < room.GetLength( 1 ); y++ )
            for ( int x = 0; x < room.GetLength( 0 ); x++ ) {
                newData[x, y] =  CountNeighbours( room, x, y ) > 4  ? 1 : 0;
            }
        room = ( int[,] )newData.Clone();
    }
    int CountNeighbours( int[,] map, int xP, int yP, int r = 1 ) {
        int c = 0;
        for ( int y = yP - r; y < yP + 1 + r; y++ )
            for ( int x = xP - r; x < xP + 1 + r; x++ ) {
                if ( x < 0 || y < 0 || x >= map.GetLength(0) || y >= map.GetLength( 1 ) )
                    c++;
                else
                    c += map[x, y];
            }

        return c;
    }

    DCell GetCellFromPoint(Vector2 p) {
        foreach ( DCell cell in cellList ) {
            if ( p.x < cell.Left() ) continue;
            if ( p.x > cell.Right() ) continue;
            if ( p.y > cell.Bottom() ) continue;
            if ( p.y < cell.Top() ) continue;
            return cell;
        }
        return null;
    }

    // Update is called once per frame
    void Update () {
        if ( Input.GetMouseButtonDown(0) ) {
            DCell c = GetCellFromPoint( Camera.main.ScreenToWorldPoint(Input.mousePosition) );
            if ( c != null )
                cellList.AddRange( SplitCell( c ) );
            UpdateRender();
        }
        if ( Input.GetKeyDown( KeyCode.Space ) ) {
            int i = Random.Range( 0, cellList.Count );
            cellList.AddRange( SplitCell( cellList[i] ) );
            UpdateRender();
        }
        if ( Input.GetKeyDown( KeyCode.G ) ) {
            MakeRooms();
            UpdateRender();
        }
    }
}
