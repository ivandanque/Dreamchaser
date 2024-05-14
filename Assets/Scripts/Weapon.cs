using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon")]
public class Weapon : ScriptableObject
{
    [TextArea]
    public string description;
    [Space]
    public float baseAttack;
    public float basicAttackTime;
    [Space]
    public AttackHitType attackHitType;
    public float basicAttackLength;
    public float basicAttackWidth;
    public float basicAttackHeight;
    public float basicAttackRadius;

    public GameObject vfxPrefab;
    public GameObject projectile;
}
