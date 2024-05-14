using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    public static event Action OnPlayerTeleport;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            switch (SceneManager.GetActiveScene().name)
            {
                case "Level1":
                    SceneManager.LoadScene("Level2");
                    OnPlayerTeleport?.Invoke();
                    break;
                case "Level2":
                    SceneManager.LoadScene("Level3");
                    OnPlayerTeleport?.Invoke();
                    break;
                case "Level3":
                    SceneManager.LoadScene("MainMenuScene");
                    break;
                default:
                    SceneManager.LoadScene("MainMenuScene");
                    break;
            }
        }
    }
}
