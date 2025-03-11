using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyAI : MonoBehaviour
{
    public Transform target;  // The player's position
    private Seeker seeker;
    private AIPath aiPath;
    public float speed = 3f;
    public LayerMask obstacleLayer; // Layer mask for walls (make sure "Wall" layer is set)
    private GridGraph gridGraph;

    private List<GraphNode> walkableNodes = new List<GraphNode>();
    private GraphNode currentNode;
    private GraphNode nextNode;

    private bool isChasing = false;
    private bool isMovingRandom = false;

    public float pathUpdateInterval = 0.5f;
    private float lastPathUpdateTime = 0f;

    void Start()
    {
        seeker = GetComponent<Seeker>();
        aiPath = GetComponent<AIPath>();
        gridGraph = AstarPath.active.data.gridGraph;

        // Initialize the walkable nodes list
        InitializeWalkableNodes();

        // Start updating the path every 0.5 seconds
        InvokeRepeating(nameof(UpdatePath), 0f, pathUpdateInterval);
    }

    void Update()
    {
        // If the enemy reaches the target node, pick a new random node (for random movement)
        if (nextNode != null && Vector3.Distance(transform.position, (Vector3)nextNode.position) < 1f)
        {
            // If we're not chasing and not moving randomly, choose a new random node
            if (!isChasing && !isMovingRandom)
            {
                PickRandomNode();  // Choose a new target node
            }
        }

        // If the enemy can see the player, switch to chasing the player
        if (CanSeePlayer())
        {
            if (!isChasing)
            {
                isChasing = true;  // Start chasing the player
                StopRandomMovement();  // Stop random movement
                SetPathToPlayer();  // Set path to player
            }
        }
        else if (!CanSeePlayer() && !isMovingRandom)
        {
            // If not chasing the player, follow the random node path
            if (!isChasing && !isMovingRandom)
            {
                StartRandomMovement();
            }
        }
    }

    void InitializeWalkableNodes()
    {
        // Get all the nodes from the grid graph and add the walkable ones to the list
        foreach (GraphNode node in gridGraph.nodes)
        {
            if ((node.Walkable) && !IsBlockedByWalls(node))
            {
                walkableNodes.Add(node);
            }
        }
    }

    void PickRandomNode()
    {
        // Pick a random walkable node from the list
        if (walkableNodes.Count == 0)
        {
            InitializeWalkableNodes();  // If no nodes, reinitialize
        }

        // Choose a random node
        nextNode = walkableNodes[Random.Range(0, walkableNodes.Count)];
        isMovingRandom = true;

        // Start pathfinding to the new node
        seeker.StartPath(transform.position, (Vector3)nextNode.position);
    }

    void StartRandomMovement()
    {
        if (nextNode == null)
        {
            PickRandomNode();  // Pick the first random node if there's none
        }

        // Start random movement if the enemy is not chasing
        isMovingRandom = true;
        seeker.StartPath(transform.position, (Vector3)nextNode.position);
    }

    void SetPathToPlayer()
    {
        // Pathfinding to the player's position if the enemy is chasing
        if (seeker.IsDone())
        {
            seeker.StartPath(transform.position, target.position);
        }
    }

    void StopRandomMovement()
    {
        // Stop any random movement pathfinding
        isMovingRandom = false;
        nextNode = null;  // Clear the current random node
    }

    bool CanSeePlayer()
    {
        // Use Raycast to check if there are any walls between the enemy and the player
        RaycastHit2D hit = Physics2D.Linecast(transform.position, target.position, obstacleLayer);
        return hit.collider == null;  // If no wall in the way, the enemy can see the player
    }

    bool IsBlockedByWalls(GraphNode node)
    {
        // Check if the node is blocked by walls
        Vector3 nodePosition = (Vector3)node.position;
        RaycastHit2D hit = Physics2D.Linecast(transform.position, nodePosition, obstacleLayer);
        return hit.collider != null;  // If a wall exists, this node is blocked
    }

    // This method is being called by InvokeRepeating instead of UpdatePath
    void UpdatePath()
    {
        if (seeker.IsDone())
        {
            // If the enemy is chasing the player and can see the player, keep following the player
            if (isChasing && CanSeePlayer())
            {
                SetPathToPlayer();
            }
            // If the enemy isn't chasing the player, start random movement immediately
            else if (!isChasing && !isMovingRandom)
            {
                StartRandomMovement();
            }
            // If the player is not seen, and the enemy has already been moving randomly, just continue with random movement
            else if (!isChasing && isMovingRandom)
            {
                // Continue moving randomly even if the player is lost
                seeker.StartPath(transform.position, (Vector3)nextNode.position);
            }
        }
    }
}