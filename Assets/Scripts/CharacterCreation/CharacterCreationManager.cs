using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCreationManager : BaseClassCharacterCreation
{
    public SelectCharacter[] characters;

    public Race currRace = Race.Male;
    public Class currCharacterClass=Class.Mage;
    SelectCharacter currCharacter;

    #region Singleton
    public static CharacterCreationManager characterCreationManager;
    public void Awake()
    {
        ChooseCharacter();

        if (characterCreationManager != null)
            throw new System.Exception("Character Creation Manager has MORE THAN ONE INSTANCES!");
        characterCreationManager = this;

    }
    #endregion

    public void ChooseCharacter()
    {
        foreach(SelectCharacter sl in characters)
        {
            if (sl.race == currRace && currCharacterClass == sl.CharacterClass)
            {
                sl.ResetRotation();
                sl.gameObject.SetActive(true);
                currCharacter = sl;
            }
            else sl.gameObject.SetActive(false);

        }
    }

    public void Rotate(bool LeftSide)
    {
        if (LeftSide)
            currCharacter.RotateObjectLeft();
        else currCharacter.RotateObjectRight();
    }

    public void StopRotation()
    {
        currCharacter.DoNotRotate();
    }

    public void SetClass(int CLASS)
    {
        currCharacterClass = (Class)CLASS;
    }

    public void SetRace(int RACE)
    {
        currRace = (Race)RACE;

    }
}
