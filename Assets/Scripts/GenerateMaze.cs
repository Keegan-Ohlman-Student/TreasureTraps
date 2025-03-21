using NUnit.Framework.Constraints;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pathfinding;

public class GenerateMaze : MonoBehaviour
{
    [SerializeField] GameObject roomPrefab;
    [SerializeField] InputField widthInput;
    [SerializeField] InputField heightInput;
    [SerializeField] Button generateButton;
    [SerializeField] CollectibleSpawner collectibleSpawner;
    [SerializeField] EnemySpawner enemySpawner;
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

        int mazeWidth = rooms.GetLength(0);
        int mazeHeight = rooms.GetLength(1);

        PlayerScript player = FindObjectOfType<PlayerScript>();

        if (collectibleSpawner != null)
        {
            collectibleSpawner.SpawnCollectibles(rooms);
        }

        if (enemySpawner != null)
        {
            Vector3 playerStartPos = player.transform.position;
            enemySpawner.SpawnEnemies(rooms, playerStartPos);
        }

        
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

        gridGraph.SetDimensions(numX, numY, Mathf.Min(roomWidth, roomHeight));
        
        Vector3 firstRoomPos = rooms[0, 0].transform.position;
        Vector3 lastRoomPos = rooms[numX - 1, numY - 1].transform.position;
        Vector3 mazeCenter = (firstRoomPos + lastRoomPos) * 0.5f;
        gridGraph.center = mazeCenter;

        astarPath.Scan();
        AstarPath.active.Scan();
    }

    private void Update()
    {
        
    }
}
