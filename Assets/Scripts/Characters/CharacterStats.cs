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
        defaultStats.Alive = defaultStats.Health > 0;

        if (GetComponent<Animator>())
            anim = GetComponent<Animator>();
    }

    public virtual void Start()
    {
        print("It has started" + gameObject.name);
    }


    public virtual void Update()
    {
        defaultStats.Alive = defaultStats.Health > 0;

        if(!defaultStats.Alive)
        {
            GetComponent<Rigidbody>().useGravity = false;
            GetComponent<Collider>().isTrigger = true;
        }
    }

}
