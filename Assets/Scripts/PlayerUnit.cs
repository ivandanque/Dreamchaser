using System;
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
    public float defenseFactor;
    public float damageCooldown;

    private float currentHealth;
    private float damageCooldownCtr;

    public Transform targetedEnemy;

    private bool isFallen;
    private bool isRecentlyDamaged;

    public static event Action OnPlayerDeath;

    private void Start()
    {
        PM = GetComponent<PlayerMovement>();
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    private void Update()
    {
        CheckDamageRecency();
    }

    private void CheckDamageRecency()
    {
        if (isRecentlyDamaged) damageCooldownCtr -= Time.deltaTime;
        else damageCooldownCtr = damageCooldown;

        if (damageCooldownCtr <= 0) isRecentlyDamaged = false;
    }

    public void TakeDamage(float damage)
    {
        if (isRecentlyDamaged) return;
        isRecentlyDamaged = true;
        currentHealth -= damage * DefenseMultiplier();
        healthBar.SetHealth(currentHealth);
        Debug.Log("Player took " + damage * DefenseMultiplier() + " damage!");
        if (currentHealth <= 0) OnPlayerDeath?.Invoke();
    }

    public void Kill()
    {
        TakeDamage(maxHealth / DefenseMultiplier());
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
