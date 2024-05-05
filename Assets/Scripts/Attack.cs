using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Attack
{
    public string name;
    public float activationRange;
    public List<AttackHit> attacks = new();
}

[System.Serializable]
public class AttackHit
{
    [Header("Hit Data")]
    public AttackHitType attackType;
    public float attackScaling;
    [TextArea]
    public string description;

    [Header("Frame Data")]
    public float startTime;
    public float activeTime;
    public float endTime;

    [Header("More Hit Data")]
    public float interruptValue;
    public float horizontalSpeed;
    public float verticalSpeed;
    public float movementSpeed;

    public GameObject assignedObject;
}

public enum AttackHitType
{
    Hitbox,
    Hitsphere,
    Point,
    Projectile
}