using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskPiece : MonoBehaviour
{
    [SerializeField, Min(0f)] private float _lifeTime = 2f;
    [SerializeField, Min(1f)] private float _fadeDuration = 1f;

    private SpriteRenderer _sr;

    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
    }

    private IEnumerator Start()
    {
        var lifeTimeWait = new WaitForSeconds(_lifeTime);
        yield return lifeTimeWait;
        yield return FadeSequence();
    }

    private IEnumerator FadeSequence()
    {
        float currentTime = 0f;
        while (currentTime < _fadeDuration)
        {
            currentTime += Time.deltaTime;
            float alpha = Mathf.Clamp01((_fadeDuration - currentTime) / _fadeDuration);
            _sr.color = new Color(_sr.color.r, _sr.color.g, _sr.color.b, alpha);
            yield return null;
        }

        Destroy(gameObject);
    }
}
