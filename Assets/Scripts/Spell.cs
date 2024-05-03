using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Spell")]
public class Spell : ScriptableObject
{
    [TextArea] public string description;

    public float attackScaling;
    public float spellTime;

    public AttackHitType spellType;
    public float spellWidth;
    public float spellHeight;
    public float spellLength;
    public float spellRadius;
}
