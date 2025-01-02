using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class BackgroundStarGenerator : MonoBehaviour
{
    [Header("Range")]
    [SerializeField] Vector2 min = new(-1, -1); // 왼쪽 아래
    [SerializeField] Vector2 max = new(1, 1); // 오른쪽 위

    [Header("Star Generator")]
    [SerializeField] bool fixedRandomSeed = true; // true면 특정 시드로 고정한다.
    [SerializeField] int seed = 123456789; // 랜덤 시드, fixedRandomSeed가 true면 해당 값을 랜덤 시드로 사용
    [SerializeField, Min(0.1f)] float radius = 1.0f; // 별 사이의 최소 간격
    [SerializeField] int tryCount = 10; // 별 스폰 위치 생성할 때 시도 횟수
    [SerializeField, Range(0.0f, 100.0f)] float generateProbability = 100.0f; // 별 생성 확률 (Percent)
    [SerializeField] List<Item> items; // 생성할 오브젝트들

    InstantiableRandom random;
    PoissonDiskSampling pds;

    void Awake() {
        // min, max가 유효한 값이 아니면 예외 발생
        if(min.x > max.x || min.y > max.y) {
            throw new InvalidRangeException("min, max가 유효한 값이 아닙니다.");
        }

        random = fixedRandomSeed ? new InstantiableRandom(seed) : new InstantiableRandom();
        pds = new PoissonDiskSampling(random);
    }

    void Start() {
        GenerateStars();
    }

    // 지정죈 범위에 별 랜덤 생성
    void GenerateStars() {
        // 별을 생성시킬 위치
        List<Vector2> starPositions = pds.GeneratePoints(
            radius,
            new Vector2(max.x - min.x, max.y - min.y),
            tryCount
        );
        for(int i = 0; i < starPositions.Count; i++) {
            starPositions[i] += min;
        }

        List<float> totalWeight = new() { 0.0f };
        foreach(Item item in items) {
            totalWeight.Add(totalWeight[^1] + item.weight);
        }

        // 각 위치마다 별 생성
        foreach(Vector2 starPosition in starPositions) {
            float generateValue = random.Range(0.0f, 100.0f);
            if(generateValue > generateProbability) continue;

            int itemIndex = 0;
            float starValue = random.Range(0.0f, totalWeight[^1]);
            for(;; itemIndex++) {
                if(starValue <= totalWeight[itemIndex+1]) {
                    break;
                }
            }

            Instantiate(items[itemIndex].obj, starPosition, Quaternion.identity, transform);
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
    class InvalidRangeException: System.Exception {
        public InvalidRangeException(string message) : base(message) {}
    }
    #endregion
}
