using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAttackHandler : MonoBehaviour
{
    private GameObject assignedProjectile;
    private Collider assignedCollider;

    private PlayerUnit pu;

    private bool hasCollidedWithPlayer = false;

    private EnemyUnit eu;
    private AttackHit attackHit;
    private GameObject assignedObject;

    public static event Action<float> OnPlayerHit;

    private void InitializeAttack(EnemyUnit eu, AttackHit attackHit)
    {
        this.eu = eu;
        this.attackHit = attackHit;
    }

    private void InitializeHitbox()
    {
        assignedCollider = attackHit.assignedObject.GetComponent<Collider>();
        assignedCollider.enabled = false;
        Invoke(nameof(ActivateHitbox), attackHit.startTime);
    }

    private void ProjectileStartup()
    {
        Invoke(nameof(FireProjectile), attackHit.startTime);
    }

    private void ActivateHitbox()
    {
        Collider[] col = Physics.OverlapBox(assignedCollider.bounds.center, assignedCollider.bounds.extents, assignedCollider.transform.rotation, eu.playerLayer);
        foreach (Collider cold in col)
        {
            pu = cold.GetComponent<PlayerUnit>();
            if (pu != null) pu.TakeDamage(CalculateDamage());
        }
        Invoke(nameof(DeactivateHitbox), attackHit.activeTime);
    }

    private void DeactivateHitbox()
    {
        Invoke(nameof(EndAttack), attackHit.endTime);
    }

    private void FireProjectile()
    {
        assignedProjectile = Instantiate(assignedObject, eu.transform.position, Quaternion.identity);
        assignedProjectile.GetComponent<ProjectileContainer>().SetAttack(CalculateDamage(), attackHit.activeTime);
        assignedProjectile.GetComponent<Rigidbody>().AddForce(eu.transform.forward * attackHit.horizontalSpeed + eu.transform.up * attackHit.verticalSpeed, ForceMode.Impulse);
        Invoke(nameof(EndAttack), attackHit.endTime);
    }

    private void EndAttack()
    {
        eu.isAttacking = false;
    }

    private float CalculateDamage()
    {
        float baseDamage = eu.attack * attackHit.attackScaling;
        if (UnityEngine.Random.value <= eu.critChance) return baseDamage * eu.critMultiplier;
        return baseDamage;
    }

    private void OnEnable()
    {
        EnemyUnit.OnHit += InitializeAttack;
        EnemyUnit.OnHitboxHit += InitializeHitbox;
        EnemyUnit.OnProjectileHit += ProjectileStartup;
    }

    private void OnDisable()
    {
        EnemyUnit.OnHit -= InitializeAttack;
        EnemyUnit.OnHitboxHit -= InitializeHitbox;
        EnemyUnit.OnProjectileHit -= ProjectileStartup;
    }
}
