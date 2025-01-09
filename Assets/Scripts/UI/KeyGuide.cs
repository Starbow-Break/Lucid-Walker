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
    struct KeyCodeSpritePair {
        public KeyCode key;
        public Sprite sprite;
    }

    [SerializeField] List<KeyCodeSpritePair> keySprites;
    [SerializeField] Image image;
    [SerializeField] TextMeshProUGUI scriptTMP;

    Dictionary<KeyCode, Sprite> keyDist;

    void Awake() {
        keyDist = new Dictionary<KeyCode, Sprite>();
    }

    void Start() {
        foreach(KeyCodeSpritePair pair in keySprites) {
            keyDist.Add(pair.key, pair.sprite);
        }

        InActive();
    }

    // UI 활성화
    public void Active() {
        gameObject.SetActive(true);
    }

    // KeyCode에 맞는 이미지와 인자로 들어온 텍스트로 변경 후 UI 활성화 
    public void Active(KeyCode keyCode, String script) {
        if(keyDist[keyCode] != null) {
            image.sprite = keyDist[keyCode];
        }
        scriptTMP.text = script;
        gameObject.SetActive(true);
    }

    // UI 비활성화
    public void InActive() {
        gameObject.SetActive(false);
    }
}
