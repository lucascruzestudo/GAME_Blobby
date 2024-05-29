using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public Rigidbody2D rb;

    private float moveX;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance != null && !GameManager.Instance.gameEnded)
        {
            moveX = Input.GetAxis("Horizontal") * moveSpeed;
        }
        else
        {
            moveX = 0f; // Stop horizontal movement if the game has ended
        }
    }

    private void FixedUpdate()
    {
        Vector2 velocity = rb.velocity;
        velocity.x = moveX;
        rb.velocity = velocity;
    }

    public void StopPlayer()
    {
        rb.velocity = Vector2.zero;
        moveX = 0f;
    }
}
