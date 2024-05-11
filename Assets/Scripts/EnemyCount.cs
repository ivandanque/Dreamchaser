using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class EnemyCount : MonoBehaviour
{
    private GameObject[] enemies;

    public TextMeshProUGUI countText;
    public static event Action OnEnemyWipe;

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
        UpdateCount(enemies.Length);
    }

    private void UpdateCount(int count)
    {
        countText.text = "Enemies left: " + count;
        if (count <= 0) OnEnemyWipe?.Invoke();
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
