using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleScript : MonoBehaviour
{
    public int scoreValue;
    [SerializeField] private Score s;

    // Start is called before the first frame update
    void Start()
    {
        s = FindObjectOfType<Score>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            s.score += scoreValue;
            GameManager.instance.CollectibleCollected();
            gameObject.SetActive(false);
        } 
    }
}
