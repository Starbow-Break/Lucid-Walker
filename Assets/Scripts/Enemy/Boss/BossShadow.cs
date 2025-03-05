using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class BossShadow : MonoBehaviour
{
    [SerializeField] Transform startingPoint;
    [SerializeField] Transform finishPoint;

    void Start()
    {
        if(startingPoint == null || finishPoint == null) {
            Debug.LogError("출발 지점 또는 도착 지점이 없습니다.");
            return;
        }

        transform.position = startingPoint.position;
    }

    public void Move(float time) {
        if(time < 0) {
            Debug.LogError("BossShado.Move()의 인자 값이 양수여야 합니다!");
            return;
        }

        StartCoroutine(MoveFlow(time));
    }

    IEnumerator MoveFlow(float time) {
        float currentTime = 0.0f;
        
        while(currentTime < time) {
            currentTime += Time.deltaTime;
            transform.position = Vector3.Lerp(startingPoint.position, finishPoint.position, currentTime / time);
            yield return null;
        }
    }
}
