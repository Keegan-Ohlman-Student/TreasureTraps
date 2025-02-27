using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyAI : MonoBehaviour
{
    public Transform target;
    private Seeker seeker;
    private Path path;
    private int currentWayPoint = 0;
    public float speed = 3f;

    // Start is called before the first frame update
    void Start()
    {
        seeker = GetComponent<Seeker>();
        seeker.StartPath(transform.position, target.position, OnPathComplete);
    }

    // Update is called once per frame
    void Update()
    {
        if (path == null)
        {
            return;
        }

        if (currentWayPoint < path.vectorPath.Count)
        {
            Vector3 direction = (path.vectorPath[currentWayPoint] - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;

            if (Vector3.Distance(transform.position, path.vectorPath[currentWayPoint]) < 0.1f) 
            {
                currentWayPoint++;
            }
        }
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWayPoint = 0;
        }
    }
}
