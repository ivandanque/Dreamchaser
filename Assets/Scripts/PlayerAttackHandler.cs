using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PlayerAttackHandler : MonoBehaviour
{
    private GameObject assignedProjectile;
    private Collider assignedCollider;

    private LayerMask enemyLayer;
    public Transform orientation;

    private PlayerUnit pu;
    private Weapon assignedWeapon;
    private GameObject assignedObject;

    public static event Action<float> OnEnemyHit;

    private void Start()
    {
        pu = GetComponent<PlayerUnit>();
    }

    private void InitializeAttack(PlayerUnit pu, Weapon weapon, GameObject go)
    {
        this.pu = pu;
        assignedWeapon = weapon;
        assignedObject = go;
    }

    public void WeaponBasicAttack()
    {
        pu.isAttacking = true;
        if (assignedWeapon.name.Equals("Hand") || assignedWeapon.name.Equals("Blade") || assignedWeapon.name.Equals("Gauntlet")) ActivateHitbox();
        if (assignedWeapon.name.Equals("Scepter") || assignedWeapon.name.Equals("Wand")) ProjectileStartup();
    }

    private void ActivateHitbox()
    {
        assignedCollider = assignedObject.GetComponent<Collider>();
        assignedCollider.enabled = false;
        Invoke(nameof(StaticHitboxAttack), assignedWeapon.basicAttackStartup);
    }

    private void StaticHitboxAttack()
    {
        assignedCollider.enabled = true;
        Collider[] cols = Physics.OverlapBox(assignedCollider.bounds.center, assignedCollider.bounds.extents, Quaternion.identity, enemyLayer);
        for (int i = 0; i < cols.Length; i++) OnEnemyHit?.Invoke(CalculateDamage());
        Invoke(nameof(DeactivateHitbox), assignedWeapon.basicAttackTime);
    }

    private void DeactivateHitbox()
    {
        assignedCollider.enabled = false;
        pu.isAttacking = false;
    }

    private void ProjectileStartup()
    {
        Invoke(nameof(FireProjectile), assignedWeapon.basicAttackStartup);
    }

    private void FireProjectile()
    {
        assignedProjectile = Instantiate(assignedObject, pu.transform.position, Quaternion.identity);
        assignedProjectile.GetComponent<ProjectileContainer>().SetAttack(CalculateDamage(), assignedWeapon.basicAttackTime);
        assignedProjectile.GetComponent<Rigidbody>().AddForce(orientation.forward * 1000, ForceMode.Impulse);
        pu.isAttacking = false;
    }

    private float CalculateDamage()
    {
        if (UnityEngine.Random.value <= pu.critChance) return assignedWeapon.baseAttack * pu.critMultiplier;
        return assignedWeapon.baseAttack;
    }


}
