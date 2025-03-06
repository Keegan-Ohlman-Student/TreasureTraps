using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyAI : MonoBehaviour
{
    public Transform target;
    private Seeker seeker;
    private AIPath aiPath;
    public float speed = 3f;

    // Start is called before the first frame update
    void Start()
    {
        seeker = GetComponent<Seeker>();
        aiPath = GetComponent<AIPath>();

        InvokeRepeating(nameof(UpdatePath), 0f, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdatePath()
    {
        if (seeker.IsDone())
        {
            seeker.StartPath(transform.position, target.position);
        }
    }
}
