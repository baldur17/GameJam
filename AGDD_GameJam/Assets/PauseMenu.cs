using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public static bool GameIsFinished = false;
    

    [FormerlySerializedAs("GameMenuUi")] public GameObject gameMenuUi;
    [FormerlySerializedAs("EndGameMenuUi")] public GameObject endGameMenuUi;

    // Update is called once per frame
    void Update()
    {

        if (GameIsFinished)
        {
            return;
        }
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
        GameIsFinished = false;
        gameMenuUi.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        GameIsFinished = false;
        gameMenuUi.SetActive(false);
        Time.timeScale = 1f;
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        GameIsFinished = false;
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void FinishGame()
    {
        GameIsFinished = true;
        endGameMenuUi.SetActive(true);
        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        GameIsFinished = false;
        Time.timeScale = 1f;
        GameManager.instance.lastCheckpoint = new Vector3(0, 0, 0);
        GameManager.instance.RestartLevel();
    }
    
    
    
}
