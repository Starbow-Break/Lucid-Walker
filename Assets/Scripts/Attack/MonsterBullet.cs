using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterBullet : MonoBehaviour
{
    [SerializeField] float speed = 10.0f; // Speed of the bullet
    [SerializeField] float lifeTime = 2.0f;
    [SerializeField] ParticleSystem effect;

    private bool hasHit = false; // Whether the bullet has already hit a target
    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        ParticleSystem.VelocityOverLifetimeModule volm = effect.velocityOverLifetime;
        ParticleSystem.ShapeModule sm = effect.shape;

        volm.xMultiplier *= transform.localScale.x;
        sm.position = new(
            sm.position.x * transform.localScale.x,
            sm.position.y,
            sm.position.z
        );

        Destroy(gameObject, lifeTime); // Destroy the bullet after 2 seconds by default
    }

    void Update()
    {
        if (!hasHit) {
            // Move the bullet based on its facing direction
            rb.velocity = speed * transform.localScale.x * (transform.rotation * Vector3.right);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        hasHit = true; // Set hit state to true

        // Check if the collided object implements IDamageable
        if (other.CompareTag("Player"))
        {
            Debug.Log("Hit detected!");
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(1, transform); // Apply 1 damage
                Debug.Log($"Bullet hit {other.name}");
            }
        }

        Destroy(gameObject);
    }
}
