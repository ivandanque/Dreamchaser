using System.Collections.Generic;
using UnityEngine;

public class PlayerUnit : MonoBehaviour
{
    [Header("References")]
    private GameManager GM;
    private PlayerMovement PM;
    public HealthBar healthBar;
    //public List<AttackSequence> Attacks;

    [Header("Stats")]
    public new string name;
    public float maxHealth;
    public float defense;
    private float defenseFactor;

    private float currentHealth;

    public Transform targetedEnemy;

    private bool isFallen;

    public bool isAttacking = false;
    private bool isRecentlyDamaged;

    private void Start()
    {
        PM = GetComponent<PlayerMovement>();
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    private void Update()
    {
        if (isAttacking) return;
    }

    public void TakeDamage(float damage)
    {
        isRecentlyDamaged = true;
        currentHealth -= damage * DefenseMultiplier();
        healthBar.SetHealth(currentHealth);
    }

    private float DefenseMultiplier()
    {
        return (10 * (defenseFactor - 10)) / ((10 * defenseFactor) - defense - 100);
    }

    /*
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, SightRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, SafeRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, ActiveAttack == null ? 0 : ActiveAttack.Range);
    }
    */

    private void OnEnable()
    {
        EnemyAttackHandler.OnPlayerHit += TakeDamage;
    }

    private void OnDisable()
    {
        EnemyAttackHandler.OnPlayerHit -= TakeDamage;
    }
}
