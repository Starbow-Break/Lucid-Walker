using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float maxSpeed;
    [SerializeField] float jumpImpulse;

    Rigidbody2D rb;
    SpriteRenderer sr;
    Animator anim;

    bool isLanding;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
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

        // Animation
        if (Mathf.Abs(rb.velocity.x) < 0.3)
            anim.SetBool("isWalking", false);
        else
            anim.SetBool("isWalking", true);
    }

    void FixedUpdate()
    {
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