using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapScript : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private Collider2D trapCollider;
    private bool hasTriggered = false;

    private int trapAmount = 1;
    private bool canBePickedUp = true;
    private bool playerNearby = false;

    private void Start()
    {
        if (trapCollider == null)
        {
            trapCollider = GetComponent<Collider2D>();
        }  
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Enemy") && !hasTriggered)
        {
            hasTriggered = true;
            anim.SetTrigger("CloseTrap");
            StartCoroutine(DestroyEnemy(collision.gameObject));
        }

        if (collision.CompareTag("Player"))
        {
            playerNearby = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerNearby = false;
        }
    }

    public bool CanBePickedUp()
    {
        return canBePickedUp && playerNearby;
    }

    public int GetTrapAmount()
    {
        return trapAmount;
    }

    private IEnumerator DestroyEnemy(GameObject enemy)
    {
        yield return new WaitForSeconds(0.48f);

        Destroy(enemy);
        Destroy(gameObject);
    }
}
