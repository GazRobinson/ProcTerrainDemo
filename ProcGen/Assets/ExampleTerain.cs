using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleTerain : MonoBehaviour
{
    private MeshFilter m_Filter = null;
    private MeshRenderer m_Renderer = null;
    private Mesh m_Mesh = null;

    private int m_Resolution = 128;

    // Start is called before the first frame update
    void Start()
    {
        m_Filter = GetComponent<MeshFilter>();
        m_Renderer = GetComponent<MeshRenderer>();

        m_Mesh = new Mesh();

        //Verts
        List<Vector3> verts = new List<Vector3>();
        for(int y=0; y < m_Resolution; y++)
            for (int x = 0; x < m_Resolution; x++)
            {
                verts.Add(new Vector3(x, 0f, y));
            }
        m_Mesh.SetVertices(verts);

        //Tris
        int xvCount = m_Resolution + 1;
        int yvCount = m_Resolution + 1;
        int[] triangles = new int[m_Resolution * m_Resolution * 6];
        int triCount = 0;
        for (int y = 0; y < m_Resolution; y++)
            for (int x = 0; x < m_Resolution; x++)
            {
                triangles[triCount] = x + (y * xvCount);
                triangles[triCount + 1] = triangles[triCount + 4] = x + ((y + 1) * xvCount);
                triangles[triCount + 2] = triangles[triCount + 3] = x + (y * xvCount) + 1;
                triangles[triCount + 5] = x + ((y + 1) * xvCount) + 1;
                triCount += 6;
            }
        m_Mesh.SetTriangles(triangles, 0);

        m_Mesh.RecalculateNormals();

        m_Filter.mesh = m_Mesh;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
