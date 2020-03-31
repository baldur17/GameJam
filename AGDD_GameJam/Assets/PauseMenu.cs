using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;

    [FormerlySerializedAs("GameMenuUi")] public GameObject gameMenuUi;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Start"))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }

            GameIsPaused = !GameIsPaused;
        }

        if (Input.GetButtonDown("BButton") && GameIsPaused)
        {
            Resume();
            
            GameIsPaused = !GameIsPaused;
        }
    }

    public void Pause()
    {
        gameMenuUi.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        gameMenuUi.SetActive(false);
        Time.timeScale = 1f;
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    
    
    
}
