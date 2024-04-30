using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapons : MonoBehaviour
{
    [Header("Right Hand")]
    public Weapon rightHandWeapon;
    public KeyCode rightHandKey = KeyCode.Mouse0;

    [Header("Left Hand")]
    public Weapon leftHandWeapon;
    public KeyCode leftHandKey = KeyCode.Mouse1;
}

[CreateAssetMenu(fileName = "New Weapon")]
public class Weapon : ScriptableObject
{
    [TextArea]
    public string description;
    [Space]
    public float baseAttack;
    public float basicAttackStartup;
    public float basicAttackTime;
    public float basicAttackRecovery;
    [Space]
    public float interruptValue;
    public float basicAttackKnockbackIncrease;
    public float spellInterruptIncrease;
    public float basicAttackStunChance;
}