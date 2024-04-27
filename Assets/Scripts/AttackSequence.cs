using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AttackSequence
{
    public string Name;
    public float ActivationRange;
    public List<Attack> attacks = new();
}

[System.Serializable]
public class Attack
{
    [Header("Attack Data")]
    public AttackType Type;
    public float AttackScaling;
    public string Description;

    [Header("Frame Data")]
    public float StartupTime;
    public float ActiveTime;
    public float RecoveryTime;

    [Header("Other Attack Data")]
    public float InterruptValue;
    public float MovementForce;
    public float PushbackForce;

    public GameObject AssignedObject;
}

public enum AttackType
{
    Hitbox,
    Projectile,
    Collision
}
