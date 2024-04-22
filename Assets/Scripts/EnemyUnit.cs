using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.AI;

public class EnemyUnit : MonoBehaviour
{
    [Header("References")]
    private GameManager GM;
    private Transform Player;
    public NavMeshAgent Agent;
    private List<Attack> Attacks;

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
    public float AttackRange;
    public float SafeRange;

    private float InterruptMeter;
    private Attack ActiveAttack;

    private bool IsPlayerInSight;
    private bool IsPlayerInAttackRange;
    private bool IsSafeToAttack;
    private bool IsAttacking;
    private bool IsMoving;

    private void Start()
    {
        GM = GameObject.Find("GameManager").GetComponent<GameManager>();
        Player = GameObject.Find("Player").transform;
        Agent = GetComponent<NavMeshAgent>();
        if (!gameObject.CompareTag("Player")) SetStats(GM.Enemies.Find(unit => Name.Equals(unit.name)));
    }

    private void Update()
    {
        if (IsAttacking) return;

        IsPlayerInSight = Physics.CheckSphere(Player.position, SightRange, PlayerLayer);
        IsSafeToAttack = Physics.CheckSphere(Player.position, SafeRange, PlayerLayer);

        if (IsPlayerInSight)
        {
            transform.LookAt(Player);
            if (IsSafeToAttack) AttackPlayer(ChooseAttack());
            else
            {
                Agent.SetDestination(Player.position);
                Agent.speed = -Agent.speed;
            }
        }
        else Agent.SetDestination(transform.position);
    }

    private void AttackPlayer(Attack attack)
    {
        IsAttacking = true;
        ActiveAttack = attack;

        Invoke(nameof(ResetAttack), attack.AttackCooldown);
    }

    private Attack ChooseAttack()
    {
        List<Attack> DoableAttacks = Attacks.FindAll(attack => IsPlayerInAttackRange = Physics.CheckSphere(Player.position, attack.Range, PlayerLayer));
        DoableAttacks.Sort((a,b) => a.Range.CompareTo(b.Range));
        return DoableAttacks[0];
    }

    private void ResetAttack()
    {
        IsAttacking = false;
    }    

    public void TakeDamage(float damage)
    {
        Health -= damage * DefenseMultiplier();
    }

    public float DealDamage()
    {
        if (Random.value <= CritChance) return Attack * CritMultiplier;
        return Attack;
    }

    private float DefenseMultiplier()
    {
        return (10 * (DefenseFactor - 10)) / ((10 * DefenseFactor) - Defense - 100);
    }

    private void SetStats(Unit unit)
    {
        Name = unit.name;
        Health = unit.health;
        Attack = unit.attack;
        Defense = unit.defense;
        DefenseFactor = unit.defenseFactor;
        CritChance = unit.critChance;
        CritMultiplier = unit.critMultiplier;
        Attacks = new List<Attack>(unit.attacks);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, SightRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, SafeRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, ActiveAttack == null ? 0 : ActiveAttack.Range);
    }
}
