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


    private void Update()
    {
        defaultStats.Health = Mathf.Clamp(defaultStats.Health, 0, maxHealth);
    }

}
