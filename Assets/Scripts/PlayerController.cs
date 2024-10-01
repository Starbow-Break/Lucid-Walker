using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;

    [SerializeField]
    private float power;
    [SerializeField]
    Transform pos;
    [SerializeField]
    float checkRadius;
    [SerializeField]
    LayerMask isLayer;

    public int jumpCount;
    int jumpCnt;

    bool isGround;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        jumpCnt = jumpCount;
    }

    private void Update()
    {
        // (Vector2 point, float radius, int layer) 
        isGround = Physics2D.OverlapCircle(pos.position, checkRadius, isLayer);
        if (isGround == true && Input.GetKeyDown(KeyCode.Space) && jumpCnt > 0)
        {
            rb.velocity = Vector2.up * power;
        }
        if (isGround == false && Input.GetKeyDown(KeyCode.Space) && jumpCnt > 0)
        {
            rb.velocity = Vector2.up * power;
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            jumpCnt--;
        }
        if (isGround)
        {
            jumpCnt = jumpCount;
        }
    }

    private void FixedUpdate()
    {
        float hor = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(hor * 3, rb.velocity.y);

        // hor left -1, right 1
        if (hor > 0)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else if (hor < 0)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
    }
}