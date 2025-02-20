using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MonsterBullet : MonoBehaviour
{
    [SerializeField] float speed = 10.0f; // Speed of the bullet
    [SerializeField] float lifeTime = 2.0f;
    [SerializeField] bool destroyAfterHit = true;
    [SerializeField] ParticleSystem effect;

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

        if(effect != null) {
            ParticleSystem.VelocityOverLifetimeModule volm = effect.velocityOverLifetime;
            ParticleSystem.ShapeModule sm = effect.shape;

            volm.xMultiplier *= transform.localScale.x >= 0 ? 1 : -1;
            sm.position = new(
                sm.position.x * (transform.localScale.x >= 0 ? 1 : -1),
                sm.position.y,
                sm.position.z
            );
        }

        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        if (!destroyAfterHit || !hasHit) {
            rb.velocity = (transform.localScale.x > 0 ? 1 : -1) * speed * transform.right;
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

            if(destroyAfterHit) {
                Destroy(gameObject);
            }
        }
    }
}
