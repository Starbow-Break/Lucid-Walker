using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] Image fillImage;
    [SerializeField] Color baseColor;
    [SerializeField] Color blinkColor;
    [SerializeField] TextMeshProUGUI hpText;
    [SerializeField] float minPosWeight;
    [SerializeField] float maxPosWeight;

    Slider slider;
    Coroutine blinkCoroutine = null;

    void Awake() {
        slider = GetComponent<Slider>();
    }

    // Start is called before the first frame update
    void Start()
    {
        hpText.enabled = false;
    }

    public void SetValue(int hp, int maxHp) {
        slider.value = 1.0f * hp / maxHp;

        if(slider.value <= 0.2f) {
            blinkCoroutine ??= StartCoroutine(Blink());
            hpText.enabled = true;
        }
        else {
            StopCoroutine(blinkCoroutine);
            blinkCoroutine = null;
            hpText.enabled = false;
        }

        if(hpText.enabled) {
            hpText.text = hp.ToString();

            float barWidth = GetComponent<RectTransform>().rect.width;
            hpText.rectTransform.localPosition = new(
                barWidth * (maxPosWeight - minPosWeight) * slider.value / 0.2f + minPosWeight * barWidth,
                hpText.rectTransform.localPosition.y,
                hpText.rectTransform.localPosition.z
            );
        }
    }

    IEnumerator Blink() {
        float time = 0.0f;
        if (0.0f < slider.value && slider.value <= 0.2f) {
            yield return null;
            time += Time.deltaTime;
            time %= 1.0f;
            float value = 0.5f - 0.5f * Mathf.Cos(2.0f * Mathf.PI * time);
            fillImage.color = baseColor * value + blinkColor * (1.0f - value);
        }

        blinkCoroutine = null;
    }
}
