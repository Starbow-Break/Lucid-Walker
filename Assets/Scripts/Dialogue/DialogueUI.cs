using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro 네임스페이스 추가

public class DialogueUI : MonoBehaviour
{
    public static DialogueUI Instance;
    public TMP_Text dialogueText; // TMP_Text로 변경
    public TMP_Text characterNameText; // TMP_Text로 변경
    public Image characterImage;
    public GameObject dialogueBox;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UpdateDialogueText(string characterName, Sprite image, string sentence)
    {
        characterNameText.text = characterName;
        characterImage.sprite = image;
        dialogueText.text = sentence;
        dialogueBox.SetActive(true);
    }

    public void HideDialogueBox()
    {
        dialogueBox.SetActive(false);
    }
}
