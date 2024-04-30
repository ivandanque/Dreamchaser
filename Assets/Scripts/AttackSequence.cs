using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AttackSequence
{
    public string name;
    public float activationRange;
    public List<Attack> attacks = new();
}

[System.Serializable]
public class Attack
{
    [Header("Attack Data")]
    public AttackType attackType;
    public float attackScaling;
    [TextArea]
    public string description;

    [Header("Frame Data")]
    public float startTime;
    public float activeTime;
    public float endTime;

    [Header("Other Attack Data")]
    public float interruptValue;
    public float horizontalSpeed;
    public float verticalSpeed;
    public float pushbackForce;

    public GameObject assignedObject;
}

public enum AttackType
{
    StaticHitbox,
    Projectile,
    Collision
}