using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class SpotlightSwitch : MonoBehaviour
{
    [SerializeField] Spotlight spotlight;
    [SerializeField] Sprite onSprite;
    [SerializeField] Sprite offSprite;

    bool isInteracting = false;
    SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();

        if(spotlight.isOn) {
            sr.sprite = onSprite;
        }
        else {
            sr.sprite = offSprite;
        }
    }

    void Update() {
        if(isInteracting && Input.GetKeyDown(KeyCode.Z)) {
            Switch();
            spotlight.Switch();
        }
    }

    void Switch()
    {
        sr.sprite = sr.sprite == onSprite ? offSprite : onSprite;
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
