using System.Collections;
using System.Collections.Generic;
using Packages.Rider.Editor.UnitTesting;
using UnityEngine;

public class LightFrameMirror : Mirror
{
    [SerializeField] bool lightOn = false; // 전등이 켜져 있으면 true
    [SerializeField] Sprite lightOnSprite;
    [SerializeField] Sprite lightOffSprite;
    [SerializeField] Collider2D mirrorCollider;
    SpriteRenderer sr;

    protected override void Awake() {
        base.Awake();
        sr = GetComponent<SpriteRenderer>();
    }

    void Start() {
        SetLight(lightOn);
    }

    public void SetLight(bool isOn) {
        lightOn = isOn;
        mirrorCollider.enabled = lightOn;
        sr.sprite = lightOn ? lightOnSprite : lightOffSprite;
    }
}
