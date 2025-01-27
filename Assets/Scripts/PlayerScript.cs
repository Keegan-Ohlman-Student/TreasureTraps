using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Sprites;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    private Rigidbody2D myRigidBody;
    private float speed = 5;
    public bool isMoving = false;
    private bool isFacing;

    // Start is called before the first frame update
    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKey)
        {
            if (Input.GetKey(KeyCode.D))
            {
                myRigidBody.position += Vector2.right * speed * Time.deltaTime;
                isMoving = true;

                if (Input.GetKeyDown(KeyCode.D) && isFacing)
                {
                    transform.Rotate(0, -180, 0);
                    isFacing = false;
                }
            }

            if (Input.GetKey(KeyCode.A))
            {
                myRigidBody.position += Vector2.left * speed * Time.deltaTime;
                isMoving = true;

                if (Input.GetKeyDown(KeyCode.A) && !isFacing)
                {
                    transform.Rotate(0, 180, 0);
                    isFacing = true;
                }
            }

            

            if (Input.GetKey(KeyCode.S))
            {
                myRigidBody.position += Vector2.down * speed * Time.deltaTime;
                isMoving = true;
            }

            if (Input.GetKey(KeyCode.W))
            {
                myRigidBody.position += Vector2.up * speed * Time.deltaTime;
                isMoving = true;
            }
        }

        else
        {
            isMoving = false;
        }
    }
}
