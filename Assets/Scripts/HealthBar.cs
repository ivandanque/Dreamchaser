using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public TextMeshProUGUI healthText;
    
    public void SetMaxHealth(float health)
    {
        slider.maxValue = health;
        SetHealth(health);
    }

    public void SetHealth(float health)
    {
        slider.value = health;
        healthText.text = string.Format("{0} / {1}", slider.value, slider.maxValue);
    }
}
