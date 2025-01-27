using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MonsterBullet : MonoBehaviour
{
    [SerializeField] float speed = 10.0f; // Speed of the bullet
    [SerializeField] float lifeTime = 2.0f;

    private bool hasHit = false; // Whether the bullet has already hit a target
    Rigidbody2D rb;
    SpriteRenderer sr;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        sr = GetComponent<SpriteRenderer>();
        if(transform.right.x < 0) {
            sr.flipY = !sr.flipY;
        }

        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        if (!hasHit) {
            rb.velocity = speed * transform.right;
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        // Check if the collided object implements IDamageable
        if (other.CompareTag("Player"))
        {
            hasHit = true; // Set hit state to true
            
            Debug.Log("Hit detected!");
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(1, transform); // Apply 1 damage
                Debug.Log($"Bullet hit {other.name}");
            }

            Destroy(gameObject);
        }
    }
}
