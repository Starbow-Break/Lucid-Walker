using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskBossLampGroup : MonoBehaviour
{
    [SerializeField] List<MaskBossLamp> lamps;
    [SerializeField] float moveDistance = 0.0f;

    Vector3 initPosition;
    Vector3 downPosition;

    void Start() {
        initPosition = transform.position;
        downPosition = initPosition + moveDistance * Vector3.down;
    }

    public int LampCount => lamps.Count;

    public IEnumerator MoveDown() {
        yield return Move(initPosition, downPosition, 1.5f);
    }

    public IEnumerator MoveUp() {
        yield return Move(downPosition, initPosition, 1.5f);
    }

    // 모든 조명 끄기
    public void TurnOffAllLamps() {
        foreach(MaskBossLamp lamp in lamps) {
            lamp.TurnOff();
        }
    }

    // i번째 조명 노란색 불빛 켜기
    public void TurnOnYellowLight(int index) {
        lamps[index].TurnOnYellowLight();
    }

    // i번째 조명 붉은색 불빛 켜기
    public void TurnOnRedLight(int index) {
        lamps[index].TurnOnRedLight();
    }

    IEnumerator Move(Vector3 start, Vector3 finish, float time) {
        float currentTime = 0.0f;
        while(currentTime < time) {
            yield return null;
            currentTime += Time.deltaTime;
            transform.position = Vector3.Lerp(start, finish, Mathf.Clamp01(currentTime / time));
        }
    }
}
