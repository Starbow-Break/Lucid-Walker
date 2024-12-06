using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    [Header("Cloud")]
    [SerializeField] ParticleSystem particle = null; // 파티클
    [SerializeField] float sinkDistance = 0.5f; // 가라앉는 거리
    [SerializeField] float sinkSpeed = 4.0f; // 가라앉는 속도

    bool isStepped = false; // 밟힌 상태
    Transform originalParent = null; // 밟은 플레이어의 원래 부모
    Vector2 initLocalPosition;

    protected virtual void Awake() {
        initLocalPosition = transform.localPosition;
        particle?.Stop();
    }

    private void Update() {
        if(isStepped) {
            float delta = sinkSpeed * Time.deltaTime * (initLocalPosition.y - sinkDistance - transform.localPosition.y);
            transform.Translate(delta * Vector2.up, Space.Self);
        }
        else {
            float delta = sinkSpeed * Time.deltaTime * (initLocalPosition.y - transform.localPosition.y);
            transform.Translate(delta * Vector2.up, Space.Self);
        }
        
    }

    // 밟으면 구름이 사라질 징조를 보여주고 일정 시간이 지난 후 사라진다.
    protected virtual void OnCollisionEnter2D(Collision2D other) {
        if(other.collider.CompareTag("Player")) {
            isStepped = true;
            originalParent = other.transform.parent;
            other.transform.parent = transform;
            particle?.Play();
        }
    }

    private void OnCollisionExit2D(Collision2D other) {
        if(other.collider.CompareTag("Player")) {
            isStepped = false;
            other.transform.parent = originalParent;
            originalParent = null;
            particle?.Stop();
        }
    }
}
