using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Main Buttons")]
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button continueGameButton;
    // Start is called before the first frame update
    private void Start()
    {
        // MusicManger.Instance.PlayMusic("Main Menu");
        if (!DataPersistenceManager.instance.HasGameData())
        {
            continueGameButton.interactable = false;
        }
    }

    // Update is called once per frame
    private void Play()
    {
        LevelManager.Instance.LoadScene("Stage5", "CrossFade");
        // MusicManager.Instance.PlayMusic("Adventure");
    }

    public void Settings()
    {

    }
    public void Quit()
    {
        Application.Quit();
    }
    public void OnNewGameClicked()
    {
        DisableMenuButtons();
        DataPersistenceManager.instance.NewGame();
        DataPersistenceManager.instance.SaveGame();

        SceneManager.LoadSceneAsync("NewsTutorial");
    }
    public void OnContinueGameClicked()
    {
        DisableMenuButtons();
        Debug.Log("게임 계속하기 클릭");
        SceneManager.LoadSceneAsync("Main");
    }

    private void DisableMenuButtons()
    {
        newGameButton.interactable = false;
        continueGameButton.interactable = false;
    }
}
