using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateSwitch : SpotlightSwitch
{
    [SerializeField] Sprite[] sprites;
    [SerializeField] int startIndex = 0;
    int currentIndex;

    protected override void Awake()
    {
        base.Awake();
        
        currentIndex = startIndex;
        sr.sprite = sprites[currentIndex];
    }

    void Update() {
        if(isInteracting && Input.GetKeyDown(KeyCode.Z) && !spotlight.isFall) {
            ChangeSprite();
            spotlight.RotateSwitch();
        }
    }

    void ChangeSprite()
    {
        currentIndex = (currentIndex + 1) % sprites.Length;
        sr.sprite = sprites[currentIndex];
    }
}
