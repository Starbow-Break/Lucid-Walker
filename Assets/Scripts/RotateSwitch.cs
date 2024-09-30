using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateSwitch : MonoBehaviour
{
    [SerializeField] Spotlight spotlight;
    [SerializeField] Sprite[] sprites;
    [SerializeField] int startIndex = 0;

    int currentIndex;

    bool isInteracting = false;
    SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        currentIndex = startIndex;

        sr.sprite = sprites[currentIndex];
    }

    void Update() {
        if(isInteracting && Input.GetKeyDown(KeyCode.Z)) {
            Switch();
            spotlight.RotateSwitch();
        }
    }

    void Switch()
    {
        currentIndex = (currentIndex + 1) % sprites.Length;
        sr.sprite = sprites[currentIndex];
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
