using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

public class Spotlight : MonoBehaviour
{   
    [Range(0.0f, 360.0f)]
    [SerializeField] float hitDeg;
    [SerializeField] float hitRadius;
    [SerializeField] bool isOn;
    [SerializeField] Light2D _light;
    [SerializeField] float lightDeg;
    [SerializeField] float lightRadius;
    [SerializeField] CircularSectorMesh mask;
    
    Vector2 initDir;
    Vector2 lightDir;

    // Start is called before the first frame update
    void Start()
    {
        // Get initial Direction (+Y dir)
        initDir = transform.rotation * Vector2.up;
        lightDir = initDir;

        // Setting Light
        _light.pointLightInnerAngle = lightDeg;
        _light.pointLightOuterAngle = lightDeg;
        _light.pointLightOuterRadius = lightRadius;
        _light.gameObject.SetActive(isOn);

        // Setting Mask
        mask.SetRadius(lightRadius);
        mask.SetAngle(lightDeg);
        mask.gameObject.SetActive(isOn);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        // Rotate SpotLight
        if(isOn) {
            // Check Overlap Player
            Collider2D collider = Physics2D.OverlapCircle(transform.position, hitRadius, LayerMask.GetMask("Player"));
            
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

        if(deg <= hitDeg)
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
        if(isOn) {
            TurnOff();
        }
        else {
            TurnOn();
        }
    }

    // Turn On Light
    void TurnOn()
    {
        _light.gameObject.SetActive(true);
        mask.gameObject.SetActive(true);
        isOn = true;
    }

    // Turn Off Light
    void TurnOff()
    {
        _light.gameObject.SetActive(false);
        mask.gameObject.SetActive(false);
        isOn = false;
    }
}
