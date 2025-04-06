using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BreakOnImpact : MonoBehaviour
{
    private ParticleSystem[] particles;
    private SpriteRenderer sr;
    private PolygonCollider2D pc;
    const int ROCK_DAMAGE = 1;

    private void Awake()
    {
        // 모든 자식 ParticleSystem들을 가져오기
        particles = GetComponentsInChildren<ParticleSystem>();
        sr = GetComponent<SpriteRenderer>();
        pc = GetComponent<PolygonCollider2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            Debug.Log("닿음");
            StartCoroutine(Break());
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("플레이어에 닿음");

            IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
            damageable?.TakeDamage(ROCK_DAMAGE, transform);

            StartCoroutine(Break());
        }
    }

    private IEnumerator Break()
    {
        // 모든 파티클 시스템 재생
        foreach (ParticleSystem ps in particles)
        {
            ps.Play();
        }

        sr.enabled = false;
        pc.enabled = false;

        // 가장 긴 startLifetime 기준으로 기다림
        float maxLifetime = 0f;
        foreach (ParticleSystem ps in particles)
        {
            if (ps.main.startLifetime.constantMax > maxLifetime)
            {
                maxLifetime = ps.main.startLifetime.constantMax;
            }
        }

        yield return new WaitForSeconds(maxLifetime);
        Destroy(gameObject);
    }
}
