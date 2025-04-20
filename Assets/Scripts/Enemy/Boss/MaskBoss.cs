using UnityEngine;
using UnityEngine.Events;

public class MaskBoss : MonoBehaviour
{
    protected bool isDead = false;
    protected MaskBossStats stats;
    protected float attackCoolDownRemain;

    public UnityEvent dieEvent;
    public bool battle { get; protected set; }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        battle = false;
        stats = GetComponent<MaskBossStats>();
    }

    public virtual void BattleStart() {
        battle = true;
    }

    public virtual void Die()
    {
        Debug.Log("Die");
        // 사망 상태 전환
        isDead = true;
        dieEvent.Invoke();
    }
}
