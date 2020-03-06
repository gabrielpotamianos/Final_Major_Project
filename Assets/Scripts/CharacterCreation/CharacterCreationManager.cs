using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCreationManager : MonoBehaviour
{
    public CharacterInSelection[] characters;

    public CharacterInSelection.Race currRace = CharacterInSelection.Race.Male;
    public CharacterInSelection.Class currCharacterClass = CharacterInSelection.Class.Mage;
    CharacterInSelection currCharacter;

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
        foreach (CharacterInSelection sl in characters)
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
        currCharacterClass = (CharacterInSelection.Class)CLASS;
    }

    public void SetRace(int RACE)
    {
        currRace = (CharacterInSelection.Race)RACE;

    }
}
