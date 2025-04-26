using JetBrains.Annotations;
using UnityEngine;

public class WoodBox : MonoBehaviour, IDamageable
{
    private readonly int HitHash = Animator.StringToHash("hit");
    private readonly int DestroyHash = Animator.StringToHash("destroy");

    [SerializeField] GameObject item;
    [SerializeField] Transform spawnPoint;
    [SerializeField] private int hp = 3;

    Animator anim;

    void Awake() {
        anim = GetComponent<Animator>();
    }

    public void TakeDamage(int damage, Transform attacker) {
        if(hp <= 0) {
            return;
        }

        anim.SetTrigger(HitHash);

        if(--hp <= 0) {
            TriggerDestroy();
        }
    }

    private void TriggerDestroy()
    {
        anim.SetTrigger(DestroyHash);
        SpawnItem();
    }
    
    private void Destroy()
    {
        MonoBehaviour.Destroy(gameObject);
    }

    private void SpawnItem()
    {
        Instantiate(item, spawnPoint.position, Quaternion.identity);
    }
}
