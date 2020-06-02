using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class SceneInit : MonoBehaviour
{
    public static SceneInit instance;
    public GameObject player;
    bool SaveLoaded = false;


    void Awake()
    {
        if (instance == null)
            instance = this;
        else Debug.LogError(Constants.SCENE_INIT_SINGLETON);

        //Disable kinematic because of the Character Selection Scene where the characters must stand still
        CharacterSelection.ChosenCharacter.Character.GetComponent<Rigidbody>().isKinematic = false;

        //Enable character
        CharacterSelection.ChosenCharacter.Character.GetComponent<PlayerData>().enabled = true;
        CharacterSelection.ChosenCharacter.Character.GetComponent<PlayerMovement>().enabled = true;

        //Enable combat based on the breed
        switch (CharacterSelection.ChosenCharacter.breed)
        {
            case CharacterInfo.Breed.Mage:
                CharacterSelection.ChosenCharacter.Character.GetComponent<MageCombatSystem>().enabled = true;
                Destroy(CharacterSelection.ChosenCharacter.Character.GetComponent<RogueCombatSystem>());
                Destroy(CharacterSelection.ChosenCharacter.Character.GetComponent<WarriorCombatSystem>());
                break;
            case CharacterInfo.Breed.Warrior:
                CharacterSelection.ChosenCharacter.Character.GetComponent<WarriorCombatSystem>().enabled = true;
                Destroy(CharacterSelection.ChosenCharacter.Character.GetComponent<RogueCombatSystem>());
                Destroy(CharacterSelection.ChosenCharacter.Character.GetComponent<MageCombatSystem>());
                break;
            case CharacterInfo.Breed.Rogue:
                CharacterSelection.ChosenCharacter.Character.GetComponent<RogueCombatSystem>().enabled = true;
                Destroy(CharacterSelection.ChosenCharacter.Character.GetComponent<WarriorCombatSystem>());
                Destroy(CharacterSelection.ChosenCharacter.Character.GetComponent<MageCombatSystem>());
                break;
        }
    }

    void Update()
    {
        //The load is done here because scripts do not get fully initialized in Start Function
        if (!SaveLoaded)
        {
            if (PlayerData.instance && PlayerInventory.instance)
                LoadSave();
            SaveLoaded = true;
        }

    }


    private void LoadSave()
    {
        PlayerData.instance.transform.position = CharacterSelection.ChosenCharacter.Position;
        PlayerData.instance.transform.eulerAngles = CharacterSelection.ChosenCharacter.Rotation;
        PlayerData.instance.Name = CharacterSelection.ChosenCharacter.name;
        PlayerData.instance.gold = CharacterSelection.ChosenCharacter.Gold;
        PlayerInventory.instance.AddItem(CharacterSelection.ChosenCharacter.items, CharacterSelection.ChosenCharacter.itemsQuantities, CharacterSelection.ChosenCharacter.slots);
        CameraMovement.Instance.transform.position = CharacterSelection.ChosenCharacter.CameraPosition;
        CameraMovement.Instance.transform.eulerAngles = CharacterSelection.ChosenCharacter.CameraRotation;
        CameraMovement.Instance.SetRotation(CharacterSelection.ChosenCharacter.CameraRotation.x, CharacterSelection.ChosenCharacter.CameraRotation.y);
    }
}
