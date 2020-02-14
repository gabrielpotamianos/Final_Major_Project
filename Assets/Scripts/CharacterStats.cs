using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [Space(10)]
    [Header("Stats")]
    
    [SerializeField]

    private Stat AttackPower;
    [SerializeField]
    private Stat Armour;


    public float maxHealth;
    public float currHealth;

}
