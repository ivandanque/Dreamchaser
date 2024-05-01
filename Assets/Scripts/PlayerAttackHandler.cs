using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAttackHandler : MonoBehaviour
{
    private GameObject assignedProjectile;
    private Collider assignedCollider;

    private PlayerUnit pu;
    private EnemyUnit eu;
    private LayerMask enemyLayer;
    public Transform orientation;

    private Weapon assignedWeapon;
    private GameObject assignedObject;

    private void Start()
    {
        pu = GetComponent<PlayerUnit>();
    }

    public void WeaponBasicAttack(Weapon weapon, GameObject go)
    {
        assignedWeapon = weapon;
        assignedObject = go;
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
        for (int i = 0; i < cols.Length; i++)
        {
            eu = cols[i].GetComponent<EnemyUnit>();
            eu.TakeDamage(Random.value <= pu.critChance ? assignedWeapon.baseAttack * pu.critMultiplier : assignedWeapon.baseAttack);
        }
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
        assignedProjectile.GetComponent<ProjectileContainer>().SetAttack(Random.value <= pu.critChance ? assignedWeapon.baseAttack * pu.critMultiplier : assignedWeapon.baseAttack, assignedWeapon.basicAttackTime);
        assignedProjectile.GetComponent<Rigidbody>().AddForce(orientation.forward * 1000, ForceMode.Impulse);
        pu.isAttacking = false;
    }
}
