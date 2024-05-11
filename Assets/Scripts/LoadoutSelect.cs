using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadoutSelect : MonoBehaviour
{
    public TMP_Dropdown rhwDropdown;
    public TMP_Dropdown lhwDropdown;

    public TMP_Dropdown lhs1Dropdown;
    public TMP_Dropdown lhs2Dropdown;
    public TMP_Dropdown rhs1Dropdown;
    public TMP_Dropdown rhs2Dropdown;

    public GameObject loadoutPanel;

    public List<Weapon> weapons = new();
    public List<Spell> spells = new();

    private Button selectedButton;

    public Weapon rhweapon;
    public Weapon lhweapon;

    public Spell rhspell1;
    public Spell rhspell2;
    public Spell lhspell1;
    public Spell lhspell2;

    private bool isScreenUp = false;

    public static event Action<LoadoutSelect> OnLoadoutConfirm;
    public static event Action<LoadoutSelect> OnPreserveLoadout;

    public void Start()
    {
        
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (!isScreenUp)
            {
                loadoutPanel.SetActive(true);
                Invoke(nameof(InitLoadout), 0.02f);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                isScreenUp = true;
            }
            else ScreenDown();
        }
    }

    public void UnselectDuplicate(TMP_Dropdown dropdown)
    {
        if (!dropdown.name.Equals(lhs1Dropdown.name)) if (dropdown.captionText.text.Equals(lhs1Dropdown.captionText.text)) lhs1Dropdown.captionText.text = "--";
        if (!dropdown.name.Equals(lhs2Dropdown.name)) if (dropdown.captionText.text.Equals(lhs2Dropdown.captionText.text)) lhs2Dropdown.captionText.text = "--";
        if (!dropdown.name.Equals(rhs1Dropdown.name)) if (dropdown.captionText.text.Equals(rhs1Dropdown.captionText.text)) rhs1Dropdown.captionText.text = "--";
        if (!dropdown.name.Equals(rhs2Dropdown.name)) if (dropdown.captionText.text.Equals(rhs2Dropdown.captionText.text)) rhs2Dropdown.captionText.text = "--";
    }

    public void SetLoadout()
    {
        rhweapon = weapons.Find(weapon => weapon.name.Equals(rhwDropdown.captionText.text));
        lhweapon = weapons.Find(weapon => weapon.name.Equals(lhwDropdown.captionText.text));

        rhspell1 = spells.Find(spell => spell.name.Equals(rhs1Dropdown.captionText.text));
        rhspell2 = spells.Find(spell => spell.name.Equals(rhs2Dropdown.captionText.text));
        lhspell1 = spells.Find(spell => spell.name.Equals(lhs1Dropdown.captionText.text));
        lhspell2 = spells.Find(spell => spell.name.Equals(lhs2Dropdown.captionText.text));

        OnLoadoutConfirm?.Invoke(this);
        OnPreserveLoadout?.Invoke(this);
    }

    private void GrabLoadout()
    {
        Loadout loadout = GameObject.FindGameObjectWithTag("Player").GetComponent<Loadout>();

        rhwDropdown.captionText.text = loadout.rightHandWeapon.name;
        lhwDropdown.captionText.text = loadout.leftHandWeapon.name;

        lhs1Dropdown.captionText.text = loadout.leftHandSpell1.name;
        lhs2Dropdown.captionText.text = loadout.leftHandSpell2.name;
        rhs1Dropdown.captionText.text = loadout.rightHandSpell1.name;
        rhs2Dropdown.captionText.text = loadout.rightHandSpell2.name;

        SetLoadout();
    }

    public void ScreenDown()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        isScreenUp = false;
        loadoutPanel.SetActive(false);
    }

    private void LoadLoadout()
    {
        rhwDropdown.captionText.text = GameManager.Instance.rhweapon.name;
        lhwDropdown.captionText.text = GameManager.Instance.lhweapon.name;

        lhs1Dropdown.captionText.text = GameManager.Instance.lhspell1.name;
        lhs2Dropdown.captionText.text = GameManager.Instance.lhspell2.name;
        rhs1Dropdown.captionText.text = GameManager.Instance.rhspell1.name;
        rhs2Dropdown.captionText.text = GameManager.Instance.rhspell2.name;

        SetLoadout();
    }

    private void InitLoadout()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsLoadoutSaved) LoadLoadout();
        else GrabLoadout();

        Time.timeScale = 0f;
    }
}
