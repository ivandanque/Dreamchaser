using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;
using UnityEngine.AI;

public class EnemyUnit : MonoBehaviour
{
    [Header("References")]
    private GameManager GM;
    [SerializeField] private Transform Player;
    public NavMeshAgent Agent;
    public List<AttackSequence> Attacks;

    [Header("Stats")]
    public string Name;
    public float Health;
    public float Attack;
    public float Defense;
    private float DefenseFactor = 0f;
    public float CritChance;
    public float CritMultiplier;

    [Header("Settings")]
    public LayerMask PlayerLayer;
    public float SightRange;
    private float AttackRange;
    public float SafeRange;

    private float InterruptMeter;
    private AttackSequence ActiveAttackSequence;
    private AttackSequence QueuedAttack;
    private Attack ActiveHit;
    private GameObject ActiveObject;

    [SerializeField] private bool IsPlayerInSight;
    [SerializeField] private bool IsPlayerInAttackRange;
    [SerializeField] private bool IsSafeToAttack;
    [SerializeField] private bool IsAttacking;
    [SerializeField] private bool IsLockedOn;
    private bool IsMoving;

    private void Start()
    {
        GM = GameObject.Find("GameManager").GetComponent<GameManager>();
        Player = GameObject.Find("Player").transform;
        Agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (IsAttacking)
        {
            if (IsLockedOn) transform.LookAt(Player);
            return;
        }

        IsPlayerInSight = Physics.CheckSphere(transform.position, SightRange, PlayerLayer);
        IsSafeToAttack = !Physics.CheckSphere(transform.position, SafeRange, PlayerLayer);

        if (IsPlayerInSight)
        {
            transform.LookAt(Player);
            if (IsSafeToAttack)
            {
                QueuedAttack = ChooseAttack();
                if (QueuedAttack != null)
                {
                    Debug.Log(QueuedAttack.Name);
                    Agent.SetDestination(transform.position);
                    AttackRange = QueuedAttack.ActivationRange;
                    DoAttackSequence(QueuedAttack);
                }
                else Agent.SetDestination(Player.position);
            }
            else
            {
                Agent.SetDestination(Player.position);
                Agent.velocity = -Agent.velocity;
            }
        }
        else Agent.SetDestination(transform.position);
    }

    private void DoAttackSequence(AttackSequence attack)
    {
        IsAttacking = true;
        ActiveAttackSequence = attack;

        foreach (Attack hit in attack.attacks)
        {
            ActiveHit = hit;
            ActiveObject = hit.AssignedObject;
            switch (hit.Type)
            {
                case AttackType.Hitbox:
                    Invoke(nameof(ActivateCollider), hit.StartupTime);
                    break;
                case AttackType.Projectile:
                    Invoke(nameof(FireProjectile), hit.StartupTime);
                    break;
                case AttackType.Collision:
                    break;
                default: 
                    break;
            }
        }
    }

    private AttackSequence ChooseAttack()
    {
        List<AttackSequence> DoableAttacks = Attacks.FindAll(attack => Physics.CheckSphere(transform.position, attack.ActivationRange, PlayerLayer));
        DoableAttacks.Sort((a, b) => a.ActivationRange.CompareTo(b.ActivationRange));
        return DoableAttacks.Count == 0 ? null : DoableAttacks[0];
    }

    public void TakeDamage(float damage)
    {
        Health -= damage * DefenseMultiplier();
    }

    public float DealDamage()
    {
        if (Random.value <= CritChance) return Attack * ActiveHit.AttackScaling * CritMultiplier;
        return Attack * ActiveHit.AttackScaling;
    }

    private float DefenseMultiplier()
    {
        return (10 * (DefenseFactor - 10)) / ((10 * DefenseFactor) - Defense - 100);
    }

    private void ResetAttack()
    {
        ActiveObject = null;
        IsAttacking = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, SightRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, SafeRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, ActiveAttackSequence == null ? 0 : ActiveAttackSequence.ActivationRange);
    }

    private void ActivateCollider()
    {
        Collider collider = ActiveObject.GetComponent<Collider>();
        Collider[] col = Physics.OverlapBox(collider.bounds.center, collider.bounds.extents, collider.transform.rotation, PlayerLayer);
        PlayerUnit pu;
        foreach (Collider cold in col)
        {
            pu = cold.GetComponent<PlayerUnit>();
            pu.TakeDamage(DealDamage());
            pu.InterruptPlayer(ActiveHit.InterruptValue);
        }
        Invoke(nameof(ResetAttack), ActiveHit.RecoveryTime);
    }

    private void FireProjectile()
    {
        //Rigidbody rb = Instantiate(ActiveObject);
    }

    private void RamAttack()
    {

    }
}
