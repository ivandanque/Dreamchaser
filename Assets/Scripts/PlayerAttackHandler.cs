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
    public Transform projectileSpawnPoint;
    private ParticleSystem ps;
    private ParticleSystem.MainModule psMain;
    private ParticleSystem psChild;
    private ParticleSystem.ShapeModule psChildShape;
    private Weapon currentWeapon;
    private Spell currentSpell;
    private GameObject assignedObject;
    private GameObject targetedEnemy;
    private bool isAttacking = false;

    public static event Action OnPlayerAttackStart;
    public static event Action OnPlayerAttackEnd;
    public static event Action<float> OnEnemyHit;

    private void Start()
    {
        hitboxCollider.enabled = false;
    }

    private void InitializeWeapon(Weapon weapon)
    {
        if (isAttacking) return;
        if (targetedEnemy == null) return;
        currentWeapon = weapon;
        isAttacking = true;
        CheckWeaponType();
    }

    private void StartSpell(Weapon weapon, Spell spell)
    { 
        if (isAttacking) return;
        if (targetedEnemy == null) return;
        currentWeapon = weapon;
        currentSpell = spell;
        CheckSpellType();
    }

    private void CheckWeaponType()
    {
        modelTransform.LookAt(new Vector3(targetedEnemy.transform.position.x, transform.position.y, targetedEnemy.transform.position.z));
        OnPlayerAttackStart?.Invoke();
        if (currentWeapon.attackHitType == AttackHitType.Hitbox) ActivateWeaponHitbox();
        if (currentWeapon.attackHitType == AttackHitType.Projectile) FireWeaponProjectile();
        if (currentWeapon.attackHitType == AttackHitType.Hitscan) AttackHitscan();
    }
    private void CheckSpellType()
    {
        modelTransform.LookAt(new Vector3(targetedEnemy.transform.position.x, transform.position.y, targetedEnemy.transform.position.z));
        OnPlayerAttackStart?.Invoke();
        if (currentSpell.spellType == AttackHitType.Hitbox) ActivateSpellHitbox();
        if (currentSpell.spellType == AttackHitType.Projectile) FireWeaponProjectile();
        if (currentSpell.spellType == AttackHitType.Hitscan) SpellHitscan();
        if (currentSpell.spellType == AttackHitType.TargetArea) SpellArea();
    }

    private void ActivateWeaponHitbox()
    {
        hitboxCollider.size = new Vector3(currentWeapon.basicAttackWidth, currentWeapon.basicAttackHeight, currentWeapon.basicAttackLength);
        hitboxCollider.transform.rotation = modelTransform.rotation;
        hitboxCollider.center = new Vector3(0f, 0f, currentWeapon.basicAttackLength / 2 + playerCapsuleRadius);
        hitboxCollider.enabled = true;

        Collider[] cols = Physics.OverlapBox(hitboxCollider.bounds.center, hitboxCollider.bounds.extents, Quaternion.identity, enemyLayer);
        if (cols.Length > 0)
        {
            if (ps != null)
            {
                ps = Instantiate(currentWeapon.vfxPrefab).GetComponent<ParticleSystem>();
                ps.transform.position = transform.position;
                ps.transform.rotation = modelTransform.rotation;
                ps.Play();
            }
            for (int i = 0; i < cols.Length; i++) cols[i].GetComponent<EnemyUnit>().TakeDamage(BasicAttackDamage());
        }

        StartCoroutine(DeactivateWeaponHitbox());
    }

    IEnumerator DeactivateWeaponHitbox()
    {
        yield return new WaitForSeconds(currentWeapon.basicAttackTime);
        hitboxCollider.enabled = false;
        isAttacking = false;
        OnPlayerAttackEnd?.Invoke();
    }

    private void ActivateSpellHitbox()
    {
        hitboxCollider.size = new Vector3(currentSpell.spellWidth, currentSpell.spellHeight, currentSpell.spellLength);
        hitboxCollider.transform.rotation = modelTransform.rotation;
        hitboxCollider.center = new Vector3(0f, 0f, currentSpell.spellLength / 2 + playerCapsuleRadius);
        hitboxCollider.enabled = true;

        Collider[] cols = Physics.OverlapBox(hitboxCollider.bounds.center, hitboxCollider.bounds.extents, Quaternion.identity, enemyLayer);
        if (cols.Length > 0)
        {
            ps = Instantiate(currentSpell.vfxPrefab).GetComponent<ParticleSystem>();
            ps.transform.position = transform.position;
            ps.transform.rotation = modelTransform.rotation;
            if (ps != null) ps.Play();
            for (int i = 0; i < cols.Length; i++) cols[i].GetComponent<EnemyUnit>().TakeDamage(SpellDamage());
        }

        StartCoroutine(DeactivateSpellHitbox());
    }

    IEnumerator DeactivateSpellHitbox()
    {
        yield return new WaitForSeconds(currentSpell.spellCastTime);
        hitboxCollider.enabled = false;
        isAttacking = false;
        OnPlayerAttackEnd?.Invoke();
    }

    private void FireWeaponProjectile()
    {
        assignedProjectile = Instantiate(currentWeapon.projectile, projectileSpawnPoint.position, Quaternion.identity);
        assignedProjectile.transform.forward = modelTransform.forward;
        assignedProjectile.GetComponent<ProjectileContainer>().SetAttack(BasicAttackDamage(), 1f);
        assignedProjectile.GetComponent<Rigidbody>().AddForce(modelTransform.forward * 50f, ForceMode.Impulse);
        StartCoroutine(EndWeaponProjectile());
    }

    IEnumerator EndWeaponProjectile()
    {
        yield return new WaitForSeconds(currentWeapon.basicAttackTime);
        isAttacking = false;
        OnPlayerAttackEnd?.Invoke();
    }

    private void AttackHitscan()
    {
        if (Physics.Raycast(modelTransform.position, modelTransform.forward, out RaycastHit hit, currentWeapon.basicAttackLength, enemyLayer))
        {
            ps = Instantiate(currentWeapon.vfxPrefab).GetComponent<ParticleSystem>();
            ps.transform.position = transform.position;
            ps.transform.rotation = modelTransform.rotation;
            if (ps != null) ps.Play();
            hit.transform.GetComponent<EnemyUnit>().TakeDamage(BasicAttackDamage());
        }
        StartCoroutine(EndAttackHitscan());
    }

    IEnumerator EndAttackHitscan()
    {
        yield return new WaitForSeconds(currentWeapon.basicAttackTime);
        if (ps != null) Destroy(ps.gameObject);
        isAttacking = false;
        OnPlayerAttackEnd?.Invoke();
    }

    private void SpellHitscan()
    {
        if (Physics.Raycast(modelTransform.position, modelTransform.forward, out RaycastHit hit, currentSpell.spellLength, enemyLayer))
        {
            ps = Instantiate(currentSpell.vfxPrefab).GetComponent<ParticleSystem>();
            psMain = ps.main;
            psMain.startRotationY = modelTransform.rotation.y;
            ps.transform.position = transform.position;
            ps.transform.rotation = modelTransform.rotation;
            if (ps != null) ps.Play();
            hit.transform.GetComponent<EnemyUnit>().TakeDamage(SpellDamage());
        }
        StartCoroutine(EndSpellHitscan());
    }

    IEnumerator EndSpellHitscan()
    {
        yield return new WaitForSeconds(currentSpell.spellCastTime);
        if (ps != null) Destroy(ps.gameObject);
        isAttacking = false;
        OnPlayerAttackEnd?.Invoke();
    }

    private void SpellArea()
    {
        ps = Instantiate(currentSpell.vfxPrefab).GetComponent<ParticleSystem>();
        ps.transform.position = targetedEnemy.transform.position;
        psChild = ps.transform.GetChild(0).GetComponent<ParticleSystem>();
        psMain = ps.main;
        psMain.startSize = currentSpell.spellRadius * 5;
        psChildShape = psChild.shape;
        psChildShape.radius = currentSpell.spellRadius;
        if (ps != null) ps.Play();
        Collider[] cols = Physics.OverlapSphere(targetedEnemy.transform.position, currentSpell.spellRadius, enemyLayer);
        for (int i = 0; i < cols.Length; i++) cols[i].GetComponent<EnemyUnit>().TakeDamage(SpellDamage());
        StartCoroutine(DispelArea());
    }

    IEnumerator DispelArea()
    {
        yield return new WaitForSeconds(currentSpell.spellCastTime);
        isAttacking = false;
        OnPlayerAttackEnd?.Invoke();
        if (ps != null) Destroy(ps.gameObject, currentSpell.spellDuration - currentSpell.spellCastTime);
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
