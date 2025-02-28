using NUnit.Framework.Constraints;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.UI;
using Pathfinding;
using UnityEditor.Experimental.GraphView;

public class GenerateMaze : MonoBehaviour
{
    [SerializeField] GameObject roomPrefab;
    [SerializeField] InputField widthInput;
    [SerializeField] InputField heightInput;
    [SerializeField] Button generateButton;
    [SerializeField] CollectibleSpawner collectibleSpawner;
    [SerializeField] private Text readyText;

    // The grid
    Room[,] rooms = null;

    [SerializeField] int numX = 10;
    [SerializeField] int numY = 10;

    // Room height and width
    float roomWidth;
    float roomHeight;

    // Stack for backtracking
    Stack<Room> stack = new Stack<Room>();

    bool generating = false;

    private void GetRoomSize()
    {
        SpriteRenderer[] spriteRenderers = roomPrefab.GetComponentsInChildren<SpriteRenderer>();

        Vector3 minBounds = Vector3.positiveInfinity;
        Vector3 maxBounds = Vector3.negativeInfinity;

        foreach(SpriteRenderer ren in spriteRenderers)
        {
            minBounds = Vector3.Min(minBounds, ren.bounds.min);
            maxBounds = Vector3.Max(maxBounds, ren.bounds.max);
        }

        roomWidth = maxBounds.x - minBounds.x;
        roomHeight = maxBounds.y - minBounds.y;
    }

    private void OnGenerateButtonClicked()
    {
        int width = int.Parse(widthInput.text);
        int height = int.Parse(heightInput.text);

        width = Mathf.Clamp(width, 5, 50);
        height = Mathf.Clamp(height, 5, 50);

        InitializeMaze(width, height);
        CreateMaze();

        ConfigurePathFindingGrid();
    }

    public void InitializeMaze(int width, int height)
    {
        numX = width;
        numY = height;

        //Clear existing maze if any
        if (rooms != null)
        {
            foreach (var room in rooms)
            {
                if (room != null)
                {
                    Destroy(room.gameObject);
                }
            }
        }

        rooms = new Room[numX, numY];

        for (int i = 0; i < numX; ++i)
        {
            for (int j = 0; j < numY; ++j)
            {
                GameObject room = Instantiate(roomPrefab, new Vector3(i * roomWidth, j * roomHeight, 0.0f), Quaternion.identity);
                room.name = "Room_" + i + "_" + j;
                rooms[i, j] = room.GetComponent<Room>();
                rooms[i, j].Index = new Vector2Int(i, j);
            }
        }
    }

    private void Start()
    {
        GetRoomSize();
        generateButton.onClick.AddListener(() => OnGenerateButtonClicked());
    }

    private void RemoveRoomWall(int x, int y, Room.Directions dir)
    {
        if(dir != Room.Directions.NONE)
        {
            rooms[x, y].SetDirFlag(dir, false);
        }
        

        Room.Directions opp = Room.Directions.NONE;
        switch(dir)
        {
            case Room.Directions.TOP:
                if (y < numY -1)
                {
                    opp = Room.Directions.BOTTOM;
                    ++y;
                }
                break;
            case Room.Directions.RIGHT:
                if (x < numX - 1)
                {
                    opp = Room.Directions.LEFT;
                    ++x;
                }
                break;
            case Room.Directions.BOTTOM:
                if (y > 0)
                {
                    opp = Room.Directions.TOP;
                    --y;
                }
                break;
            case Room.Directions.LEFT:
                if (x > 0)
                {
                    opp = Room.Directions.RIGHT;
                    --x;
                }
                break;
        }
        if (opp != Room.Directions.NONE)
        {
           rooms[x, y].SetDirFlag(opp, false);
        }
    }

    public List<Tuple<Room.Directions, Room>> GetNeighboursNotVisited(int cx, int cy)
    {
        List<Tuple<Room.Directions, Room>> neighbours = new List<Tuple<Room.Directions, Room>>();

        foreach(Room.Directions dir in Enum.GetValues(typeof(Room.Directions)))
        {
            int x = cx;
            int y = cy;

            switch (dir)
            {
                case Room.Directions.TOP:
                    if (y <numY - 1)
                    {
                        ++y;
                        if (!rooms[x, y].visited)
                        {
                            neighbours.Add(new Tuple<Room.Directions, Room>(Room.Directions.TOP, rooms[x, y]));
                        }
                    }
                    break;

                case Room.Directions.RIGHT:
                    if (x < numX - 1)
                    {
                        ++x;
                        if (!rooms[x, y].visited)
                        {
                            neighbours.Add(new Tuple<Room.Directions, Room>(Room.Directions.RIGHT, rooms[x, y]));
                        }
                    }
                    break;

                case Room.Directions.BOTTOM:
                    if (y > 0)
                    {
                        --y;
                        if (!rooms[x, y].visited)
                        {
                            neighbours.Add(new Tuple<Room.Directions, Room>(Room.Directions.BOTTOM, rooms[x, y]));
                        }
                    }
                    break;

                case Room.Directions.LEFT:
                    if (x > 0)
                    {
                        --x;
                        if (!rooms[x, y].visited)
                        {
                            neighbours.Add(new Tuple<Room.Directions, Room>(Room.Directions.LEFT, rooms[x, y]));
                        }
                    }
                    break;
            }
        }
        return neighbours;
    }

    private bool GenerateStep()
    {
        if (stack.Count == 0) return true;

        Room r = stack.Peek();
        var neighbours = GetNeighboursNotVisited(r.Index.x, r.Index.y);

        if (neighbours.Count != 0)
        {
            var index = 0;

            if (neighbours.Count > 1)
            {
                index = UnityEngine.Random.Range(0, neighbours.Count);
            }

            var item = neighbours[index];
            Room neighbour = item.Item2;
            neighbour.visited = true;
            RemoveRoomWall(r.Index.x, r.Index.y, item.Item1);

            stack.Push(neighbour);
        }

        else
        {
            stack.Pop();
        }

        return false;
    }

    public void CreateMaze()
    {
        if (generating) return;

        Reset();
        
        stack.Push(rooms[0, 0]);
       
        StartCoroutine(Coroutine_Generate());
    }

    IEnumerator Coroutine_Generate()
    {
        generating = true;
        bool flag = false;

        if (readyText != null)
        {
            readyText.text = "Generating Maze...";
            readyText.gameObject.SetActive(true);
        }

        while (!flag)
        {
            flag = GenerateStep();
            yield return new WaitForSeconds(0.01f);
        }

        generating = false;

        if (collectibleSpawner != null)
        {
            collectibleSpawner.SpawnCollectibles(rooms);
        }

        PlayerScript player = FindObjectOfType<PlayerScript>();
        if ( player != null)
        {
            player.canMove = true;
        }

        ConfigurePathFindingGrid();

        if (readyText != null)
        {
            readyText.text = "Ready!";
            yield return new WaitForSeconds(2f);
            readyText.gameObject.SetActive(false);
        }
    }

    private void Reset()
    {
        for (int i = 0; i < numX; ++i)
        {
            for (int j = 0; j <numY; ++j)
            {
                rooms[i, j].SetDirFlag(Room.Directions.TOP, true);
                rooms[i, j].SetDirFlag(Room.Directions.RIGHT, true);
                rooms[i, j].SetDirFlag(Room.Directions.BOTTOM, true);
                rooms[i, j].SetDirFlag(Room.Directions.LEFT, true);
                rooms[i, j].visited = false;
            }
        }
    }

    void ConfigurePathFindingGrid()
    {
        AstarPath astarPath = AstarPath.active;
        GridGraph gridGraph = astarPath.data.gridGraph;

        gridGraph.nodeSize = Mathf.Min(roomWidth, roomHeight);
        gridGraph.width = numX;
        gridGraph.depth = numY;

        Vector3 mazeBottomLeft = new Vector3(0, 0, 0);
        Vector3 mazeCenter = mazeBottomLeft + new Vector3((numX - 1) * roomWidth / 2, (numY - 1) * roomHeight / 2, 0);

        gridGraph.center = mazeCenter;
        astarPath.Scan();

        for (int i = 0; i < numX; i++)
        {
            for (int j = 0; j < numY; j++)
            {
                Vector3 nodePosition = new Vector3(i * roomWidth, j * roomHeight, 0);
                GridNodeBase node = gridGraph.GetNode(i, j);

                if (node == null)
                {
                    Debug.LogError($"Node at pos ({i}, {j}) is null!");
                    continue;
                }

                bool isWalkable = true;
                node.Walkable = isWalkable;
            }
        }

        astarPath.Scan();
        Debug.Log("Graph successfully updated and rescanned!");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!generating)
            {
                CreateMaze();
            }
        }
    }
}
