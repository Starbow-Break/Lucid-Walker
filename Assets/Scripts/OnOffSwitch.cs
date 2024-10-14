using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class OnOffSwitch : SpotlightSwitch
{
    [SerializeField] Sprite onSprite;
    [SerializeField] Sprite offSprite;

    protected override void Awake() {
        base.Awake();

        if(spotlight.isOn) {
            sr.sprite = onSprite;
        }
        else {
            sr.sprite = offSprite;
        }
    }

    void Update() {
        if(isInteracting && Input.GetKeyDown(KeyCode.Z) && !spotlight.isBroken) {
            ChangeSprite();
            spotlight.Switch();
        }
    }

    void ChangeSprite()
    {
        sr.sprite = sr.sprite == onSprite ? offSprite : onSprite;
    }
}
