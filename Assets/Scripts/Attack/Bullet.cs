using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Animator anim;
    public float speed;
    public float distance;
    public LayerMask isLayer;
    void Start()
    {
        anim = GetComponent<Animator>();
        Invoke("DestroyBullet", 2f);
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit2D ray = Physics2D.Raycast(transform.position, transform.right, distance, isLayer);
        if (ray.collider != null)
        {
            anim.SetTrigger("Hit");
            if (ray.collider.tag == "Enemy")
            {
                Debug.Log("명중!");
            }
            DestroyBullet();
        }
        if (transform.rotation.y == 0)
        {
            transform.Translate(transform.right * speed * Time.deltaTime);
        }
        else
        {
            transform.Translate(transform.right * -1 * speed * Time.deltaTime);
        }
    }
    void DestroyBullet()
    {
        Destroy(gameObject);
    }

}
