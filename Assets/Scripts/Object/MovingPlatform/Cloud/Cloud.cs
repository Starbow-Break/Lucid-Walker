using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Cloud : MovingPlatform
{
    [Header("Cloud")]
    [SerializeField] ParticleSystem particle = null; // 파티클
    [SerializeField] float sinkDistance = 0.5f; // 가라앉는 거리
    [SerializeField] float sinkSpeed = 4.0f; // 가라앉는 속도

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

    // 밟으면 밟은 위치에 구름 파티클을 생성한다.
    protected override void OnCollisionEnter2D(Collision2D other) {
        if(other.collider.CompareTag("Player")) {
            base.OnCollisionEnter2D(other);

            if(particle != null) {
                ContactPoint2D contact = other.contacts[0];
                Vector3 landingPoint = contact.point;

                ParticleSystem spawnedParticle = Instantiate(particle, landingPoint, Quaternion.identity, transform.parent);
                Destroy(spawnedParticle.gameObject, spawnedParticle.main.duration);
            }
        } 
    }
}
