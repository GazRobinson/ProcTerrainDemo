using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent {
    public Agent(Vector2Int startPos, Vector2Int startHeading)
    {
        Position = startPos;
        Heading = startHeading;

        nextTurn =  Random.value;
        nextSpawn = Random.value;
        nextSplit = Random.value;
    }

    public Vector2Int Position;
    public Vector2Int Heading;
    private float turnChance = 0.0f;
    private float spawnChance = 0.0f;
    private float splitChance = 0.0f;


    private float nextTurn = 0.0f;
    private float nextSpawn = 0.0f;
    private float nextSplit = 0.0f;

    private float turnChanceIncrement = 0.05f;
    private float spawnChanceIncrement = 0.01f;
    private float splitChanceIncrement = 0.01f;

    public System.Action<Agent> SpawnRoom;
    public System.Action<Agent> Split;

    public void Move()
    {
        Position += Heading;

        if(nextTurn < turnChance)
        {
            nextTurn = Random.value;
            Debug.Log("Turn at: " + turnChance);
            turnChance = 0f;
            if(Random.value > 0.5)
            {
                int t = Heading.x;
                Heading.x = -Heading.y;
                Heading.y = t;
            }
            else
            {
                int t = Heading.x;
                Heading.x = Heading.y;
                Heading.y = -t;
            }  
            if(nextSplit < splitChance)
            {
                nextSplit = Random.value;
                splitChance = 0;
                Split(this);
            }
            else
            {
                splitChance += splitChanceIncrement;
            }
        }
        else
        {
            turnChance += turnChanceIncrement;
        }
        if (nextSpawn < spawnChance)
        {
            nextSpawn = Random.value;
            Debug.Log("Spawn at: " + spawnChance.ToString());
            spawnChance = 0f;
            SpawnRoom(this);
        }
        else
        {
            spawnChance += spawnChanceIncrement;
        }
    }
}


public class AgentManager : MonoBehaviour
{
    public int textureSize = 256;
    public int iterationsPerFrame = 1;
    Texture2D tex;
    Color[] colors;
    private bool generate = false;

    List<Agent> agents = new List<Agent>();
    // Start is called before the first frame update
    void Start()
    {
        tex = new Texture2D(textureSize, textureSize, TextureFormat.ARGB32, false);
        tex.filterMode = FilterMode.Point;
        colors = new Color[textureSize * textureSize];
        for (int y = 0; y < textureSize; y++)
        {
            for (int x = 0; x < textureSize; x++)
            {
                colors[x + y* textureSize] = Color.black;
            }
        }
            agents.Add(new Agent(new Vector2Int(textureSize/2, textureSize/2), Vector2Int.up));
        agents[0].SpawnRoom += OnSpawnRoom;
        agents[0].Split += OnSplit;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            generate = true;
        if (generate)
        {
            for (int f = 0; f < iterationsPerFrame; f++)
            {
                for (int i = 0; i < agents.Count; i++)
                {
                    agents[i].Move();
                    if (agents[i].Position.y >= textureSize)
                    {
                        agents[i].Position.y = 0;
                    }
                    else if (agents[i].Position.y < 0)
                    {
                        agents[i].Position.y = textureSize - 1;
                    }
                    colors[agents[i].Position.x + agents[i].Position.y * textureSize] = Color.white;
                    /*if (colors[agents[i].Position.x + agents[i].Position.y * textureSize] == Color.white)
                    {
                        agents.RemoveAt(i);
                        i--;
                    }
                    else
                    {
                    }*/
                }
                SetTexture();
                tex.Apply();
            }
        }
    }

    void SetTexture()
    {
        tex.SetPixels(colors);
        GetComponent<Renderer>().sharedMaterial.mainTexture = tex;
    }

    void OnSpawnRoom(Agent a)
    {
        Vector2Int p = a.Position;
        int size = 4;
        for(int y = p.y-size; y < p.y + size; y++)
        {
            for (int x = p.x - size; x < p.x + size; x++)
            {
                x = Mathf.Clamp(x, 0, textureSize);
                y = Mathf.Clamp(y, 0, textureSize);
                colors[x + y * textureSize] = Color.white;
            }
        }
        a.Position += a.Heading * size;
    }
    void OnSplit(Agent a)
    {
        agents.Add(new Agent(a.Position, new Vector2Int(-a.Heading.x, -a.Heading.y)));
        agents[agents.Count-1].SpawnRoom += OnSpawnRoom;
        agents[agents.Count - 1].Split += OnSplit;
    }
}
