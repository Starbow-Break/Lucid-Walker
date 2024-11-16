using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    [SerializeField] Vector2 center = Vector2.zero; // 중심
    [SerializeField] Vector2 size = Vector2.one; // 크기

    public Vector2 boundMin => center - size / 2; // 맵 경계 좌하단
    public Vector2 boundMax => center + size / 2; // 맵 경계 우상단

    private void OnDrawGizmos() {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube((boundMax + boundMin) / 2, boundMax - boundMin);
    }
}
