using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAttackHandler : MonoBehaviour
{
    private GameObject assignedProjectile;
    private Collider assignedCollider;

    private PlayerUnit pu;
    private EnemyUnit eu;

    private bool hasCollidedWithPlayer = false;

    public Transform orientation;
    public AttackHit attackHit;
    public GameObject assignedObject;

    private void Start()
    {
        eu = GetComponent<EnemyUnit>();
    }

    public void StaticHitboxHit()
    {
        eu.isAttacking = true;
        InitializeHitbox();
    }

    public void ProjectileHit()
    {
        eu.isAttacking = true;
        ProjectileStartup();
    }

    public void CollisionHit()
    {
        eu.isAttacking = true;
        InitializeCollider();
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
        assignedCollider.enabled = true;
        Collider[] col = Physics.OverlapBox(assignedCollider.bounds.center, assignedCollider.bounds.extents, assignedCollider.transform.rotation, eu.playerLayer);
        foreach (Collider cold in col)
        {
            pu = cold.GetComponent<PlayerUnit>();
            pu.TakeDamage(CalculateDamage());
            //pu.InterruptPlayer(attackHit.interruptValue);
        }
        Invoke(nameof(DeactivateHitbox), attackHit.activeTime);
    }

    private void DeactivateHitbox()
    {
        assignedCollider.enabled = false;
        Invoke(nameof(EndAttack), attackHit.endTime);
    }

    private void FireProjectile()
    {
        assignedProjectile = Instantiate(assignedObject, eu.transform.position, Quaternion.identity);
        assignedProjectile.GetComponent<ProjectileContainer>().SetAttack(CalculateDamage(), attackHit.activeTime);
        assignedProjectile.GetComponent<Rigidbody>().AddForce(eu.transform.forward * attackHit.horizontalSpeed + eu.transform.up * attackHit.verticalSpeed, ForceMode.Impulse);
        Invoke(nameof(EndAttack), attackHit.endTime);
    }

    private void InitializeCollider()
    {
        assignedCollider = attackHit.assignedObject.GetComponent<Collider>();
        eu.isLockedOn = true;
        Invoke(nameof(RamIntoPlayer), attackHit.startTime);
    }

    private void RamIntoPlayer()
    {
        eu.isLockedOn = false;
        eu.agent.speed = attackHit.horizontalSpeed;
        eu.agent.SetDestination(eu.player.position);
        if (!hasCollidedWithPlayer) Invoke(nameof(RecoverFromRamming), attackHit.activeTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            hasCollidedWithPlayer = true;
            pu = collision.gameObject.GetComponent<PlayerUnit>();
            pu.TakeDamage(CalculateDamage());
            //pu.InterruptPlayer(attackHit.interruptValue);
            //pu.GetComponent<Rigidbody>().AddForce(-orientation.forward * attackHit.pushbackForce, ForceMode.Impulse);
            RecoverFromRamming();
        }
    }

    private void RecoverFromRamming()
    {
        hasCollidedWithPlayer = false;
        eu.agent.SetDestination(eu.transform.position);
        Invoke(nameof(EndAttack), attackHit.endTime);
    }

    private void EndAttack()
    {
        eu.isAttacking = false;
    }

    private float CalculateDamage()
    {
        float baseDamage = eu.attack * attackHit.attackScaling;
        if (Random.value <= eu.critChance) return baseDamage * eu.critMultiplier;
        return baseDamage;
    }
}
