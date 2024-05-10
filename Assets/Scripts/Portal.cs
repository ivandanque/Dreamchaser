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
        if (other.gameObject.CompareTag("Player")) OnLevelComplete?.Invoke(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
