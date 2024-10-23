using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitForCoroutines : CustomYieldInstruction
{
    // 대기 여부
    // 아직 종료 안된 코루틴이 남아있다면 계속 대기
    public override bool keepWaiting => _remainCount > 0;

    // 남은 코루틴 수
    private int _remainCount;

    public WaitForCoroutines(MonoBehaviour runner, params IEnumerator[] coroutines)
    {
        // 코루틴이 종료될 때까지 대기
        IEnumerator WaitingCoroutine(IEnumerator coroutine)
        {
            yield return coroutine;
            _remainCount--;
        }

        // 인자로 들어온 모든 코루틴 실행
        _remainCount = coroutines.Length;
        for (var i = 0; i < _remainCount; i++)
        {
            runner.StartCoroutine(WaitingCoroutine(coroutines[i]));
        }
    }
}
