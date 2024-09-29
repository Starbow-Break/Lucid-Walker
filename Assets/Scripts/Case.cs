using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Case : MonoBehaviour
{
    [Tooltip("True로 설정 시 케이스를 이동시킬 수 있다.")]
    [SerializeField] bool isMovable;

    Rigidbody2D rb; // RigidBody

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = !isMovable;
    }

    void FixedUpdate()
    {
        if(rb.totalForce == Vector2.zero)
        {
            rb.velocity = new(0.0f, rb.velocity.y);
        }
    }
}
