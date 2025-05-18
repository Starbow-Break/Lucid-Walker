using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.WSA;

public class KeyGuide : MonoBehaviour
{
    [System.Serializable]
    struct KeyCodeSpritePair
    {
        public KeyCode key;
        public Sprite sprite;
    }
    // 계속 활성화하고 싶은 경우 
    [SerializeField] private bool startActive = false;
    [SerializeField] List<KeyCodeSpritePair> keySprites;
    [SerializeField] Image image;
    [SerializeField] TextMeshProUGUI scriptTMP;

    Dictionary<KeyCode, Sprite> keyDist;

    void Awake()
    {
        keyDist = new Dictionary<KeyCode, Sprite>();
    }

    void Start()
    {
        foreach (KeyCodeSpritePair pair in keySprites)
        {
            keyDist.Add(pair.key, pair.sprite);
        }

        // InActive();
        if (startActive)
            Active();
        else
            InActive();
    }

    // UI 활성화
    public void Active()
    {
        gameObject.SetActive(true);
    }

    // KeyCode에 맞는 이미지와 인자로 들어온 텍스트로 변경 후 UI 활성화 
    public void Active(KeyCode keyCode, String script)
    {
        if (keyDist[keyCode] != null)
        {
            image.sprite = keyDist[keyCode];
        }
        scriptTMP.text = script;
        Active();
    }

    public void Active(KeyCode keyCode, String script, Vector2 uiSize)
    {
        RectTransform rt = GetComponent<RectTransform>();
        Vector2 oldSize = rt.rect.size;
        Vector2 deltaSize = uiSize - oldSize;
        rt.offsetMin -= new Vector2(deltaSize.x * rt.pivot.x, deltaSize.y * rt.pivot.y);
        rt.offsetMax += new Vector2(deltaSize.x * (1f - rt.pivot.x), deltaSize.y * (1f - rt.pivot.y));
        Active(keyCode, script);
    }

    // UI 비활성화
    public void InActive()
    {
        gameObject.SetActive(false);
    }
}
