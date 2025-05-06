using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EpisodeItem : MonoBehaviour, ICollectable
{
    public void Collect(GameObject owner)
    {
        StageManager.Instance.ActGetTreasure();
        gameObject.SetActive(false);
    }

    protected void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player")) {
            Collect(other.gameObject);
        }
    }
}
