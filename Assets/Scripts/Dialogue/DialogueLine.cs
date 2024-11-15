using UnityEngine;
using System.Collections.Generic;


[System.Serializable]
public class DialogueLine
{
    public string characterName; // 대화하는 캐릭터 이름
    public Sprite characterImage; // 캐릭터 이미지
    [TextArea(3, 10)]
    public string sentence; // 대사 내용
}
