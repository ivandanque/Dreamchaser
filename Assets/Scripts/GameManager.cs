using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<Unit> Enemies = new List<Unit>();
}

[System.Serializable]
public class Unit
{
    public string name;
    public float health;
    public float attack;
    public float defense;
    public float defenseFactor;
    public float critChance;
    public float critMultiplier;

    public List<Attack> attacks = new List<Attack>();
}

[System.Serializable]
public class Attack
{
    public float Damage;
    public float Range;
    public float ActiveFrames;
    public float MovementForce;
    public float InterruptValue;
    public float AttackCooldown;
}