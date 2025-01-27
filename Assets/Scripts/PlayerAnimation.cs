using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]

public class PlayerAnimation : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private PlayerScript player;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        player = GetComponent<PlayerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetBool("isMoving", player.isMoving);
    }
}
