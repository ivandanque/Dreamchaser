using System.Collections;
using System.Collections.Generic;
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
    //private AttackSequence ActiveAttackSequence;
    //private AttackSequence QueuedAttack;

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

        /*
        if (IsPlayerInSight)
        {
            transform.LookAt(Player);
            if (IsSafeToAttack)
            {
                QueuedAttack = ChooseAttack();
                Debug.Log(QueuedAttack);
                if (QueuedAttack != null)
                {
                    AttackRange = QueuedAttack.ActivationRange;
                    DoAttackSequence(QueuedAttack);
                }
                else Agent.SetDestination(Player.position);
            }
            else
            {
                Agent.SetDestination(Player.position);
                Agent.speed = -Agent.speed;
            }
        }
        else Agent.SetDestination(transform.position);
        */
    }

    private void DoAttackSequence()
    {
        IsAttacking = true;
        //ActiveAttackSequence = attack;

        //do attack here

        ResetAttack();
    }

    /*
    private AttackSequence ChooseAttack()
    {
        List<AttackSequence> DoableAttacks = Attacks.FindAll(attack => Physics.CheckSphere(transform.position, attack.ActivationRange, PlayerLayer));
        DoableAttacks.Sort((a,b) => a.ActivationRange.CompareTo(b.ActivationRange));
        ret
    */

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

    private void HitboxAttack(Attack attack)
    {
        StartCoroutine(ActivateCollider(attack.AssignedObject.GetComponent<Collider>(), attack.StartupTime, attack.ActiveTime, attack.RecoveryTime));
    }

    private void ProjectileAttack()
    {
        
    }

    private void CollisionAttack()
    {

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, SightRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, SafeRange);
        Gizmos.color = Color.yellow;
        //Gizmos.DrawWireSphere(transform.position, ActiveAttackSequence == null ? 0 : ActiveAttackSequence.ActivationRange);
    }

    IEnumerator ActivateCollider(Collider collider, float startup, float active, float recovery)
    {
        yield return new WaitForSeconds(startup);
        collider.enabled = true;
        yield return new WaitForSeconds(active);
        collider.enabled = false;
        yield return new WaitForSeconds(recovery);
    }
}
