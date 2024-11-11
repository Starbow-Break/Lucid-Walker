using System;
using System.Collections;
using UnityEngine;

public class Floating : MonoBehaviour
{
    enum FloatingFunc {
        Zigzag, Sine, Cosine
    }

    [SerializeField] FloatingFunc func = FloatingFunc.Zigzag;
    [SerializeField, Min(0.0f)] float floatingAmount = 0.0f; // 떠다니는 정도
    [SerializeField, Min(0.0f)] float period = 2.0f; // 주기

    Vector3 initPos; // 초기 위치
    Func<float, float> amountFunc; // 가중치 함수
    Coroutine coroutine = null; // 현재 실행중인 코루틴
    bool isPlay = false;

    void OnValidate() {
        switch(func)
        {
            case FloatingFunc.Zigzag: { amountFunc = ZigzagFunc; break; }
            case FloatingFunc.Sine: { amountFunc = SineFunc; break; }
            case FloatingFunc.Cosine: { amountFunc = CosineFunc; break; }
        }
    }

    void Start()
    {
        initPos = transform.localPosition;
        Play();
    }

    // 떠다니는 효과 실행
    IEnumerator FloatingFlow() {
        float value = 0.0f;

        while(true) {
            if(isPlay)
            {
                value = (value + Time.deltaTime / period) % 1.0f;
                transform.localPosition = initPos + Vector3.up * amountFunc(value);
            }
            yield return null;
        }
    }

    //===============================================================
    // 떠다니는 모션 재생, 일시정지, 정지
    //===============================================================
    // 재생
    public void Play() {
        if(coroutine == null) {
            coroutine = StartCoroutine(FloatingFlow());
        }

        isPlay = true;
    }

    // 일시 정지
    public void Pause() {
        isPlay = false;
    }

    // 정지
    public void Stop() {
        if(coroutine != null) {
            StopCoroutine(coroutine);
            coroutine = null;
        }

        isPlay = false;
    }

    //===============================================================
    // 가중치 함수들 (주기함수, 정의역은 [0, 1])
    //===============================================================
    // 지그재그
    float ZigzagFunc(float value)
    {
        return floatingAmount * (2.0f * Mathf.Abs(value - 0.5f) - 0.5f);
    }

    // Sine
    float SineFunc(float value)
    {
        return floatingAmount * Mathf.Sin(value * 2.0f * Mathf.PI);
    }

    // Cosine
    float CosineFunc(float value)
    {
        return floatingAmount * Mathf.Cos(value * 2.0f * Mathf.PI);
    }
}
