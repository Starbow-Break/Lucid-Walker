using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateSwitch : SpotlightSwitch
{
    [SerializeField] Sprite[] sprites;

    protected override void Awake()
    {
        base.Awake();
        
        if(sprites.Length > 0) {
            sr.sprite = sprites[spotlight.switchIndex >= sprites.Length ? ^1 : spotlight.switchIndex];
        }
    }

    void Update() {
        if(isInteracting && Input.GetKeyDown(KeyCode.Z) && !spotlight.isBroken) {
            spotlight.RotateSwitch();
            ChangeSprite(
                spotlight.switchIndex >= sprites.Length 
                ? spotlight.switchIndex - 1 
                : spotlight.switchIndex);
        }
    }

    void ChangeSprite(int index)
    {
        sr.sprite = sprites[index];
    }
}
