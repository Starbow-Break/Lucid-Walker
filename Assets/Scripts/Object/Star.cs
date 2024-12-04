using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour
{
    const int DAMAGE = 1;

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player")) {
            IDamageable damageable = other.GetComponent<IDamageable>();
            damageable.TakeDamage(DAMAGE, transform);
        }
    }
}
