using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCreationManager : MonoBehaviour
{

    public CharacterToCreate[] characters;

    public CharacterInfo currCharacterInfo;
    CharacterToCreate currCharacter;

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


    private void Start()
    {
        if (SaveSystem.MaximumCharactersReached())
            MessageManager.instance.DisplayMessage(Constants.MAXIMUM_CHARACTERS_MESSAGE, 10);

    }


    public void ChooseCharacter()
    {
        foreach (CharacterToCreate sl in characters)
        {
            if (sl.info.race == currCharacterInfo.race && sl.info.breed == currCharacterInfo.breed)
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

    public void SetClass(int BREED)
    {
        currCharacterInfo.breed = (CharacterInfo.Breed)BREED;
    }

    public void SetRace(int RACE)
    {
        currCharacterInfo.race = (CharacterInfo.Race)RACE;

    }
    
    public void SaveCharacteR()
    {
        SaveSystem.SaveNewCharacterCreated(currCharacterInfo);
    }
}
