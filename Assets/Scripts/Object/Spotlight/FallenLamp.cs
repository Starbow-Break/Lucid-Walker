using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallenLamp : MonoBehaviour
{
    [Header("Spawn Object")]
    [SerializeField] GameObject spike; // 스폰될 가시

    [Header("Effect")]
    [Min(0.0f), SerializeField] float scaleMultiplier = 1.0f; // 스케일
    [SerializeField] Vector2 offset = Vector2.zero; // 오프셋
    [SerializeField] GameObject effect; // 효과

    private void OnTriggerEnter2D(Collider2D other) {
        Vector2 spawnPoint = new(transform.position.x, Mathf.Floor(transform.position.y));
        
        // 충돌 위치에 가시 및 연기 생성
        GameObject spawnedEffect = Instantiate(effect, spawnPoint + offset, Quaternion.identity);
        spawnedEffect.transform.localScale *= scaleMultiplier;

        Instantiate(spike, spawnPoint, Quaternion.identity);

        // 조명 제거
        Destroy(gameObject);
    }
}
