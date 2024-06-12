using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{

    public string sparkEffectPrefabName = "SparkEffect";


    public float jumpForce = 10f;
    public GameObject sparkEffectPrefab;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.relativeVelocity.y <= 0f)
        {
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 velocity = rb.velocity;
                velocity.y = jumpForce;
                rb.velocity = velocity;

                GameObject effect = GameObject.Find(sparkEffectPrefabName);
                if (effect)
                {
                    GameObject instance = Instantiate(effect, transform.position, transform.rotation);
                    Destroy(instance, 1f);
                }
            }
        }
    }
}

