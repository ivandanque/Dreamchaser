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
    private AttackHandler AH;
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
    public Attack ActiveHit;
    public GameObject ActiveObject;

    [SerializeField] private bool IsPlayerInSight;
    [SerializeField] private bool IsPlayerInAttackRange;
    [SerializeField] private bool IsSafeToAttack;
    public bool IsAttacking;
    [SerializeField] private bool IsLockedOn;
    private bool IsMoving;

    private void Start()
    {
        GM = GameObject.Find("GameManager").GetComponent<GameManager>();
        Player = GameObject.Find("Player").transform;
        AH = GetComponent<AttackHandler>();
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
            else Agent.SetDestination(transform.position + transform.position - Player.position);
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
                    AH.StaticColliderAttack();
                    break;
                case AttackType.Projectile:
                    AH.ProjectileAttack();
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, SightRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, SafeRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, ActiveAttackSequence == null ? 0 : ActiveAttackSequence.ActivationRange);
    }
}
