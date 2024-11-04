using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotlightFall : MonoBehaviour
{
    [SerializeField] Spotlight spotlight;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            spotlight.Fall();
            GetComponent<Collider2D>().enabled = false;
        }
    }
}
