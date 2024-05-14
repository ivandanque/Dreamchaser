using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LoadoutPanel : MonoBehaviour
{
    public static event Action OnLoadoutActive;
    private void OnEnable()
    {
        OnLoadoutActive?.Invoke();
    }
}
