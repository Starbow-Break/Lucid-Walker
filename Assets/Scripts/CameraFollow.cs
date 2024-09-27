using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform target;

    void Start()
    {
        transform.position = new(target.position.x, target.position.y, transform.position.z);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = new(target.position.x, target.position.y, transform.position.z);
    }
}
