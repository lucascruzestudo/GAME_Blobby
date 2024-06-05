using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float jumpForce = 5f;
    public Rigidbody2D rb;
    public AudioClip jumpSound;
    public AudioClip landingSound;

    private float moveX;
    private bool canJump = false;
    private AudioSource audioSource;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (GameManager.Instance != null && !GameManager.Instance.gameEnded)
        {
            moveX = Input.GetAxis("Horizontal") * moveSpeed;

            if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) && canJump)
            {
                Jump();
            }
        }
        else
        {
            moveX = 0f;
        }
    }

    private void FixedUpdate()
    {
        Vector2 velocity = rb.velocity;
        velocity.x = moveX;
        rb.velocity = velocity;
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        canJump = false;
        if (jumpSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(jumpSound, 1.5f);
        }
        Debug.Log("Jump initiated");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.5)
            {
                canJump = true;
                if (landingSound != null && audioSource != null)
                {
                audioSource.PlayOneShot(landingSound, 3.5f);
                }
                Debug.Log("Can jump");
                break;
            }
        }
    }

    public void StopPlayer()
    {
        rb.velocity = Vector2.zero;
        moveX = 0f;
    }
}
