using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MaskBossLamp : MonoBehaviour
{
    enum LampStatus {
        OFF, YELLOW, RED
    }

    [SerializeField] GameObject lampObj;
    [SerializeField] Sprite offSprite;
    [SerializeField] Sprite yellowSprite;
    [SerializeField] GameObject yellowLightObj;
    [SerializeField] Sprite redSprite;
    [SerializeField] GameObject redLightObj;

    LampStatus status = LampStatus.OFF;
    SpriteRenderer lampSR;

    void Awake() {
        lampSR = lampObj.GetComponent<SpriteRenderer>();
        TurnOff();
    }

    public void TurnOff() {
        ChangeStatus(LampStatus.OFF);
    }

    public void TurnOnYellowLight() {
        ChangeStatus(LampStatus.YELLOW);
    }

    public void TurnOnRedLight() {
        ChangeStatus(LampStatus.RED);
    }

    void ChangeStatus(LampStatus status) {
        this.status = status;

        Sprite sprite;
        switch(status) {
            case LampStatus.OFF:
                sprite = offSprite;
                yellowLightObj.SetActive(false);
                redLightObj.SetActive(false);
                break;
            case LampStatus.YELLOW:
                sprite = yellowSprite;
                yellowLightObj.SetActive(true);
                redLightObj.SetActive(false);
                break;
            default:
                sprite = redSprite;
                yellowLightObj.SetActive(false);
                redLightObj.SetActive(true);
                break;
        }
        lampSR.sprite = sprite;
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
