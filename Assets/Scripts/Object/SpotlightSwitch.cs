using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class SpotlightSwitch : MonoBehaviour
{
    [SerializeField] protected Spotlight spotlight;

    protected bool isInteracting { get; private set; } = false;
    protected SpriteRenderer sr { get; private set;}

    protected virtual void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void OnTriggerEnter2D(Collider2D other) {
        if(!isInteracting && other.gameObject.CompareTag("Player")) {
            isInteracting = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(isInteracting && other.gameObject.CompareTag("Player")) {
            isInteracting = false;
        }
    }
}
