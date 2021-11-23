using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LSystemInfo {
    public string start = "A";
    public Rule[] rules;
    public int iterations = 3;
    [HideInInspector]
    public string currentSystem = "";

    public void Run() {
        currentSystem = start;
        for ( int i = 0; i < iterations; i++ ) {
            Iterate();
        }
    }

    public void Iterate() {
        string s = "";
        bool a;
       // string app;
        for ( int i = 0; i < currentSystem.Length; i++ ) {
            //  app = currentSystem[i];
            a = false;
            foreach ( Rule r in rules ) {
                if ( currentSystem[i] == r.lhs ) {
                   // app = r.rhs;
                    s += r.rhs;
                    a = true;
                    continue;
                }
            }
            if(!a)
                s += currentSystem[i];
            // s += app;
        }
        currentSystem = s;
        iterations++;
    }

}

[System.Serializable]
public class Rule {
    public char lhs = 'A';
    public string rhs = "AB";
}
public class LSystem : MonoBehaviour {
    public LSystemInfo systemInfo;
    // Draws a line from "startVertex" var to the curent mouse position.
    public Material mat;
    public Mesh cylinder;
    MeshRenderer meshRenderer;
    public Transform startVertex;
    public Transform mousePos;

    public

    void Start() {
        meshRenderer = GetComponent<MeshRenderer>();
       // startVertex = Vector3.zero;
    }

    void Update() {
        if ( Input.GetKeyDown( KeyCode.L ) ) {
            systemInfo.Run();
        }
        if ( Input.GetKeyDown( KeyCode.I ) ) {
            systemInfo.Iterate();
        }
    }
    void Draw() {

        if ( !mat ) {
            Debug.LogError( "Please Assign a material on the inspector" );
            return;
        }

        //   GL.LoadIdentity();
        Stack<Vector3> directionStack = new Stack<Vector3>();
        Stack<Vector3> posStack = new Stack<Vector3>();
        //  directionStack.Push( Vector3.up );


        Vector3 dir = Vector3.up;
        Vector3 pos = Vector3.zero;
        string sys = systemInfo.currentSystem;
        GL.Begin( GL.LINES );
        mat.SetPass( 0 );
        GL.Color( new Color( mat.color.r, mat.color.g, mat.color.b, mat.color.a ) );

        pos = startVertex.position;
        for ( int i = 0; i < sys.Length; i++ ) {
            //            s = posStack.Peek();
            Vector3 p = Vector3.zero;
            switch ( sys[i] ) {
                case 'A':
                    GL.Vertex3( pos.x, pos.y, pos.z );
                    pos = pos + dir*0.5f;
                    GL.Vertex3( pos.x, pos.y, pos.z );
                    break;
                case 'B':
                    GL.Vertex3( pos.x, pos.y, pos.z );
                    pos = pos + dir;
                    GL.Vertex3( pos.x, pos.y, pos.z );
                    break;
                case '[':
                    directionStack.Push( dir );
                    posStack.Push( pos );
                    dir = Quaternion.AngleAxis( Random.Range(35f,55f), Vector3.forward ) * dir;
                    //p = pos + dir;
                    break;
                case ']':
                    dir = directionStack.Pop();
                    pos = posStack.Pop();
                    dir = Quaternion.AngleAxis( -Random.Range( 35f, 55f ), Vector3.forward ) * dir;

                    break;
            }
        }

        GL.End();

    }
    void OnPostRender() {
       // Draw();
    }
}
