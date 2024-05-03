using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PlayerAttackHandler : MonoBehaviour
{
    private GameObject assignedProjectile;
    public Transform modelTransform;
    public BoxCollider hitboxCollider;
    public LayerMask enemyLayer;
    public float playerCapsuleRadius;

    private Weapon currentWeapon;
    private Spell currentSpell;
    private GameObject assignedObject;
    private GameObject targetedEnemy;

    public static event Action OnPlayerAttackStart;
    public static event Action OnPlayerAttackEnd;
    public static event Action<float> OnEnemyHit;

    private void Start()
    {
        hitboxCollider.enabled = false;
    }

    private void InitializeWeapon(Weapon weapon)
    {
        currentWeapon = weapon;
        CheckWeaponType();
    }

    private void StartSpell(Weapon weapon, Spell spell)
    {
        currentWeapon = weapon;
        currentSpell = spell;
        CheckSpellType();
    }

    private void CheckWeaponType()
    {
        modelTransform.LookAt(new Vector3(targetedEnemy.transform.position.x, transform.position.y, targetedEnemy.transform.position.z));
        OnPlayerAttackStart?.Invoke();
        if (currentWeapon.attackHitType == AttackHitType.Hitbox) ActivateWeaponHitbox();
        if (currentWeapon.attackHitType == AttackHitType.Projectile) ProjectileAttack();
    }

    private void ActivateWeaponHitbox()
    {
        hitboxCollider.size = new Vector3(currentWeapon.basicAttackWidth, currentWeapon.basicAttackHeight, currentWeapon.basicAttackLength);
        hitboxCollider.transform.rotation = modelTransform.rotation;
        hitboxCollider.center = new Vector3(0f, 0f, currentWeapon.basicAttackLength / 2 + playerCapsuleRadius);
        hitboxCollider.enabled = true;

        Collider[] cols = Physics.OverlapBox(hitboxCollider.bounds.center, hitboxCollider.bounds.extents, Quaternion.identity, enemyLayer);
        for (int i = 0; i < cols.Length; i++) cols[i].GetComponent<EnemyUnit>().TakeDamage(BasicAttackDamage());

        StartCoroutine(DeactivateWeaponHitbox());
    }

    IEnumerator DeactivateWeaponHitbox()
    {
        yield return new WaitForSeconds(currentWeapon.basicAttackTime);
        hitboxCollider.enabled = false;
        OnPlayerAttackEnd?.Invoke();
    }

    private void CheckSpellType()
    {
        modelTransform.LookAt(new Vector3(targetedEnemy.transform.position.x, transform.position.y, targetedEnemy.transform.position.z));
        OnPlayerAttackStart?.Invoke();
        if (currentWeapon.attackHitType == AttackHitType.Hitbox) ActivateSpellHitbox();
        if (currentWeapon.attackHitType == AttackHitType.Projectile) ProjectileAttack();
    }

    private void ActivateSpellHitbox()
    {
        hitboxCollider.size = new Vector3(currentSpell.spellWidth, currentSpell.spellHeight, currentSpell.spellLength);
        hitboxCollider.transform.rotation = modelTransform.rotation;
        hitboxCollider.center = new Vector3(0f, 0f, currentWeapon.basicAttackLength / 2 + playerCapsuleRadius);
        hitboxCollider.enabled = true;

        Collider[] cols = Physics.OverlapBox(hitboxCollider.bounds.center, hitboxCollider.bounds.extents, Quaternion.identity, enemyLayer);
        for (int i = 0; i < cols.Length; i++) cols[i].GetComponent<EnemyUnit>().TakeDamage(BasicAttackDamage());

        StartCoroutine(DeactivateSpellHitbox());
    }

    IEnumerator DeactivateSpellHitbox()
    {
        yield return new WaitForSeconds(currentSpell.spellTime);
        hitboxCollider.enabled = false;
        OnPlayerAttackEnd?.Invoke();
    }

    private void ProjectileAttack()
    {

    }

    private float BasicAttackDamage()
    {
        return currentWeapon.baseAttack;
    }

    private float SpellDamage()
    {
        return currentWeapon.baseAttack * currentSpell.attackScaling;
    }

    private void AcquireTarget(GameObject go)
    {
        targetedEnemy = go;
    }

    private void OnEnable()
    {
        Loadout.OnBasicAttackStart += InitializeWeapon;
        Loadout.OnSpellStart += StartSpell;
        TargetHandler.OnTargetAcquired += AcquireTarget;
    }

    private void OnDisable()
    {
        Loadout.OnBasicAttackStart -= InitializeWeapon;
        Loadout.OnSpellStart -= StartSpell;
        TargetHandler.OnTargetAcquired -= AcquireTarget;
    }
}
