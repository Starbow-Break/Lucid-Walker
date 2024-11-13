using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogueData", menuName = "Dialogue/DialogueData")]
public class DialogueData : ScriptableObject
{
    public string dialogueID;
    public List<DialogueLine> lines; // 주고받는 대사를 포함한 대화 목록
}
