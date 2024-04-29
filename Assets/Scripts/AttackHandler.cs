using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHandler : MonoBehaviour
{
    private GameObject assignedProjectile;
    private Collider assignedCollider;

    private PlayerUnit PU;
    private EnemyUnit EU;

    private void Start()
    {
        EU = GetComponent<EnemyUnit>();
    }

    public void StaticColliderAttack()
    {
        EU.IsAttacking = true;
        InitializeCollider();
    }

    public void ProjectileAttack()
    {
        EU.IsAttacking = true;
        FireProjectile();
    }

    private void InitializeCollider()
    {
        assignedCollider = EU.ActiveObject.GetComponent<Collider>();
        assignedCollider.enabled = false;
        Invoke(nameof(ActivateCollider), EU.ActiveHit.StartTime);
    }

    private void InitializeProjectile()
    {
        assignedProjectile = EU.ActiveObject;
    }

    private void ActivateCollider()
    {
        assignedCollider.enabled = true;
        Collider[] col = Physics.OverlapBox(assignedCollider.bounds.center, assignedCollider.bounds.extents, assignedCollider.transform.rotation, EU.PlayerLayer);
        foreach (Collider cold in col)
        {
            PU = cold.GetComponent<PlayerUnit>();
            PU.TakeDamage(EU.DealDamage());
            PU.InterruptPlayer(EU.ActiveHit.InterruptValue);
        }
        Invoke(nameof(DeactivateCollider), EU.ActiveHit.ActiveTime);
    }

    private void DeactivateCollider()
    {
        assignedCollider.enabled = false;
        Invoke(nameof(EndAttack), EU.ActiveHit.EndTime);
    }

    private void FireProjectile()
    {

    }

    private void EndAttack()
    {
        EU.IsAttacking = false;
    }
}
