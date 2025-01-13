using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        // MusicManger.Instance.PlayMusic("Main Menu");
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
}
