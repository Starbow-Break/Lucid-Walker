using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BackgroundStarGenerator : MonoBehaviour
{
    [SerializeField] Vector2 min = new(-1, -1); // 왼쪽 아래
    [SerializeField] Vector2 max = new(1, 1); // 오른쪽 위
    [SerializeField, Min(0)] int generateCount = 1; // 생성 갯수
    [SerializeField] List<Item> items; // 생성할 오브젝트들

    void Awake() {
        // min, max가 유효한 값이 아니면 예외 발생
        if(min.x > max.x || min.y > max.y) {
            throw new RangeException("min, max가 유효한 값이 아닙니다.");
        }
    }

    void Start() {
        GenerateStars();
    }

    // 지정죈 범위에 별 랜덤 생성
    void GenerateStars() {
        List<float> totalWeight = new() { 0.0f };

        foreach(Item item in items) {
            totalWeight.Add(totalWeight[^1] + item.weight);
        }

        if(items.Count > 0) {
            for(int cnt = 0; cnt < generateCount; cnt++) {
                float x = Random.Range(min.x, max.x);
                float y = Random.Range(min.y, max.y);
                Vector2 spawnPosition = new(x, y);

                int itemIndex = 0;
                float randomValue = Random.Range(0.0f, totalWeight[^1]);
                for(;; itemIndex++) {
                    if(randomValue <= totalWeight[itemIndex+1]) {
                        break;
                    }
                }

                Instantiate(items[itemIndex].obj, spawnPosition, Quaternion.identity, transform);
            }
        }
    }

    #region GIZMOS
    private void OnDrawGizmos() {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube((max + min) / 2, max - min);
    }
    #endregion

    #region ITEM
    [System.Serializable]
    class Item {
        [SerializeField] 
        public GameObject obj; // 오브젝트
        [SerializeField, Min(0.0f)] 
        public float weight; // 확률 가중치
    }
    #endregion

    #region EXCEPTION
    // 범위 값 잘못 설정 시 (min, max) 던지는 예외
    class RangeException: System.Exception {
        public RangeException(string message) : base(message) {}
    }
    #endregion
}
