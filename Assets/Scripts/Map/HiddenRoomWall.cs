using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HiddenRoomWall : MonoBehaviour
{
    [SerializeField] float duration = 1.0f;
    Tilemap tilemap;
    float alpha = 1.0f;
    IEnumerator coroutine = null;

    void Awake() {
        tilemap = GetComponent<Tilemap>();
        alpha = tilemap.color.a;
    }

    public void Enter()
    {
        if(coroutine != null) {
            StopCoroutine(coroutine);
            coroutine = null;
        }

        coroutine = EnterRoutine();
        StartCoroutine(coroutine);
    }

    public void Exit()
    {
        if(coroutine != null) {
            StopCoroutine(coroutine);
            coroutine = null;
        }

        coroutine = ExitRoutine();
        StartCoroutine(coroutine);
    }

    IEnumerator EnterRoutine()
    {
        while(alpha > 0.0f) {
            alpha = Mathf.Max(0.0f, alpha - Time.deltaTime / duration);
            tilemap.color = new Color(tilemap.color.r, tilemap.color.g, tilemap.color.b, alpha);
            yield return null;
        }
    }

    IEnumerator ExitRoutine()
    {
        while(alpha < 1.0f) {
            alpha = Mathf.Min(1.0f, alpha + Time.deltaTime / duration);
            tilemap.color = new Color(tilemap.color.r, tilemap.color.g, tilemap.color.b, alpha);
            yield return null;
        }
    }
}
