using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalRevealer : MonoBehaviour
{
    public GameObject portal;
    [SerializeField] private AudioClip audioClip;

    private void RevealPortal()
    {
        portal.SetActive(true);
        AudioManager.Instance.PlaySound(audioClip);
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
