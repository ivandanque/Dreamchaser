using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    public static event Action<int> OnLevelComplete;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            OnLevelComplete?.Invoke(SceneManager.GetActiveScene().buildIndex + 1);

            switch (SceneManager.GetActiveScene().name)
            {
                case "Level1":
                    SceneManager.LoadScene("Level2");
                    break;
                case "Level2":
                    SceneManager.LoadScene("Level3");
                    break;
                case "Level3":
                    SceneManager.LoadScene("MainMenu");
                    break;
                default:
                    SceneManager.LoadScene("MainMenu");
                    break;
            }
        }
    }
}
