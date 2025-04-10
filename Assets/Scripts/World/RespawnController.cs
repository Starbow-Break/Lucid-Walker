using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnController : MonoBehaviour
{
    public static RespawnController Instance;
    public Transform respawnPoint;
    private void Awake()
    {
        Instance = this;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerDamage pd = other.GetComponent<PlayerDamage>();
            if (pd != null)
            {
                pd.TakeDamage(1, transform);
            }
            if (HealthManager.Instance.currentHealth > 0)
                other.transform.position = respawnPoint.position;
        }

    }
}
