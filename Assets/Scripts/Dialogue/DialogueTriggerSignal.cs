using UnityEngine;

public class DialogueTriggerSignal : MonoBehaviour
{
    public string characterName;
    public Sprite characterSprite;
    [TextArea]
    public string dialogueLine;

    public void TriggerDialogue()
    {
        Debug.Log("⚡ TriggerDialogue() called!");

        if (DialogueUI.Instance == null)
        {
            Debug.LogError("❌ DialogueUI.Instance is NULL!");
            return;
        }

        DialogueUI.Instance.UpdateDialogueText(characterName, characterSprite);
        DialogueUI.Instance.SetDialogueText(dialogueLine);
    }

}
