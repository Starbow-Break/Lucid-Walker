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
            spotlight.Switch();
            ChangeSprite(spotlight.isOn);
        }
    }

    void ChangeSprite(bool value)
    {
        sr.sprite = value ? onSprite : offSprite;
    }
}
