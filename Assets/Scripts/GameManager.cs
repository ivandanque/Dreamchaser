using System;
using System.Threading;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private GameObject[] enemies;

    public static event Action<int> OnCountUpdate;

    private void Awake()
    {
        CreateSingleton();
    }

    private void Start()
    {
        UpdateEnemyCount();
    }

    private void UpdateEnemyCount()
    {
        StartCoroutine(GetEnemyCount());
    }

    IEnumerator GetEnemyCount()
    {
        yield return new WaitForSeconds(0.25f);
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        OnCountUpdate?.Invoke(enemies.Length);
    }

    void CreateSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Game Closed");
            Application.Quit();
        }
    }

    private void OnEnable()
    {
        EnemyUnit.OnEnemyDeath += UpdateEnemyCount;
    }

    private void OnDisable()
    {
        EnemyUnit.OnEnemyDeath -= UpdateEnemyCount;
    }
}