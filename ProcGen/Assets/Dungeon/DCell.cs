using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DCell : MonoBehaviour {
    public Vector2 position;
    public Vector2 size;
    public Color color;
    public int[,] room;
  //  private SpriteRenderer spriteRenderer;
    public static Sprite sprite = null;

    public void Init( Vector2 pos, Vector2 s ) {
        // spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        room = new int[Mathf.FloorToInt( s.x ), Mathf.FloorToInt( s.y )];
        for ( int y = 0; y < s.y; y++ )
            for ( int x = 0; x < s.x; x++ )
                room[x, y] = 1;
                
        position = pos;
        size = s;
        color = new Color( Random.value, Random.value, Random.value );
        color.a = 1f;
        if ( sprite == null )
            sprite = Resources.Load<Sprite>( "Square" );
      //  spriteRenderer.sprite = sprite;
       // spriteRenderer.color = color;
        transform.localScale = s;

        transform.position = pos;
    }

    public float Left() {
        return position.x - size.x * 0.5f;
    }
    public float Right() {
        return position.x + size.x * 0.5f;
    }
    public float Top() {
        return position.y - size.y * 0.5f;
    }
    public float Bottom() {
        return position.y + size.y * 0.5f;
    }
}