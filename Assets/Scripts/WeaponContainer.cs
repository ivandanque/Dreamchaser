using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponContainer : MonoBehaviour
{
    [Header("Right Hand")]
    public Weapon rightHandWeapon;
    public KeyCode rightHandKey = KeyCode.Mouse0;
    public GameObject rightHandObject;

    [Header("Left Hand")]
    public Weapon leftHandWeapon;
    public KeyCode leftHandKey = KeyCode.Mouse1;
    public GameObject leftHandObject;

    public GameObject[] requiredObjects = new GameObject[5];

    private PlayerAttackHandler pah;

    public static event Action<PlayerUnit, Weapon, GameObject> OnAttackInit;
    public static event Action OnBasicAttack;

    private void Start()
    {
        pah = GetComponent<PlayerAttackHandler>();
    }

    private void Update()
    {
        SetRightWeapon();
        SetLeftWeapon();

        CheckInput();
    }

    private void CheckInput()
    {
        if (Input.GetKey(rightHandKey))
        {
            OnAttackInit?.Invoke(GetComponent<PlayerUnit>(), rightHandWeapon, rightHandObject);
            OnBasicAttack?.Invoke();
        }
        if (Input.GetKey(leftHandKey) && leftHandWeapon != null)
        {
            OnAttackInit?.Invoke(GetComponent<PlayerUnit>(), leftHandWeapon, leftHandObject);
            OnBasicAttack?.Invoke();
        }
    }

    private void SetRightWeapon()
    {
        switch (rightHandWeapon.name)
        {
            case "Hand":
                rightHandObject = requiredObjects[0];
                break;
            case "Blade":
                rightHandObject = requiredObjects[1];
                break;
            case "Gauntlet":
                rightHandObject = requiredObjects[2];
                break;
            case "Scepter":
                rightHandObject = requiredObjects[3];
                break;
            case "Wand":
                rightHandObject = requiredObjects[4];
                break;
        }
    }

    private void SetLeftWeapon()
    {
        switch (leftHandWeapon.name)
        {
            case "Hand":
                if (rightHandWeapon.name.Equals("Hand")) leftHandWeapon = null;
                else leftHandObject = requiredObjects[0];
                break;
            case "Blade":
                leftHandObject = requiredObjects[1];
                break;
            case "Gauntlet":
                leftHandObject = requiredObjects[2];
                break;
            case "Scepter":
                leftHandObject = requiredObjects[3];
                break;
            case "Wand":
                leftHandObject = requiredObjects[4];
                break;
        }
    }
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