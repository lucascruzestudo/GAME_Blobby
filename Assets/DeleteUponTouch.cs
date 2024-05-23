using UnityEngine;

public class DeleteUponTouch : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the collision is from below (collision normal points downwards)
        if (collision.contacts[0].normal.y < 0)
        {
            Debug.Log("Collided with: " + collision.gameObject.name + " from top");
            // Destroy the object
            Destroy(gameObject);
        }
    }
}
