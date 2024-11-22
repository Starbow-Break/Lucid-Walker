using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

enum MoveMode
{
    NONE, FOLLOW_TARGET, SWITCH
}

enum BlinkMode
{
    NONE
}

public class Spotlight : MonoBehaviour
{   
    [Header("Mode")]
    [SerializeField] MoveMode moveMode;
    [SerializeField] BlinkMode blinkMode;
    [SerializeField] bool fall;
    
    [Header("Target")]
    [Range(0.0f, 360.0f)]
    [SerializeField] float hitDeg = 0.0f; // 스포트라이트가 플레이어를 감지하는 시야각
    public bool isOn; // 켜짐 여부

    [Header("Switch")]
    [SerializeField] int startIndex = 0;
    [SerializeField] float[] rotValue;
    int switchIndex;

    [Header("Light")]
    [SerializeField] Light2D _light; // 불빛
    [SerializeField] float innerSpot; // Inner Spot
    [SerializeField] float outerSpot; // Outer Spot
    [SerializeField] float lightRadius; // 빛의 반지름
    [SerializeField] CircularSectorMesh mask; // Mask

    [Header("Lamp")]
    [SerializeField] Transform lamp;
    [SerializeField] Sprite onSprite; // 점등 시 스프라이트
    [SerializeField] Sprite offSprite; // 점멸 시 스프라이트
    [SerializeField] Light2D sourceLight; // 광원 불빛

    [Header("Fall")]
    [SerializeField] Collider2D fallCollider; // 낙하 감지 Collider
    [Range(0.0f, 180.0f)]
    [SerializeField] float angle; // 흔들리는 각
    [SerializeField] float period; // 흔들리는 주기
    [SerializeField] GameObject fallenLampPrefab; // 떨어지는 램프
    [SerializeField] float gravityScale; // 낙하 시 중력 스케일
    public bool isBroken { get; private set; } = false; // 피손 여부
    float swingTime = 0.0f;
    
    Vector2 initDir;
    Vector2 lightDir;
    SpriteRenderer lampSpriteRenderer; // 전등 이미지에 사용되는 렌더러

    private void OnValidate() {
        if(0.0f <= lightRadius && 0.0f <= innerSpot && outerSpot <= 360.0f && innerSpot <= outerSpot) {
            _light.pointLightInnerAngle = innerSpot;
            _light.pointLightOuterAngle = outerSpot;
            _light.pointLightOuterRadius = lightRadius;
        }
    }

    void Awake()
    {
        lampSpriteRenderer = lamp.GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Get initial Direction (-Y dir)
        initDir = lamp.rotation * Vector2.down;
        lightDir = initDir;

        // Setting Light
        _light.pointLightInnerAngle = innerSpot;
        _light.pointLightOuterAngle = outerSpot;
        _light.pointLightOuterRadius = lightRadius;

        // Setting Mask
        mask.SetRadius(lightRadius);
        mask.SetAngle(outerSpot);
        mask.Generate();

        // initialize switch index
        if(moveMode == MoveMode.SWITCH) {
            switchIndex = startIndex;
        }

        if(fall) {
            StartCoroutine(Swing());
        }

        // Turn On/Off according to isOn value
        if(isOn) TurnOn(); else TurnOff();
    }

    void FixedUpdate()
    {
        // rotate spotlight
        if(isOn) {
            // if move mode is FOLLOW_TARGET, rotate spotlight to target
            if(moveMode == MoveMode.FOLLOW_TARGET) {
                // Check Overlap Player
                Collider2D collider = Physics2D.OverlapCircle(lamp.position, lightRadius, LayerMask.GetMask("Player"));
                
                // Follow Player
                if(collider != null) {
                    Rotate(collider.gameObject);
                }
            }
            // if move mode is SWITCH, rotate spotlight to next rotation value
            else if(moveMode == MoveMode.SWITCH) {
                Rotate(rotValue[switchIndex]);
            }
        }
    }

    // rotate to target
    void Rotate(GameObject target)
    {
        // Follow Target
        Vector2 nextDir = (target.transform.position - lamp.position).normalized;

        float deg = Mathf.Rad2Deg * Mathf.Acos(Vector2.Dot(initDir, nextDir));

        if(deg <= hitDeg / 2)
        {
            float curRotZ = Mathf.Rad2Deg * Mathf.Atan2(lightDir.x, -lightDir.y);
            float nextRotZ = Mathf.Rad2Deg * Mathf.Atan2(nextDir.x, -nextDir.y);

            float diffRot = ((nextRotZ - curRotZ) % 360.0f + 360.0f) % 360.0f;
            diffRot = diffRot > 180.0f ? diffRot - 360.0f : diffRot;

            float targetRotZ = curRotZ + diffRot * 5.0f * Time.fixedDeltaTime;

            lamp.rotation = Quaternion.Euler(0, 0, targetRotZ);
            lightDir = new(Mathf.Sin(targetRotZ * Mathf.Deg2Rad), -Mathf.Cos(targetRotZ * Mathf.Deg2Rad));
        }
    }

    // rotate while euler z == zrot
    void Rotate(float zRot)
    {
        // Follow Target
        Vector2 nextDir = new(Mathf.Sin(zRot * Mathf.Deg2Rad), -Mathf.Cos(zRot * Mathf.Deg2Rad));
        nextDir = transform.rotation * nextDir;

        float curRotZ = Mathf.Rad2Deg * Mathf.Atan2(lightDir.x, -lightDir.y);
        float nextRotZ = Mathf.Rad2Deg * Mathf.Atan2(nextDir.x, -nextDir.y);

        float diffRot = ((nextRotZ - curRotZ) % 360.0f + 360.0f) % 360.0f;
        diffRot = diffRot > 180.0f ? diffRot - 360.0f : diffRot;

        float targetRotZ = curRotZ + diffRot * 5.0f * Time.fixedDeltaTime;

        lamp.rotation = Quaternion.Euler(0, 0, targetRotZ);
        lightDir = new(Mathf.Sin(targetRotZ * Mathf.Deg2Rad), -Mathf.Cos(targetRotZ * Mathf.Deg2Rad));
    }

    // Switch Light
    public void Switch()
    {
        if(isOn) TurnOff(); else TurnOn();
    }

    public void RotateSwitch()
    {
        switchIndex = (switchIndex + 1) % rotValue.Length;
    }

    // Turn On Light
    public void TurnOn()
    {
        isOn = true;
        SetActiveLight(isOn);
    }

    // Turn Off Light
    public void TurnOff()
    {
        isOn = false;
        SetActiveLight(isOn);
    }

    // Active/Inactive Light
    void SetActiveLight(bool isActive)
    {
        lampSpriteRenderer.sprite = isActive ? onSprite : offSprite;
        sourceLight.gameObject.SetActive(isActive);
        _light.gameObject.SetActive(isActive);
        mask.gameObject.SetActive(isActive);
    }

    // swing Lamp
    IEnumerator Swing()
    {
        Quaternion start = Quaternion.Euler(0.0f, 0.0f, -angle);
        Quaternion end = Quaternion.Euler(0.0f, 0.0f, angle);

        while(!isBroken) {
            swingTime += Time.deltaTime;
            lamp.rotation = Quaternion.Lerp(start, end, (Mathf.Sin(2 * Mathf.PI * swingTime / period) + 1.0f) / 2.0f);
            yield return null;
        }

        yield return null;
    }

    // Fall Lamp
    public void Fall()
    {
        isBroken = true;
        GameObject fallenLamp = Instantiate(fallenLampPrefab, lamp.position, lamp.rotation);
        fallenLamp.GetComponent<Rigidbody2D>().gravityScale = gravityScale;
        Destroy(lamp.gameObject);
    }
}
