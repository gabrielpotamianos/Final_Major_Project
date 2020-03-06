using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseClassCharacterCreation : MonoBehaviour
{
    public enum Race { Female, Male };
    public enum Class { Mage, Rogue, Warrior };
    public Race race;
    public Class CharacterClass;
}
