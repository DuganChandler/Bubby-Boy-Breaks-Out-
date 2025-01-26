using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    void Start() {
        MusicManager.Instance.PlayMusic("BubbleBoyTheme");
    }
    
    public void PlayGame()
    {
        SceneManager.LoadScene("FirstLevel");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
