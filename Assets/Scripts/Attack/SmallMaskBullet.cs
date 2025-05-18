using UnityEngine;

public class SmallMaskBullet : MonoBehaviour
{
    [SerializeField] float lifeTime = 10.0f;

    Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        Destroy(gameObject, lifeTime);
    }

    public void SetVelocity(Vector2 newVelocity)
    {
        rb.velocity = newVelocity;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            var damageable = collision.GetComponent<IDamageable>();
            damageable?.TakeDamage(1, transform);
        }

        Destroy(gameObject);
    }
}
