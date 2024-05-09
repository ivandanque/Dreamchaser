using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetHandler : MonoBehaviour
{
    public Camera thirdPersonCamera;
    public Transform cameraOrientation;
    public float maxTargetingAngle;
    public float targetingRange;
    public LayerMask enemyLayer;
    public GameObject targetCursor;

    private PlayerUnit pu;
    private GameObject enemy;
    private List<GameObject> validTargets = new();
    private GameObject closestTarget;
    private Transform cameraFlat;

    public static event Action<GameObject> OnTargetAcquired;

    private void Start()
    {
        pu = GetComponent<PlayerUnit>();
        targetCursor.SetActive(false);
    }

    private void Update()
    {
        validTargets.Clear();
        Collider[] cols = Physics.OverlapSphere(transform.position, targetingRange, enemyLayer);
        foreach (Collider col in cols)
        {
            enemy = col.gameObject;
            if (IsEnemyInView(enemy.transform)) validTargets.Add(enemy);
        }

        if (validTargets.Count > 0)
        {
            FindClosestTarget();
            if (closestTarget != null)
            {
                OnTargetAcquired?.Invoke(closestTarget);
                ShowTargetCursor();
            }
            
        }
        else HideTargetCursor();
    }

    private bool IsEnemyInView(Transform enemy)
    {
        cameraFlat = cameraOrientation;
        cameraFlat.position = new Vector3(cameraOrientation.position.x, transform.position.y, cameraOrientation.position.z);
        if (Vector3.Angle(enemy.position - cameraFlat.position, cameraFlat.forward) < maxTargetingAngle) return true;
        else return false;
    }

    private void FindClosestTarget()
    {
        float closestDistance = 0;
        closestTarget = null;
        foreach (GameObject target in validTargets)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, target.transform.position - transform.position, out hit, Mathf.Infinity, enemyLayer))
            {
                if (closestTarget == null)
                {
                    closestDistance = hit.distance;
                    closestTarget = target;
                }
                else
                {
                    if (hit.distance < closestDistance)
                    {
                        closestDistance = hit.distance;
                        closestTarget = target;
                    }
                }
            }    
        }
    }

    private void ShowTargetCursor()
    {
        targetCursor.SetActive(true);
        Vector3 viewportRawPosition = thirdPersonCamera.WorldToViewportPoint(closestTarget.transform.position);
        Vector3 viewportActualPosition = new Vector3(viewportRawPosition.x * 1920, viewportRawPosition.y * 1080 - 1080, 0f);
        targetCursor.GetComponent<RectTransform>().anchoredPosition = viewportActualPosition;
    }

    private void HideTargetCursor()
    {
        targetCursor.SetActive(false);
    }
}
