using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{   
    const int SPIKE_DAMAGE = 1;
    
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            IDamageable damageable = collider.GetComponent<IDamageable>();
            damageable.TakeDamage(SPIKE_DAMAGE, transform);
        }
    }
}
