using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

// 스포트라이트 스위치
public class SpotlightSwitch : MonoBehaviour
{
    [SerializeField] protected Spotlight spotlight; // 목표 스포트라이트

    protected bool isInteracting { get; private set; } = false;
    protected SpriteRenderer sr { get; private set;} // 스위치 스프라이트

    protected virtual void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void OnTriggerEnter2D(Collider2D other) {
        if(!isInteracting && other.gameObject.CompareTag("Player")) {
            isInteracting = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(isInteracting && other.gameObject.CompareTag("Player")) {
            isInteracting = false;
        }
    }
}
