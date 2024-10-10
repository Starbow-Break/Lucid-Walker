using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamage : MonoBehaviour, IDamageable
{
    public int health = 10;  // 플레이어의 초기 체력
    bool isHurt;
    SpriteRenderer sr;
    Color halfA = new Color(1, 1, 1, 0.5f);
    Color fullA = new Color(1, 1, 1, 1);
    public float speed;
    private Animator anim;     // Animator 참조
    private Rigidbody2D rb;    // Rigidbody2D 참조
    bool isKnockBack = false;
    private void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(int damage, Transform attacker)
    {
        Debug.Log("TakeDamage called"); // 디버그 로그 추가
        if (!isHurt)
        {
            isHurt = true;
            health -= damage;

            if (health <= 0)
            {
                // 죽음 처리
                Debug.Log("Player Died"); // 죽음 처리 확인용 로그

            }
            else
            {
                Debug.Log("Player took damage: " + damage); // 데미지 확인용 로그
                float x = transform.position.x - attacker.position.x;
                x = x < 0 ? 1 : -1;
                StartCoroutine(DamageRoutine(x));
            }
        }
    }

    IEnumerator DamageRoutine(float dir)
    {
        // Knockback 처리
        isKnockBack = true;
        float ctime = 0;
        anim.SetTrigger("hurt");
        while (ctime < 0.2f)
        {
            if (transform.rotation.y == 0)
                transform.Translate(Vector2.left * speed * Time.deltaTime * dir);
            else
                transform.Translate(Vector2.left * speed * Time.deltaTime * -1f * dir);

            ctime += Time.deltaTime;
            yield return null;
        }
        isKnockBack = false;

        // Alpha Blink 처리
        float blinkDuration = 2f;  // HurtRoutine에서 대기할 시간
        float blinkTime = 0;
        while (blinkTime < blinkDuration)
        {
            sr.color = halfA;
            yield return new WaitForSeconds(0.1f);
            sr.color = fullA;
            yield return new WaitForSeconds(0.1f);

            blinkTime += 0.2f; // 두 번의 대기 시간 더함
        }

        // HurtRoutine 종료 처리
        isHurt = false;
    }
}
