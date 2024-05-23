using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;  // The player's transform
    public float deathThreshold = -5f; // How far below the camera the player can go before dying
    public bool playerDead = false;

    private void LateUpdate()
    {
        if (target.position.y > transform.position.y)
        {
            Vector3 newPosition = new Vector3(transform.position.x, target.position.y, transform.position.z);
            transform.position = newPosition;
            GameManager.Instance.UpdateScore(target.position.y);
        }

        if (target.position.y < transform.position.y + deathThreshold && !playerDead)
        {
            PlayerDied();
        }
    }

    private void PlayerDied()
    {
        playerDead = true;
        Debug.Log("Player has died.");
        GameManager.Instance.GameOver();
    }
}
