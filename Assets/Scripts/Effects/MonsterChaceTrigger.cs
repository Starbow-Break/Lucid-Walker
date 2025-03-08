using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterChaseTrigger : MonoBehaviour
{
    public List<GameObject> Monsters = new List<GameObject>(); // 몬스터 리스트
    public List<GameObject> FallingObjects = new List<GameObject>(); // 무너지는 오브젝트 리스트

    public float moveSpeed = 2f; // 이동 속도 (오른쪽 이동)

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // 몬스터 활성화 및 이동 시작
            foreach (var monster in Monsters)
            {
                if (monster != null)
                {
                    monster.SetActive(true);
                }
            }

            // 무너지는 오브젝트 활성화 및 이동 시작
            foreach (var obj in FallingObjects)
            {
                if (obj != null)
                {
                    obj.SetActive(true);
                    StartCoroutine(MoveRight(obj));
                }
            }
        }
    }

    private IEnumerator MoveRight(GameObject obj)
    {
        while (obj.activeSelf)
        {
            obj.transform.position += Vector3.right * moveSpeed * Time.deltaTime;
            yield return null;
        }
    }

}
