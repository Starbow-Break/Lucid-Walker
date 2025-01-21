using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using Unity.VisualScripting;
using UnityEngine;

public class MonsterHouse : MonoBehaviour
{
    [SerializeField] List<GameObject> monsters; // 소환할 몬스터 들
    [SerializeField, Min(0)] int spawnMonstersNum = 1; // 스폰 할 몬스터의 수
    [SerializeField, Min(0.0f)] float gravity = 5.0f; // 집에 적용되는 중력
    [SerializeField] float landingYPos;
    [SerializeField] float spawnLength;

    Rigidbody2D rb;
    Animator anim;
    bool isReady;
    bool isDoorOpen;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        isReady = false;
        isDoorOpen = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(!isReady) {
            if(landingYPos >= transform.position.y) {
                transform.position += (landingYPos - transform.position.y) * Vector3.up;
                rb.velocity = Vector2.zero;
                isReady = true;
            }
            else {
                rb.velocity += gravity * Time.deltaTime * Vector2.down;
                Debug.Log("velocity : " + rb.velocity);
            }
        }
        else {
            if(!isDoorOpen) {
                DoorOpen();
            }
        }
    }

    // 문 열림
    void DoorOpen() {
        anim.SetTrigger("open");
    }

    // 문이 완전히 열리면 몬스터 소환
    public void SpawnMonsters() {
        for(int i = 0; i < spawnMonstersNum; i++) {
            int mIdx = Random.Range(0, monsters.Count);
            Vector3 spawnPosition = transform.position + Random.Range(-spawnLength/2, spawnLength/2) * Vector3.right;
            Instantiate(monsters[mIdx], spawnPosition, Quaternion.identity);
        }
    }

    private void OnDrawGizmos() {
        Gizmos.DrawLine(
            transform.position - spawnLength / 2 * Vector3.right,
            transform.position + spawnLength / 2 * Vector3.right
        );
    }
}
