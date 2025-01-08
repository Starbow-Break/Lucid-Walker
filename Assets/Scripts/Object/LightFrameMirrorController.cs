using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LightFrameMirrorController : MonoBehaviour
{
    [SerializeField, Min(1)] int period; // 주기
    [SerializeField, Min(0.01f)] float timePerUnit; // 단위당 시간
    [SerializeField] List<int> lightOnTime; // 불이 켜지는 시간

    LightFrameMirror mirror;
    List<int> _lightOnTime;
    int lightOnTimeIndex = 0;
    int curTimeUnit; // 현재 시간

    void Awake() {
        mirror = GetComponent<LightFrameMirror>();
        if(mirror == null) { 
            throw new System.Exception("컨트롤 할 수 있는 LightFrameMirror가 없습니다.");
        }
    }

    // Start is called before the first frame update
    void Start() {
        StartController();
    }

    void StartController() {
        curTimeUnit = period - 1;
        _lightOnTime = lightOnTime.Distinct().ToList().Where(t => t >= 0).ToList();
        _lightOnTime.Sort();
        StartCoroutine(MirrorControlFlow());
    }

    IEnumerator MirrorControlFlow()
    {
        while(true) {
            curTimeUnit = (curTimeUnit + 1) % period;
            if(curTimeUnit == 0) lightOnTimeIndex = 0;

            if(lightOnTimeIndex < _lightOnTime.Count() && _lightOnTime[lightOnTimeIndex] == curTimeUnit) {
                mirror.SetLight(true);
            }
            else {
                mirror.SetLight(false);
            }

            yield return new WaitForSeconds(timePerUnit);
        }
    }
}
