using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CharacterCreationManager : MonoBehaviour
{

    public CharacterToCreate[] characters;

    public CharacterInfo currCharacterInfo;
    CharacterToCreate currCharacter;
    Button buttonRace;
    Button buttonBreed;

    #region Singleton
    public static CharacterCreationManager characterCreationManager;
    public void Awake()
    {
        currCharacterInfo = new CharacterInfo();
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
        if (buttonBreed) buttonBreed.interactable = true;
        buttonBreed = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        currCharacterInfo.breed = (CharacterInfo.Breed)BREED;
        buttonBreed.interactable = false;
    }

    public void SetRace(int RACE)
    {
        if (buttonRace) buttonRace.interactable = true;
        buttonRace = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        currCharacterInfo.race = (CharacterInfo.Race)RACE;
        buttonRace.interactable = false;

    }

    public void SaveCharacteR()
    {
        SaveSystem.SaveNewCharacterCreated(currCharacterInfo);
    }
}
