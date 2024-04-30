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
    public Attack attack;
    public GameObject assignedObject;

    private void Start()
    {
        eu = GetComponent<EnemyUnit>();
    }

    public void StaticHitboxAttack()
    {
        eu.isAttacking = true;
        InitializeHitbox();
    }

    public void ProjectileAttack()
    {
        eu.isAttacking = true;
        ProjectileStartup();
    }

    public void CollideAttack()
    {
        eu.isAttacking = true;
        InitializeCollider();
    }

    private void InitializeHitbox()
    {
        assignedCollider = attack.assignedObject.GetComponent<Collider>();
        assignedCollider.enabled = false;
        Invoke(nameof(ActivateHitbox), attack.startTime);
    }

    private void ProjectileStartup()
    {
        Invoke(nameof(FireProjectile), attack.startTime);
    }

    private void ActivateHitbox()
    {
        assignedCollider.enabled = true;
        Collider[] col = Physics.OverlapBox(assignedCollider.bounds.center, assignedCollider.bounds.extents, assignedCollider.transform.rotation, eu.playerLayer);
        foreach (Collider cold in col)
        {
            pu = cold.GetComponent<PlayerUnit>();
            pu.TakeDamage(CalculateDamage());
            pu.InterruptPlayer(attack.interruptValue);
        }
        Invoke(nameof(DeactivateHitbox), attack.activeTime);
    }

    private void DeactivateHitbox()
    {
        assignedCollider.enabled = false;
        Invoke(nameof(EndAttack), attack.endTime);
    }

    private void FireProjectile()
    {
        assignedProjectile = Instantiate(assignedObject, eu.transform.position, Quaternion.identity);
        assignedProjectile.GetComponent<ProjectileContainer>().SetAttack(CalculateDamage(), attack);
        assignedProjectile.GetComponent<Rigidbody>().AddForce(eu.transform.forward * attack.horizontalSpeed + eu.transform.up * attack.verticalSpeed, ForceMode.Impulse);
        Invoke(nameof(EndAttack), attack.endTime);
    }

    private void InitializeCollider()
    {
        assignedCollider = attack.assignedObject.GetComponent<Collider>();
        eu.isLockedOn = true;
        Invoke(nameof(RamIntoPlayer), attack.startTime);
    }

    private void RamIntoPlayer()
    {
        eu.isLockedOn = false;
        eu.agent.speed = attack.horizontalSpeed;
        eu.agent.SetDestination(eu.player.position);
        if (!hasCollidedWithPlayer) Invoke(nameof(RecoverFromRamming), attack.activeTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            hasCollidedWithPlayer = true;
            pu = collision.gameObject.GetComponent<PlayerUnit>();
            pu.TakeDamage(CalculateDamage());
            pu.InterruptPlayer(attack.interruptValue);
            pu.GetComponent<Rigidbody>().AddForce(-orientation.forward * attack.pushbackForce, ForceMode.Impulse);
            RecoverFromRamming();
        }
    }

    private void RecoverFromRamming()
    {
        hasCollidedWithPlayer = false;
        eu.agent.SetDestination(eu.transform.position);
        Invoke(nameof(EndAttack), attack.endTime);
    }

    private void EndAttack()
    {
        eu.isAttacking = false;
    }

    private float CalculateDamage()
    {
        float baseDamage = eu.attack * attack.attackScaling;
        if (Random.value <= eu.critChance) return baseDamage * eu.critMultiplier;
        return baseDamage;
    }
}
