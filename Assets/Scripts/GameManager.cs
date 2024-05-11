using System;
using System.Threading;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public Weapon rhweapon;
    public Weapon lhweapon;

    public Spell rhspell1;
    public Spell rhspell2;
    public Spell lhspell1;
    public Spell lhspell2;

    public bool IsLoadoutSaved = false;

    public static event Action<int> OnCountUpdate;

    private void Awake()
    {
        CreateSingleton();
    }

    void CreateSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    private void StoreLoadout(LoadoutSelect loadout)
    {
        rhweapon = loadout.rhweapon;
        lhweapon = loadout.lhweapon;

        rhspell1 = loadout.rhspell1;
        rhspell2 = loadout.rhspell2;
        lhspell1 = loadout.lhspell1;
        lhspell2 = loadout.lhspell2;

        IsLoadoutSaved = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Game Closed");
            Application.Quit();
        }
    }

    private void OnEnable()
    {
        LoadoutSelect.OnPreserveLoadout += StoreLoadout;
    }

    private void OnDisable()
    {
        LoadoutSelect.OnPreserveLoadout -= StoreLoadout;
    }
}