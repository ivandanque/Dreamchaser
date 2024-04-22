using System.Collections.Generic;
using UnityEngine;

public class PlayerUnit : MonoBehaviour
{
    [Header("References")]
    private GameManager GM;
    public List<Attack> Attacks;
    public List<Collider> AttackColliders;

    [Header("Stats")]
    public string Name;
    public float Health;
    public float Attack;
    public float Defense;
    private float DefenseFactor;
    public float CritChance;
    public float CritMultiplier;

    public float DamageInterruptionInterval;
    public float InterruptMeterMax;
    private float InterruptMeter;

    private bool IsFallen;

    private bool IsRecentlyDamaged;

    private void Update()
    {
        if (IsFallen)
        {
            if (!IsRecentlyDamaged)
            {
                InterruptMeter += Time.deltaTime / 3;
                if (InterruptMeter >= InterruptMeterMax)
                {
                    IsFallen = false;
                    InterruptMeter = InterruptMeterMax;
                }
            }
            else InterruptMeter = 0;
        }
    }

    public void TakeDamage(float damage)
    {
        IsRecentlyDamaged = true;
        Health -= damage * DefenseMultiplier();
        Invoke(nameof(ResetDamageInterrupt), DamageInterruptionInterval);
    }

    public float DealDamage()
    {
        if (Random.value <= CritChance) return Attack * CritMultiplier;
        return Attack;
    }

    public void InterruptPlayer(float interruptValue)
    {
        InterruptMeter -= interruptValue;
        if (InterruptMeter <= 0)
        {
            IsFallen = true;
            InterruptMeter = 0;
        }
    }

    private void ResetDamageInterrupt()
    {
        IsRecentlyDamaged = false;
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
}
