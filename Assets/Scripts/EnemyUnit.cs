using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;
using UnityEngine.AI;

public class EnemyUnit : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public NavMeshAgent agent;
    private EnemyAttackHandler EAH;
    public List<Attack> attacks;

    [Header("Stats")]
    public new string name;
    public float health;
    public float attack;
    public float defense;
    private float defenseFactor = 0f;
    public float critChance;
    public float critMultiplier;

    [Header("Settings")]
    public LayerMask playerLayer;
    public float sightRange;
    private float attackRange;
    public float safeRange;

    private float interruptMeter;
    private Attack activeAttack;
    private Attack queuedAttack;
    public AttackHit activeHit;
    public GameObject activeObject;

    [SerializeField] private bool isPlayerInSight;
    [SerializeField] private bool isPlayerInAttackRange;
    [SerializeField] private bool isSafeToAttack;
    public bool isAttacking;
    public bool isLockedOn;
    private bool isMoving;

    public static event Action<EnemyUnit, AttackHit> OnHit;
    public static event Action OnHitboxHit;
    public static event Action OnProjectileHit;
    public static event Action OnCollisionHit;

    private void Start()
    {
        player = GameObject.Find("Player").transform;
        EAH = GetComponent<EnemyAttackHandler>();
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (isAttacking)
        {
            if (isLockedOn) transform.LookAt(player);
            return;
        }

        isPlayerInSight = Physics.CheckSphere(transform.position, sightRange, playerLayer);
        isSafeToAttack = !Physics.CheckSphere(transform.position, safeRange, playerLayer);

        if (isPlayerInSight)
        {
            transform.LookAt(player);
            if (isSafeToAttack)
            {
                queuedAttack = ChooseAttack();
                if (queuedAttack != null)
                {
                    Debug.Log(queuedAttack.name);
                    agent.SetDestination(transform.position);
                    attackRange = queuedAttack.activationRange;
                    DoAttack(queuedAttack);
                }
                else agent.SetDestination(player.position);
            }
            else agent.SetDestination(transform.position + transform.position - player.position);
        }
        else agent.SetDestination(transform.position);
    }

    private void DoAttack(Attack attack)
    {
        isAttacking = true;
        activeAttack = attack;

        foreach (AttackHit hit in attack.attacks)
        {
            OnHit?.Invoke(this, hit);
            switch (hit.attackType)
            {
                case AttackHitType.StaticHitbox:
                    OnHitboxHit?.Invoke();
                    break;
                case AttackHitType.Projectile:
                    OnProjectileHit?.Invoke();
                    break;
                case AttackHitType.Collision:
                    OnCollisionHit?.Invoke();
                    break;
            }
        }
    }

    private Attack ChooseAttack()
    {
        List<Attack> DoableAttacks = attacks.FindAll(attack => Physics.CheckSphere(transform.position, attack.activationRange, playerLayer));
        DoableAttacks.Sort((a, b) => a.activationRange.CompareTo(b.activationRange));
        return DoableAttacks.Count == 0 ? null : DoableAttacks[0];
    }

    public void TakeDamage(float damage)
    {
        health -= damage * DefenseMultiplier();
    }

    private float DefenseMultiplier()
    {
        return (10 * (defenseFactor - 10)) / ((10 * defenseFactor) - defense - 100);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, safeRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, activeAttack == null ? 0 : activeAttack.activationRange);
    }

    private void OnEnable()
    {
        PlayerAttackHandler.OnEnemyHit += TakeDamage;
    }

    private void OnDisable()
    {
        PlayerAttackHandler.OnEnemyHit -= TakeDamage;
    }
}
