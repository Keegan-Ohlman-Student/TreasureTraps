using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.U2D.Sprites;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour
{
    private Rigidbody2D myRigidBody;
    private float speed = 5;
    public bool isMoving = false;
    private bool isFacing;
    public bool canMove = false;
    [SerializeField] private GameObject trapPrefab;
    [SerializeField] private int trapCount = 5;

    public int currentHealth = 3;
    public int maxHealth = 2;
    private bool isInvulerable = false;
    [SerializeField] private float invulnerabilityDuration = 1f;

    [SerializeField] private UnityEngine.UI.Text healthText;

    // Start is called before the first frame update
    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
        HandleTrapPlacement();
        HandleTrapPickup();

        if (healthText != null)
        {
            healthText.text = "Health: " + currentHealth;
        }
    }

    void HandleMovement()
    {
        if (!canMove) return;

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

    void HandleTrapPlacement()
    {
        if (Input.GetKeyDown(KeyCode.T) && trapCount > 0)
        {
            Vector3 dropPos = transform.position;
            GameObject newTrap = Instantiate(trapPrefab, dropPos, Quaternion.identity);

            trapCount--;
            newTrap.SetActive(true);
        }
    }

    void HandleTrapPickup()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.5f);

            foreach(Collider2D collider in colliders)
            {
                TrapScript trap = collider.GetComponent<TrapScript>();

                if (trap != null && trap.CanBePickedUp())
                {
                    AddTrap(trap.GetTrapAmount());
                    Destroy(trap.gameObject);
                    return;
                }
            }
        }
    }

    public void AddTrap(int amount)
    {
        trapCount+= amount;
    }

    public void TakeDamage()
    {
        if (!isInvulerable)
        {
            currentHealth--;

            if (currentHealth <= 0)
            {
                Die();
            }
            else
            {
                StartCoroutine(InvulnerabilityCooldown());
            }
        }
    }

    private void Die()
    {
        canMove = false;
        SceneManager.LoadScene("DeathScreen");
    }

    private IEnumerator InvulnerabilityCooldown()
    {
        isInvulerable = true;
        yield return new WaitForSeconds(invulnerabilityDuration);
        isInvulerable = false;
    }
}
