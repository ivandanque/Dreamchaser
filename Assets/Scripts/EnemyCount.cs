using System;
using TMPro;
using UnityEngine;

public class EnemyCount : MonoBehaviour
{
    public TextMeshProUGUI countText;
    public static event Action OnEnemyWipe;

    private void UpdateCount(int count)
    {
        countText.text = "Enemies left: " + count;
        if (count <= 0) OnEnemyWipe?.Invoke();
    }

    private void OnEnable()
    {
        GameManager.OnCountUpdate += UpdateCount;
    }

    private void OnDisable()
    {
        GameManager.OnCountUpdate -= UpdateCount;
    }
}
