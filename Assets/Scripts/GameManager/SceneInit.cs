using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class SceneInit : MonoBehaviour
{
    public GameObject player;

    public static SceneInit instance;
    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        if(instance==null)
            instance=this;
        else Debug.LogError("many instances going on");
        player = GameObject.Find("Warrior (2)");

        CharacterSelection.ChosenCharacter = new CharacterInfo();
        CharacterSelection.ChosenCharacter.breed = (CharacterInfo.Breed)Enum.Parse(typeof(CharacterInfo.Breed), player.tag);


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

}
