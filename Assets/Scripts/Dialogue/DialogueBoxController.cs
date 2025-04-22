using UnityEngine;

public class DialogueBoxController : MonoBehaviour
{
  public void HideDialogue()
  {
    if (DialogueUI.Instance != null)
    {
      DialogueUI.Instance.HideDialogueBox();
    }
  }
}
