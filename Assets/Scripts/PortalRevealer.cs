using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalRevealer : MonoBehaviour
{
    public GameObject portal;

    private void RevealPortal()
    {
        portal.SetActive(true);
    }

    private void OnEnable()
    {
        EnemyCount.OnEnemyWipe += RevealPortal;
    }

    private void OnDisable()
    {
        EnemyCount.OnEnemyWipe -= RevealPortal;
    }
}
