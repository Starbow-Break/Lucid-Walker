using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Warp : MonoBehaviour
{
    [SerializeField] Tilemap currentMap; // 현재 위프가 있는 타일 맵
    [SerializeField] Tilemap targetMap; // 워프 지점 타일 맵
    [SerializeField] Warp targetWarp; // 목표 Warp

    GameObject interactingPlayer = null; // 상호작용 중인 오브젝트

    protected Color targetTintColor; // 워프 이후 Tint Color

    protected virtual void Awake()
    {
        targetTintColor = targetMap.gameObject.GetComponent<Tilemap>().color;
    }

    void Update() {
        if(interactingPlayer != null && Input.GetKeyDown(KeyCode.Z)) {
            StartCoroutine(WarpTarget());
        }
    }

    IEnumerator WarpTarget() {
        yield return new WaitForCoroutines(this, WarpInAnim(), targetWarp.WarpInAnim());

        // Player의 pivot에 맞춰서 offset 계산
        SpriteRenderer sr = interactingPlayer.GetComponent<SpriteRenderer>();
        Vector2 bound = new(sr.bounds.max.x - sr.bounds.min.x, sr.bounds.max.y - sr.bounds.min.y);
        Vector3 offset = new(0.0f, bound.y * sr.sprite.pivot.y / sr.sprite.rect.height, 0.0f);
        
        // 플레이어를 목표 타일맵 및 목표 위치로 이동
        interactingPlayer.transform.parent = targetMap.transform.parent;
        interactingPlayer.transform.localScale = targetMap.transform.localScale;
        interactingPlayer.transform.localPosition = targetWarp.transform.localPosition + targetWarp.transform.rotation * offset;

        // Tint 컬러 변경
        interactingPlayer.GetComponent<SpriteRenderer>().color = targetTintColor;

        // Collider 설정
        DeactiveChildColliders(currentMap.transform.parent.gameObject);
        ActiveChildColliders(targetMap.transform.parent.gameObject);
        
        yield return new WaitForCoroutines(this, WarpOutAnim(), targetWarp.WarpOutAnim());
    }

    protected virtual IEnumerator WarpInAnim()
    {
        yield return null;
    }

    protected virtual IEnumerator WarpOutAnim()
    {
        yield return null;
    }

    // 해당 오브젝트의 자식 오브젝트들의 collider를 활성화
    void ActiveChildColliders(GameObject obj) {
        for(int i = 0; i < obj.transform.childCount; i++) {
            Collider2D collider = obj.transform.GetChild(i).GetComponent<Collider2D>();
            if(collider != null) {
                collider.enabled = true;
            }
        }
    }

    // 해당 오브젝트의 자식 오브젝트들의 collider를 비활성화
    void DeactiveChildColliders(GameObject obj) {
        for(int i = 0; i < obj.transform.childCount; i++) {
            Collider2D collider = obj.transform.GetChild(i).GetComponent<Collider2D>();
            if(collider != null) {
                collider.enabled = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player")) {
            interactingPlayer = other.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if(other.CompareTag("Player")) {
            interactingPlayer = null;
        }
    }
}
