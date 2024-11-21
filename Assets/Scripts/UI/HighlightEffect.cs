using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightEffect : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    [SerializeField] private float highlightSpeed = 2f; // 반짝임 속도
    private bool isHighlighted = true;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (isHighlighted)
        {
            float alpha = Mathf.PingPong(Time.time * highlightSpeed, 0.3f) + 0.7f;
            spriteRenderer.color = new Color(1f, 1f, 1f, alpha);
        }
    }

    public void StopHighlight()
    {
        isHighlighted = false;
        spriteRenderer.color = Color.white; // 원래 색상으로 복구
    }
}
