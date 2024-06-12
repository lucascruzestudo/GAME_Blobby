using UnityEngine;

public class GameOverPlatform : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            ContactPoint2D[] contactPoints = collision.contacts;
            foreach (var contact in contactPoints)
            {
                if (contact.point.y > transform.position.y)
                {
                    collision.gameObject.GetComponent<PlayerController>().StopPlayer();
                    
                    GameManager.Instance.GameOver();
                    break;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            Collider2D playerCollider = collider.GetComponent<Collider2D>();
            if (playerCollider != null && playerCollider.bounds.min.y > transform.position.y)
            {
                collider.GetComponent<PlayerController>().StopPlayer();
                
                GameManager.Instance.GameOver();
            }
        }
    }
}
