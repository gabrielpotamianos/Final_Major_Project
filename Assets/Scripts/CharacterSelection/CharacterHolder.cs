using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine.UI;

public class CharacterHolder : MonoBehaviour
{


    public Vector3 Position;
    public GameObject MalePrefab;
    public GameObject FemalePrefab;
    public List<CharacterInfo> allCharacters;

    public GameObject[] CharactersUI;

    CharacterInfo CurrCharInfo;

    private void Awake()
    {
        allCharacters = SaveSystem.LoadAllCharacters();

        for (int i = 0; i < CharactersUI.Length; i++)
        {
            switch (allCharacters[i].race)
            {
                case CharacterInfo.Race.Female:
                        allCharacters[i].Character = Instantiate(FemalePrefab, Position, Quaternion.identity);
                        break;
                case CharacterInfo.Race.Male:
                        allCharacters[i].Character = Instantiate(MalePrefab, Position, Quaternion.identity);
                        break;
                default:
                    break;
            }
            DisplayCharacterInfo(CharactersUI[i], allCharacters[i]);

            if(CurrCharInfo==null)
                CurrCharInfo = allCharacters[0];
        }
        SelectCharacter();

    }

    public void SelectCharacter()
    {
        for(int i=0;i< CharactersUI.Length; i++)
            allCharacters[i].Character.SetActive(allCharacters[i] == CurrCharInfo);
    }

    public void UpdateCurrentSelectedCharacter(GameObject go)
    {
        for (int i = 0; i < CharactersUI.Length; i++)
            if (CharactersUI[i] == go)
                CurrCharInfo = allCharacters[i];
        SelectCharacter();

    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene("CharacterCreation");
    }

    private void DisplayCharacterInfo(GameObject character, CharacterInfo info)
    {
        character.GetComponent<CanvasGroup>().alpha = 1;
        character.transform.GetChild(0).GetComponent<Text>().text = info.race.ToString();
        character.transform.GetChild(1).GetComponent<Text>().text = info.breed.ToString();

    }
}
