using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class CharacterStats : MonoBehaviour
{
    [System.Serializable]
    public struct CharacterBaseClass
    {
        public string Name;
        public bool Hostile;
        public Image Icon;

        [SerializeField]
        public float Health;
        public bool Alive;
    }

    [Space(10)]
    [Header("Stats")]

    [SerializeField]
    public CharacterBaseClass defaultStats;

    [SerializeField]
    public float AttackPower;

    [SerializeField]
    public float Armour;


    public float maxHealth;


    public Animator anim;
    public virtual void Awake()
    {
        if (GetComponent<Animator>())
            anim = GetComponent<Animator>();
        defaultStats.Alive = defaultStats.Health > 0;

    }


    public virtual void Update()
    {
        defaultStats.Alive = defaultStats.Health > 0;
    }

}
