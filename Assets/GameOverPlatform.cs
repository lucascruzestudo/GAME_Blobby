using UnityEngine;

public class GameOverPlatform : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the object colliding with this platform is tagged as "Player"
        if (collision.gameObject.CompareTag("Player"))
        {
            // Get the contact points
            ContactPoint2D[] contactPoints = collision.contacts;
            foreach (var contact in contactPoints)
            {
                // Check if the contact point is above the platform's collider
                if (contact.point.y > transform.position.y)
                {
                    // Stop the player's movement
                    collision.gameObject.GetComponent<PlayerController>().StopPlayer();
                    
                    // Call the GameOver method from the GameManager
                    GameManager.Instance.GameOver();
                    break;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        // Check if the object triggering this platform is tagged as "Player"
        if (collider.gameObject.CompareTag("Player"))
        {
            // Get the player's collider bounds and the platform's collider bounds
            Collider2D playerCollider = collider.GetComponent<Collider2D>();
            if (playerCollider != null && playerCollider.bounds.min.y > transform.position.y)
            {
                // Stop the player's movement
                collider.GetComponent<PlayerController>().StopPlayer();
                
                // Call the GameOver method from the GameManager
                GameManager.Instance.GameOver();
            }
        }
    }
}
