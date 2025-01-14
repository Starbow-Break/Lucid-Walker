using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MaskBossLamp : MonoBehaviour
{
    [SerializeField] GameObject lampObj;
    [SerializeField] Sprite offSprite;
    [SerializeField] Sprite onSprite;
    [SerializeField] GameObject lightObj;

    SpriteRenderer lampSR;
    bool on;

    void Awake() {
        lampSR = lampObj.GetComponent<SpriteRenderer>();
        SetLamp(false);
    }

    public void SetLamp(bool isOn) {
        if(isOn) { TurnOn(); }
        else { TurnOff(); }
    }

    void TurnOn() {
        on = true;
        lampSR.sprite = onSprite;
        lightObj.SetActive(true);
    }

    void TurnOff() {
        on = false;
        lampSR.sprite = offSprite;
        lightObj.SetActive(false);
    }

    public IEnumerator MoveLampFlow(Vector3 start, Vector3 end, float time) {
        float currentTime = 0.0f;
        while(currentTime < time) {
            yield return null;
            currentTime += Time.deltaTime;
            transform.position = Vector3.Lerp(start, end, Mathf.Clamp01(currentTime / time));
        }
        yield return null;
    }
}
