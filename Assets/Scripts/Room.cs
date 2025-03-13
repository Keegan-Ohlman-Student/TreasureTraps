using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public enum Directions
    {
        TOP, 
        RIGHT,
        BOTTOM,
        LEFT,
        NONE,
    }

    [SerializeField] GameObject topWall;
    [SerializeField] GameObject rightWall;
    [SerializeField] GameObject bottomWall;
    [SerializeField] GameObject leftWall;
    
    Dictionary<Directions, GameObject> walls = new Dictionary<Directions, GameObject>();

    public Vector2Int Index
    {
        get;
        set;
    }

    public bool visited { get; set; } = false;

    Dictionary<Directions, bool> dirflags = new Dictionary<Directions, bool>();

    private void Start()
    {
        
    }

    private void Awake()
    {
        walls[Directions.TOP] = topWall;
        walls[Directions.RIGHT] = rightWall;
        walls[Directions.BOTTOM] = bottomWall;
        walls[Directions.LEFT] = leftWall;

        dirflags[Directions.TOP] = true;
        dirflags[Directions.RIGHT] = true;
        dirflags[Directions.BOTTOM] = true;
        dirflags[Directions.LEFT] = true;
    }

    private void SetActive(Directions dir, bool flag)
    {
        walls[dir].SetActive(flag);
    }

    public void SetDirFlag(Directions dir, bool flag)
    {
        dirflags[dir] = flag;
        SetActive(dir, flag);
    }

    public void RemoveWallForExit()
    {
        List<Directions> availableWalls = new List<Directions>();

        if (walls[Directions.TOP] != null) availableWalls.Add(Directions.TOP);
        if (walls[Directions.RIGHT] != null) availableWalls.Add(Directions.RIGHT);
        if (walls[Directions.BOTTOM] != null) availableWalls.Add(Directions.BOTTOM);
        if (walls[Directions.LEFT] != null) availableWalls.Add(Directions.LEFT);


        if (availableWalls.Count > 0)
        {
            Directions wallToRemove = availableWalls[Random.Range(0, availableWalls.Count)];
            Destroy(walls[wallToRemove]);
            walls[wallToRemove] = null;
        }

        GameObject exitTrigger = new GameObject("ExitTrigger");
        exitTrigger.transform.position = transform.position;
        BoxCollider2D collider = exitTrigger.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        exitTrigger.AddComponent<ExitTrigger>();
    }
}