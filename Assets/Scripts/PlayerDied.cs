using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDied : MonoBehaviour
{
    public GameObject loseScreen;

    private void OpenLoseScreen()
    {
        loseScreen.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0;
    }

    private void OnEnable()
    {
        PlayerUnit.OnPlayerDeath += OpenLoseScreen;
    }

    private void OnDisable()
    {
        PlayerUnit.OnPlayerDeath -= OpenLoseScreen;
    }
}
