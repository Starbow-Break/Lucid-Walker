using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotlightSwitch : MonoBehaviour
{
    [SerializeField] Spotlight spotlight;

    bool isInteracting = false;

    void Update() {
        if(isInteracting && Input.GetKeyDown(KeyCode.Z)) {
            spotlight.Switch();
        }
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
