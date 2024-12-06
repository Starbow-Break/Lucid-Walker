using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class Warp : MonoBehaviour
{
    [SerializeField] Map map; // Warp가 속한 Map
    [SerializeField] Warp targetWarp; // 목표 Warp

    GameObject interactingPlayer = null; // 상호작용 중인 오브젝트

    public Map GetMap() => map;

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
        
        // 워프 대상 오브젝트를 목표 타일맵 및 목표 위치로 이동
        warpTarget.transform.parent = targetWarp.map.transform;
        warpTarget.transform.localPosition = targetWarp.transform.localPosition + targetWarp.transform.rotation * offset;

        // 워프 애니메이션
        yield return targetWarp.WarpOutAnim(warpTarget);

        // 컨트롤러 활성화, 애니메이션 재생, RigidBody는 Dynamic으로 설정
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

    protected abstract IEnumerator WarpInAnim(GameObject warpTarget);
    protected abstract IEnumerator WarpOutAnim(GameObject warpTarget);

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
