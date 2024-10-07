using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] float maxSpeed;
    [SerializeField] float jumpImpulse;

    Rigidbody2D rb;
    SpriteRenderer sr;

    bool isLanding;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        isLanding = false;
    }

    void Update()
    {
        // Jump
        if (Input.GetButtonDown("Jump") && isLanding)
        {
            rb.AddForce(Vector2.up * jumpImpulse, ForceMode2D.Impulse);
            isLanding = false;
        }
    }

    void FixedUpdate()
    {
        // Get Move Input
        float h = Input.GetAxisRaw("Horizontal");

        // Calculate Velocity
        rb.velocity = new Vector2(h * maxSpeed, rb.velocity.y);
        Flip();

        // Landing Platform
        if (rb.velocity.y < 0)
        {
            float rayLength = sr.bounds.size.y / 2 + 0.5f;
            RaycastHit2D rayHit = Physics2D.Raycast(rb.position, Vector2.down, rayLength, LayerMask.GetMask("Platform") | LayerMask.GetMask("Shadow Platform"));
            if (!isLanding && rayHit.collider != null)
            {
                isLanding = true;
            }

            Debug.DrawRay(rb.position, Vector2.down, rayHit.collider != null ? Color.green : Color.red);
        }
    }

    void Flip()
    {
        // Flip Sprite
        if (rb.velocity.x != 0.0f)
        {
            sr.flipX = rb.velocity.x < 0.0f;
        }
    }
}
