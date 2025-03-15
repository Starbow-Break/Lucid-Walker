using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOutHandler : MonoBehaviour
{
    [SerializeField] private float fadeSpeed;
    [SerializeField] private int destroyDistance;
    [SerializeField] private int forceFadeTimer;

    private Rigidbody2D rb;
    private bool startFade;
    private int forceFade;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        transform.Rotate(0, 0, 90 * Random.Range(0, 4));
    }

    private void LateUpdate()
    {
        // float distanceToCamera = Vector2.Distance(transform.position, Camera.main.transform.position);
        // if (distanceToCamera > destroyDistance)
        // {
        //     // 풀링 대상이라면 Destroy 대신 풀로 반환
        //     // ObjectPooler.Instance.ReturnPooledObject(gameObject);
        // }
        if (startFade)
        {
            // Time.deltaTime을 곱해 프레임 독립적인 점진적 감소 적용
            SpriteRenderer sr = transform.GetChild(0).GetComponent<SpriteRenderer>();
            Color color = sr.color;
            color.a = Mathf.Clamp01(color.a - fadeSpeed * Time.deltaTime);
            sr.color = color;

            if (color.a <= 0)
            {
                ObjectPooler.Instance.ReturnPooledObject(gameObject);
            }
        }
        else
        {
            if (rb != null && rb.velocity.x == 0) startFade = true;
            if (forceFade < forceFadeTimer) forceFade++;
            else startFade = true;
        }
    }

}
