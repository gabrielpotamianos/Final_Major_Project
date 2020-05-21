
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class CharacterInfo
{
    public enum Race { Female, Male };
    public enum Breed { Mage, Rogue, Warrior };

    public Race race;
    public Breed breed;
    public string name;

    [SerializeField]
    public List<Item> items;
    
    [SerializeField]
    public Vector3 Position;

    [SerializeField]
    public Vector3 Rotation;
    //[HideInInspector]
    [System.NonSerialized]
    public GameObject Character;

    public CharacterInfo()
    {
        items = new List<Item>();
    }

    public CharacterInfo(Race RACE, Breed BREED)
    {
        race = RACE;
        breed = BREED;
        items = new List<Item>();
        Position= new Vector3();
        Rotation= new Vector3();
    }

    public void SetCharacter(GameObject PickCharacter)
    {
        Character = PickCharacter;
    }

    public bool IsTheSame(CharacterInfo charInfo)
    {
        if (this.race == charInfo.race && this.breed == charInfo.breed)
            return true;
        return false;
    }

}
