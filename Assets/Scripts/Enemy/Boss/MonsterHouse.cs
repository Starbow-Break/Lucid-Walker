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
    [SerializeField] float landingYPos; // 착지지점 y좌표
    [SerializeField] Vector2 spawnRangeCenter; // 스폰 범위 중심
    [SerializeField] Vector2 spawnRangeSize; // 스폰 범위 크기

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
            // 몬스터 소환
            int mIdx = Random.Range(0, monsters.Count);
            Vector3 spawnPosition 
                = (spawnRangeCenter.x + Random.Range(-spawnRangeSize.x / 2, spawnRangeSize.x / 2)) * Vector3.right
                    + (spawnRangeCenter.y + Random.Range(-spawnRangeSize.y / 2, spawnRangeSize.y / 2)) * Vector3.up;
            GameObject monster = Instantiate(monsters[mIdx], spawnPosition, Quaternion.identity);
            
            // 랜덤 방향으로 튀어오르기
            Rigidbody2D rb = monster.GetComponent<Rigidbody2D>();
            if(rb != null) {
                float xForce = Random.Range(-5.0f, 5.0f);
                float yForce = Random.Range(3.0f, 5.0f);
                rb.AddForce(xForce * Vector2.right + yForce * Vector2.up, ForceMode2D.Impulse);
                
                WalkingMonster walkingMonster = monster.GetComponent<WalkingMonster>();
                if(walkingMonster != null && xForce >= 0 != walkingMonster.isFacingRight) {
                    walkingMonster.Flip();
                }
            }
        }
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireCube(
            spawnRangeCenter,
            spawnRangeSize / 2
        );
    }
}
