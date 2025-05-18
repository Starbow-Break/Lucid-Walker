using UnityEngine;

public class MaskBossTongue : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.name);
        if (collision.CompareTag("Player"))
        {
            var damageable = collision.GetComponent<IDamageable>();
            damageable?.TakeDamage(1, transform);
        }
    }
}
