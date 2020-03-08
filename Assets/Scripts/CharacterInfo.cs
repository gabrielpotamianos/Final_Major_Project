
using UnityEngine;

[System.Serializable]
public class CharacterInfo
{
    public enum Race { Female, Male };
    public enum Breed { Mage, Rogue, Warrior };

    public Race race;
    public Breed breed;

    //[HideInInspector]
    [System.NonSerialized]     
    public GameObject Character;

    public CharacterInfo()
    {
    }

    public CharacterInfo(Race RACE, Breed BREED)
    {
        race = RACE;
        breed = BREED;
    }

    public void SetCharacter(GameObject PickCharacter)
    {
        Character = PickCharacter;
    }

}
