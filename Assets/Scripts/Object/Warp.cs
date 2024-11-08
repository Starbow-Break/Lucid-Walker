using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Warp : MonoBehaviour
{
    [SerializeField] Tilemap currentMap; // 현재 위프가 있는 타일 맵
    [SerializeField] Tilemap targetMap; // 워프 지점 타일 맵
    [SerializeField] Warp targetWarp; // 목표 Warp
    [SerializeField] List<GameObject> activeObjects; // 워프 후 활성화 할 오브젝트
    [SerializeField] List<GameObject> inactiveObjects; // 워프 후 비활성화 할 오브젝트

    GameObject interactingPlayer = null; // 상호작용 중인 오브젝트

    protected Color targetTintColor; // 워프 이후 Tint Color

    protected virtual void Awake()
    {
        targetTintColor = targetMap.gameObject.GetComponent<Tilemap>().color;
    }

    void Update() {
        if(interactingPlayer != null && Input.GetKeyDown(KeyCode.Z)) {
            StartCoroutine(WarpTarget(interactingPlayer));
        }
    }

    IEnumerator WarpTarget(GameObject warpTarget) {
        // 플레이어가 가지고 있는 컴포넌트들
        PlayerController pc = warpTarget.GetComponent<PlayerController>();
        Animator anim = warpTarget.GetComponent<Animator>();
        Rigidbody2D rb = warpTarget.GetComponent<Rigidbody2D>();

        // 워프 위치 오프셋
        Vector2 offset = warpTarget.transform.position - transform.position;

        // 컨트롤러 비활성화, 애니메이션 일시정지, RigidBody는 Kinematic으로 설정
        if(pc != null) {
            pc.enabled = false;
        }
        if(anim != null) {
            anim.speed = 0.0f;
        }
        if(rb != null) {
            rb.isKinematic = true;
            rb.velocity = Vector2.zero;
        }

        // 워프 애니메이션
        yield return WarpInAnim(warpTarget);

        // 활성화할 오브젝트들을 먼저 활성화
        foreach(GameObject obj in activeObjects) {
            obj.SetActive(true);
        }

        // 워프 대상 오브젝트의 pivot에 맞춰서 offset 계산
        SpriteRenderer sr = warpTarget.GetComponent<SpriteRenderer>();
        
        // 워프 대상 오브젝트를 목표 타일맵 및 목표 위치로 이동
        warpTarget.transform.parent = targetMap.transform.parent;
        warpTarget.transform.localScale = targetMap.transform.localScale;
        warpTarget.transform.localPosition = targetWarp.transform.localPosition + targetWarp.transform.rotation * offset;

        // Tint 컬러 변경
        sr.color = targetTintColor;

        foreach(GameObject obj in inactiveObjects) {
            obj.SetActive(false);
        }

        // Collider 설정
        DeactiveChildColliders(currentMap.transform.parent.gameObject);
        ActiveChildColliders(targetMap.transform.parent.gameObject);

        // 워프 애니메이션
        yield return targetWarp.WarpOutAnim(warpTarget);

        // 컨트롤러 활성화, 애니메이션 재시작, RigidBody는 Dynamic으로 설정
        if(pc != null) {
            pc.enabled = true;
        }
        if(anim != null) {
            anim.speed = 1.0f;
        }
        if(rb != null) {
            rb.isKinematic = false;
        }
    }

    protected virtual IEnumerator WarpInAnim(GameObject warpTarget) { yield return null; }
    protected virtual IEnumerator WarpOutAnim(GameObject warpTarget) { yield return null; }

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
