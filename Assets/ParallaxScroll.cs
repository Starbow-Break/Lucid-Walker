using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxScroll : MonoBehaviour
{
    [SerializeField] Transform mainCamera; // 메인 카메라

    [Range(0.0f, 1.0f)]
    [SerializeField] float amount; // 카메라 기준 이동 강도

    void Update()
    {
        transform.position = -mainCamera.transform.position * (1 - amount);
    }
}
