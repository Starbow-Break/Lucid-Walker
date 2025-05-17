using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BossCollider : MonoBehaviour, IDamageable
{
    [SerializeField] MaskBossStats _stats;

    public void TakeDamage(int damage, Transform attacker)
    {
        _stats.GetComponent<IDamageable>().TakeDamage(damage, attacker);
    }
}
