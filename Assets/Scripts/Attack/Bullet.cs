using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Animator anim;
    public float speed = 10f; // Speed of the bullet
    public float distance = 1f; // Raycast distance
    public LayerMask isLayer; // Layer to check collisions

    private bool hasHit = false; // Whether the bullet has already hit a target

    void Start()
    {
        anim = GetComponent<Animator>();
        Invoke("DestroyBullet", 2f); // Destroy the bullet after 2 seconds by default
    }

    void Update()
    {
        if (hasHit) return; // Stop moving if already hit

        // Check for collision using Raycast
        RaycastHit2D ray = Physics2D.Raycast(transform.position, transform.right * transform.localScale.x, distance, isLayer);
        if (ray.collider != null)
        {
            anim.SetTrigger("Hit"); // Play hit animation
            HandleHit(ray.collider); // Handle the hit logic
        }

        // Move the bullet based on its facing direction
        transform.Translate(Vector3.right * transform.localScale.x * speed * Time.deltaTime);
    }

    void HandleHit(Collider2D collider)
    {
        hasHit = true; // Set hit state to true

        // Check if the collided object implements IDamageable
        if (collider.CompareTag("Enemy") || collider.CompareTag("Player"))
        {
            Debug.Log("Hit detected!");
            IDamageable damageable = collider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(1, transform); // Apply 1 damage
                Debug.Log($"Bullet hit {collider.name}");
            }
        }

        // Destroy the bullet after the hit animation
        float animLength = anim.GetCurrentAnimatorStateInfo(0).length;
        Invoke("DestroyBullet", animLength / 2); // Delay destruction for animation
    }

    void DestroyBullet()
    {
        Destroy(gameObject); // Destroy the bullet object
    }
}
