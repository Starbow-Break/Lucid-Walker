using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DepthMap : MonoBehaviour
{
    [Range(0.0f, 1.0f)]
    [SerializeField] float zValue;

    [Range(0.0f, 1.0f)]
    [SerializeField] float alpha;

    private void OnValidate() {
        transform.localScale = new(zValue, zValue, transform.localScale.z);

        for(int i = 0; i < transform.childCount; i++) {
            Transform child = transform.GetChild(i);
            Tilemap tilemap = child.GetComponent<Tilemap>();
            if(tilemap != null) {
                tilemap.color = new Color(tilemap.color.r, tilemap.color.g, tilemap.color.b, alpha * zValue);
            }
            else {
                SpriteRenderer sr = child.GetComponent<SpriteRenderer>();
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, alpha * zValue);
            }
        }
    }
}
