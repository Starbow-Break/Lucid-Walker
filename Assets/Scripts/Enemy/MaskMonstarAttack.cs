using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskMonstarAttack : MonoBehaviour
{
    [System.Serializable]
    public struct AttackRangeData {
        public Vector2 center;
        public float rotation;
        public Vector2 size;
    }

    [SerializeField] MaskMonster maskMonster;
    [SerializeField] List<AttackRangeData> attackRange; // 공격 범위

    // 공격
    // attackRange[index]에 해당하는 공격 범위로 플레이어를 공격
    private void Attack(int index) {
        AttackRangeData ard = attackRange[index];

        RaycastHit2D hit = Physics2D.BoxCast(
            maskMonster.isRight * ard.center + Vector2.right * transform.position.x + Vector2.up *  transform.position.y,
            ard.size,
            maskMonster.isRight * ard.rotation,
            Vector2.right,
            0.0f,
            LayerMask.GetMask("Player")
        );

        if(hit.collider != null) {
            IDamageable damageable = hit.collider.GetComponent<IDamageable>();
            if(damageable != null) {
                damageable.TakeDamage(1, transform);
            }
        }
    }

    private void OnDrawGizmos() {
        // 공격 범위 확인
        foreach(AttackRangeData ard in attackRange)
        {
            Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position + maskMonster.isRight * (Vector3)ard.center, Quaternion.Euler(0f, 0f, maskMonster.isRight * ard.rotation), Vector3.one);
            Gizmos.matrix = rotationMatrix;
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube(Vector3.zero, ard.size);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player"))
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            if(damageable != null) {
                damageable.TakeDamage(1, transform);
            }
        }
    }
}
