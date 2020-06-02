using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CharacterCreationManager : MonoBehaviour
{
    //Variable to load all saved characters
    public CharacterToCreate[] characters;

    //The selected information about the character
    public CharacterInfo currCharacterInfo;

    //Character selected - GameObject
    CharacterToCreate currCharacter;

    //Button Links for Button Pressed Feature
    Button buttonRace;
    Button buttonBreed;

    //Singleton
    public static CharacterCreationManager characterCreationManager;


    private void Awake()
    {

        //Check Singleton
        if (characterCreationManager != null)
            throw new System.Exception("Character Creation Manager has MORE THAN ONE INSTANCES!");
        characterCreationManager = this;

        currCharacterInfo = new CharacterInfo();

        //Gather Character
        ChooseCharacter();


    }


    private void Start()
    {
        if (SaveSystem.MaximumCharactersReached())
            MessageManager.instance.DisplayMessage(Constants.CHARACTER_CREATION_MAX_CHAR_MSG, Constants.CHARACTER_CREATION_MAX_CHAR_TIME);
    }


    public void ChooseCharacter()
    {
        //Iterate though all characters and find the matching one
        foreach (CharacterToCreate character in characters)
        {
            if (character.info.race == currCharacterInfo.race && character.info.breed == currCharacterInfo.breed)
            {
                character.ResetRotation();
                character.gameObject.SetActive(true);
                currCharacter = character;
            }
            else character.gameObject.SetActive(false);

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
        //Take the last button pressed and make it usable
        if (buttonBreed) buttonBreed.interactable = true;

        //Get the new button pressed
        buttonBreed = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        
        //Set the character breed based on integers
        currCharacterInfo.breed = (CharacterInfo.Breed)BREED;
        
        //Disable button ( for pressed effect )
        buttonBreed.interactable = false;
    }

    public void SetRace(int RACE)
    {
        //Take the last button pressed and make it usable
        if (buttonRace) buttonRace.interactable = true;

        //get the new button pressed
        buttonRace = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        
        //Set the character raece based on integers
        currCharacterInfo.race = (CharacterInfo.Race)RACE;
        
        //Disable button ( for pressed effect )
        buttonRace.interactable = false;

    }

    public void SaveCharacteR()
    {
        SaveSystem.SaveNewCharacterCreated(currCharacterInfo);
    }
}
