using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Case : MonoBehaviour
{
    Rigidbody2D rb;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate() {
        // 상자에 수평으로 작용하는 힘이 없다면 정지
        if(rb.totalForce.x == 0.0f)
        {
            rb.velocity = new(0.0f, rb.velocity.y);
        }
    }
}
