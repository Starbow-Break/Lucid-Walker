using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    [SerializeField] float disappearTime = 2.0f; // 사라지는 시간
    [SerializeField] float spawnTime = 2.0f; // 스폰 시간
    [SerializeField] ParticleSystem particle = null; // 파티클
    bool isStepped = false; // 밟힌 상태
    Transform originalParent = null; // 밟은 플레이어의 원래 부모
    Animator anim; // 애니매이터
    Coroutine disappearCoroutine; // 현재 실행중인 Disappear 코루틴 
    Coroutine spawnCoroutine; // 현재 실행중인 Spawn 코루틴
    

    private void Awake() {
        anim = GetComponent<Animator>();
        particle?.Stop();
        disappearCoroutine = null;
        spawnCoroutine = null;
    }

    private void Start() {
        anim.SetFloat("steppingAnimSpeed", 1.0f / disappearTime);
    }

    private void Update() {
        if(isStepped) {
            float delta = -4.0f * Time.deltaTime * (transform.localPosition.y + 0.5f);
            transform.Translate(delta * Vector2.up, Space.Self);
        }
        else {
            float delta = -4.0f * Time.deltaTime * transform.localPosition.y;
            transform.Translate(delta * Vector2.up, Space.Self);
        }
        
    }

    // 밟으면 구름이 사라질 징조를 보여주고 일정 시간이 지난 후 사라진다.
    private void OnCollisionEnter2D(Collision2D other) {
        if(other.collider.CompareTag("Player")) {
            isStepped = true;
            originalParent = other.transform.parent;
            other.transform.parent = transform;
            particle?.Play();
            anim.SetTrigger("step");
            Disappear();
        }
    }

    private void OnCollisionExit2D(Collision2D other) {
        if(other.collider.CompareTag("Player")) {
            isStepped = false;
            other.transform.parent = originalParent;
            originalParent = null;
            particle?.Stop();
        }
    }

    // 구름이 사라짐
    public void Disappear() {
        if(disappearCoroutine == null) {
            disappearCoroutine = StartCoroutine(DisappearFlow());
        }
    }

    // 구름 생성
    public void Spawn() {
        if(spawnCoroutine == null) {
            spawnCoroutine = StartCoroutine(SpawnFlow());
        }
    }

    IEnumerator DisappearFlow() {
        yield return new WaitForSeconds(disappearTime);
        anim.ResetTrigger("step");
        anim.SetTrigger("disappear");
        disappearCoroutine = null;
    }

    IEnumerator SpawnFlow() {
        yield return new WaitForSeconds(spawnTime);
        anim.SetTrigger("spawn");
        spawnCoroutine = null;
    }
}
