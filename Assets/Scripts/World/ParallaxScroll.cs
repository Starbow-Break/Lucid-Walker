using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParallaxScroll : MonoBehaviour
{
    [SerializeField] Transform mainCamera; // 메인 카메라

    [Tooltip("수평 방향 무한 스크롤 사용 여부")]
    [SerializeField] bool horizontalInfiniteScroll; // 수평 무한 스크롤 사용 여부
    [Tooltip("수직 방향 무한 스크롤 사용 여부")]
    [SerializeField] bool verticalInfiniteScroll; // 수직 무한 스크롤 사용 여부

    [Range(-1.0f, 1.0f)]
    [Tooltip("수평 방향 평행 이동 강도")]
    [SerializeField] float horizontalAmount; // 수평 방향 평행 이동 강도

    [Range(-1.0f, 1.0f)]
    [Tooltip("수직 방향 평행 이동 강도")]
    [SerializeField] float verticalAmount; // 수직 방향 평행 이동 강도

    [Min(1.0f)]
    [Tooltip("수직 방향 스케일")]
    [SerializeField] float verticalUnitScale = 1.0f; // 수직 방향 스케일

    [Min(1.0f)]
    [Tooltip("수평 방향 스케일")]
    [SerializeField] float horizontalUnitScale = 1.0f; // 수평 방향 스케일

    Vector3 pos; // 기준 위치
    float width, height;

    void Awake()
    {
        pos = transform.position;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if(sr != null) {
            Vector2 renderSize = sr.bounds.size;
            width = renderSize.x * horizontalUnitScale;
            height = renderSize.y * verticalUnitScale;
        }
    }

    void Update()
    {
        Scroll();
    }

    protected virtual void Scroll() {
        Vector2 temp = new(mainCamera.transform.position.x * (1 - horizontalAmount), mainCamera.transform.position.y * (1 - verticalAmount));
        Vector2 dist = new(mainCamera.transform.position.x * horizontalAmount, mainCamera.transform.position.y * verticalAmount);

        transform.position = new(pos.x + dist.x, pos.y + dist.y, transform.position.z);

        if(horizontalInfiniteScroll) {
            if(temp.x > pos.x + width / 2) pos += Vector3.right * width;
            else if(temp.x < pos.x - width / 2) pos -= Vector3.right * width;
        }

        if(verticalInfiniteScroll) {
            if(temp.y > pos.y + height / 2) pos += Vector3.up * height;
            else if(temp.y < pos.y - height / 2) pos -= Vector3.up * height;
        }
    }
}
