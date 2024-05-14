using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loadout : MonoBehaviour
{
    [Header("Right Hand")]
    public Weapon rightHandWeapon;
    public Spell rightHandSpell1;
    public Spell rightHandSpell2;

    [Header("Left Hand")]
    public Weapon leftHandWeapon;
    public Spell leftHandSpell1;
    public Spell leftHandSpell2;

    public static event Action<Weapon> OnBasicAttackStart;
    public static event Action<Weapon, Spell> OnSpellStart;

    private void Update()
    {
        if (Time.timeScale != 0) CheckInput();
    }

    private void CheckInput()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0)) OnBasicAttackStart?.Invoke(rightHandWeapon);
        if (Input.GetKeyDown(KeyCode.Mouse1)) OnBasicAttackStart?.Invoke(leftHandWeapon);

        if (Input.GetKeyDown(KeyCode.Alpha1)) OnSpellStart?.Invoke(leftHandWeapon, leftHandSpell1);
        if (Input.GetKeyDown(KeyCode.Q)) OnSpellStart?.Invoke(leftHandWeapon, leftHandSpell2);
        if (Input.GetKeyDown(KeyCode.Alpha3)) OnSpellStart?.Invoke(rightHandWeapon, rightHandSpell1);
        if (Input.GetKeyDown(KeyCode.E)) OnSpellStart?.Invoke(rightHandWeapon, rightHandSpell2);
    }

    private void SetLoadout(LoadoutSelect ls)
    {
        rightHandWeapon = ls.rhweapon;
        leftHandWeapon = ls.lhweapon;

        rightHandSpell1 = ls.rhspell1;
        rightHandSpell2 = ls.rhspell2;
        leftHandSpell1 = ls.lhspell1;
        leftHandSpell2 = ls.lhspell2;
    }

    private void OnEnable()
    {
        LoadoutSelect.OnLoadoutConfirm += SetLoadout;
    }

    private void OnDisable()
    {
        LoadoutSelect.OnLoadoutConfirm -= SetLoadout;
    }
}