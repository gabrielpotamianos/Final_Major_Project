using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class SceneInit : MonoBehaviour
{
    public GameObject player;

    bool SaveLoaded = false;

    public static SceneInit instance;
    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        if (instance == null)
            instance = this;
        else Debug.LogError("many instances going on");
        // player = GameObject.Find("Warrior (2)");

        // CharacterSelection.ChosenCharacter = new CharacterInfo();
        // CharacterSelection.ChosenCharacter.breed = (CharacterInfo.Breed)Enum.Parse(typeof(CharacterInfo.Breed), player.tag);
        // CharacterSelection.ChosenCharacter.Character=GameObject.FindGameObjectWithTag(CharacterSelection.ChosenCharacter.breed.ToString());
        CharacterSelection.ChosenCharacter.Character.GetComponent<Rigidbody>().isKinematic = false;
        GameObject.FindGameObjectWithTag(CharacterSelection.ChosenCharacter.breed.ToString()).GetComponent<PlayerData>().enabled = true;

        GameObject.FindGameObjectWithTag(CharacterSelection.ChosenCharacter.breed.ToString()).GetComponent<PlayerMovement>().enabled = true;
        switch (CharacterSelection.ChosenCharacter.breed)
        {
            case CharacterInfo.Breed.Mage:
                GameObject.FindObjectOfType<MageCombatSystem>().enabled = true;
                Destroy(GameObject.FindObjectOfType<RogueCombatSystem>());
                Destroy(GameObject.FindObjectOfType<WarriorCombatSystem>());
                break;
            case CharacterInfo.Breed.Warrior:
                GameObject.FindObjectOfType<WarriorCombatSystem>().enabled = true;
                Destroy(GameObject.FindObjectOfType<RogueCombatSystem>());
                Destroy(GameObject.FindObjectOfType<MageCombatSystem>());
                break;
            case CharacterInfo.Breed.Rogue:
                GameObject.FindObjectOfType<RogueCombatSystem>().enabled = true;
                Destroy(GameObject.FindObjectOfType<WarriorCombatSystem>());
                Destroy(GameObject.FindObjectOfType<MageCombatSystem>());
                break;

        }


    }

    void Update()
    {
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
        CameraMovement.Instance.SetRotation(CharacterSelection.ChosenCharacter.CameraRotation.x,CharacterSelection.ChosenCharacter.CameraRotation.y);
    }
}
