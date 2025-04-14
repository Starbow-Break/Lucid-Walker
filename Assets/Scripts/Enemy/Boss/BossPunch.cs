using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BossPunch : MonoBehaviour
{
    [SerializeField] float punchSpeed = 10.0f;
    [SerializeField] float liftSpeed = 3.0f;

    [Header("Camera Shake")]
    [SerializeField] float shakeIntensity = 5.0f;
    [SerializeField] float shakeTime = 0.1f;

    public UnityAction OnDestroyed;

    Animator anim;
    float startY;
    bool isFinish;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void OnEnable()
    {
        startY = transform.position.y;
        isFinish = false;
        StartCoroutine(AttackSequence());
    }

    IEnumerator AttackSequence()
    {
        // 위치 이동

        // 주먹 쥐기
        anim.SetTrigger("make_fist");
        yield return new WaitForSeconds(1.0f);

        // 주먹 흔들기
        anim.SetTrigger("shake");
        yield return new WaitForSeconds(1.0f);

        // 내려찍기
        anim.SetTrigger("punch");
        yield return PunchSequence();

        // 주먹 올리기
        anim.SetTrigger("attack_finish");
        yield return FinishSequence();

        // 종료
        OnDestroyed?.Invoke();
        Destroy(gameObject);
    }

    IEnumerator PunchSequence()
    {
        while(!isFinish)
        {
            transform.Translate(punchSpeed * Time.deltaTime * Vector2.down);
            yield return null;
        }
    }

    IEnumerator FinishSequence()
    {
        while(transform.position.y <= startY)
        {
            transform.Translate(liftSpeed * Time.deltaTime * Vector2.up);
            yield return null;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Platform"))
        {
            CameraShake.instance.ShakeActiveCamera(shakeIntensity, shakeTime);
            isFinish = true;
        }
    }
}
