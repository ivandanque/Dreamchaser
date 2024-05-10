using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{

    public void StartGame()
    {
        SceneManager.LoadScene("TestingGround");
    }

    public void LoadLevel(int level)
    {
        switch (level)
        {
            case 1:
                SceneManager.LoadScene("Level1");
                break;
            case 2:
                SceneManager.LoadScene("Level2");
                break;
            case 3:
                SceneManager.LoadScene("Level3");
                break;
            default:
                SceneManager.LoadScene("MainMenu");
                break;
        }
    }

    public void ExitGame()
    {
        Debug.Log("Game Closed");
        Application.Quit();
    }

    private void OnEnable()
    {
        Portal.OnLevelComplete += LoadLevel;
    }

    private void OnDisable()
    {
        Portal.OnLevelComplete -= LoadLevel;
    }
}