using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxScroll : MonoBehaviour
{
    [SerializeField] Transform mainCamera; // 메인 카메라

    [Range(0.0f, 1.0f)]
    [Tooltip("수평 방향 평행 이동 강도")]
    [SerializeField] float horizontalAmount; // 수평 방향 평행 이동 강도

    [Range(0.0f, 1.0f)]
    [Tooltip("수직 방향 평행 이동 강도")]
    [SerializeField] float verticalAmount; // 직 방향 평행 이동 강도

    Vector3 pos; // 기준 위치
    float width, height;

    void Awake()
    {
        pos = transform.position;
        Vector2 renderSize = GetComponent<SpriteRenderer>().bounds.size;
        Debug.Log(renderSize);
        width = renderSize.x;
        height = renderSize.y;
    }

    void Update()
    {
        Vector2 temp = new(mainCamera.transform.position.x * (1 - horizontalAmount), mainCamera.transform.position.y * (1 - verticalAmount));
        Vector2 dist = new(mainCamera.transform.position.x * horizontalAmount, mainCamera.transform.position.y * verticalAmount);

        transform.position = new(pos.x + dist.x, pos.y + dist.y, transform.position.z);

        if(temp.x > pos.x + width / 2) pos += Vector3.right * width;
        else if(temp.x < pos.x - width / 2) pos -= Vector3.right * width;

        if(temp.y > pos.y + height / 2) pos += Vector3.up * height;
        else if(temp.y < pos.y - height / 2) pos -= Vector3.up * height;
    }
}
