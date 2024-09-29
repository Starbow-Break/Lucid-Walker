using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

enum BlinkMode
{
    NONE, BLINK
}

public class Spotlight : MonoBehaviour
{   
    [Header("Common")]
    [Range(0.0f, 360.0f)]
    [SerializeField] float hitDeg; // 스포트라이트가 플에이어를 감지하는 시야각
    public bool isOn { get; private set; } // 켜짐 여부
    [SerializeField] BlinkMode blinkMode;

    [Header("Light")]
    [SerializeField] Light2D _light; // 불빛
    [SerializeField] float innerSpot; // Inner Spot
    [SerializeField] float outerSpot; // Outer Spot
    [SerializeField] float lightRadius; // 빛의 반지름
    [SerializeField] CircularSectorMesh mask; // Mask

    [Header("Source Light")]
    [SerializeField] SpriteRenderer lampSpriteRenderer; // 전등 이미지에 사용되는 렌더러
    [SerializeField] Sprite onSprite; // 점등 시 스프라이트
    [SerializeField] Sprite offSprite; // 점멸 시 스프라이트
    [SerializeField] Light2D sourceLight; // 광원 불빛
    
    
    Vector2 initDir;
    Vector2 lightDir;
    IEnumerator blinkCoroutine = null;

    // Start is called before the first frame update
    void Start()
    {
        // Get initial Direction (+Y dir)
        initDir = transform.rotation * Vector2.up;
        lightDir = initDir;

        // Setting Light
        _light.pointLightInnerAngle = innerSpot;
        _light.pointLightOuterAngle = outerSpot;
        _light.pointLightOuterRadius = lightRadius;

        // Setting Mask
        mask.SetRadius(lightRadius);
        mask.SetAngle(outerSpot);

        // Turn On/Off according to isOn value
        if(isOn) TurnOn(); else TurnOff();
    }

    void FixedUpdate()
    {
        // Rotate SpotLight
        if(isOn) {
            // Check Overlap Player
            Collider2D collider = Physics2D.OverlapCircle(transform.position, lightRadius, LayerMask.GetMask("Player"));
            
            // Follow Player
            if(collider != null) {
                FollowTarget(collider.gameObject);
            }
        }
    }

    void FollowTarget(GameObject target)
    {
        // Follow Target
        Vector2 nextDir = (target.transform.position - transform.position).normalized;

        float deg = Mathf.Rad2Deg * Mathf.Acos(Vector2.Dot(initDir, nextDir));

        if(deg <= hitDeg / 2)
        {
            float curRotZ = Mathf.Rad2Deg * Mathf.Atan2(-lightDir.x, lightDir.y);
            float nextRotZ = Mathf.Rad2Deg * Mathf.Atan2(-nextDir.x, nextDir.y);

            float diffRot = ((nextRotZ - curRotZ) % 360.0f + 360.0f) % 360.0f;
            diffRot = diffRot > 180.0f ? diffRot - 360.0f : diffRot;

            float targetRotZ = curRotZ + diffRot * 5.0f * Time.fixedDeltaTime;

            transform.rotation = Quaternion.Euler(0, 0, targetRotZ);
            lightDir = new(-Mathf.Sin(targetRotZ * Mathf.Deg2Rad), Mathf.Cos(targetRotZ * Mathf.Deg2Rad));
        }
    }

    // Switch Light
    public void Switch()
    {
        if(isOn) TurnOff(); else TurnOn();
    }

    // Turn On Light
    void TurnOn()
    {
        lampSpriteRenderer.sprite = onSprite;
        isOn = true;
        SetActiveLight(isOn);

        // if blink mode is BLINK, light blink
        if(blinkMode == BlinkMode.BLINK) {
            blinkCoroutine = Blink();
            StartCoroutine(blinkCoroutine);
        }
    }

    // Turn Off Light
    void TurnOff()
    {
        // stop blink coroutine
        if(blinkCoroutine != null && blinkMode == BlinkMode.BLINK) {
            StopCoroutine(blinkCoroutine);
            blinkCoroutine = null;
        }

        lampSpriteRenderer.sprite = offSprite;
        isOn = false;
        SetActiveLight(isOn);
    }

    // Active/Inactive Light
    void SetActiveLight(bool isActive)
    {
        sourceLight.gameObject.SetActive(isActive);
        _light.gameObject.SetActive(isActive);
        mask.gameObject.SetActive(isActive);
    }

    // Blink Light
    IEnumerator Blink()
    {
        float waitTime;

        while(isOn) {
            waitTime = Random.Range(0.25f, 0.5f);
            yield return new WaitForSeconds(waitTime);
            SetActiveLight(false);

            yield return null;
            yield return null;
            SetActiveLight(true);
        }

        yield return null;
    }
}
