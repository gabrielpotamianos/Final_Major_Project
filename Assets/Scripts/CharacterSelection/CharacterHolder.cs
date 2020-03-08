using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterHolder : MonoBehaviour
{
    public Button CreateCharacterButton;
    public Button DeleteCharacterButton;

    public Vector3 Position;
    public GameObject MalePrefab;
    public GameObject FemalePrefab;
    public List<CharacterInfo> allCharacters;

    public GameObject[] CharactersUI;

    CharacterInfo CurrCharInfo;
    public int minLength;

    private void Awake()
    {
        InitScene();
    }

    public void InitScene()
    {
        allCharacters = SaveSystem.LoadAllCharacters();

        minLength = Mathf.Min(allCharacters.Count, CharactersUI.Length);


        for (int i = 0; i < minLength; i++)
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

            if (CurrCharInfo == null)
                CurrCharInfo = allCharacters[0];
        }
        SelectCharacter();
    }

    public void SelectCharacter()
    {
        for (int i = 0; i < minLength; i++)
            allCharacters[i].Character.SetActive(allCharacters[i] == CurrCharInfo);
    }

    public void UpdateCurrentSelectedCharacter(GameObject go)
    {
        for (int i = 0; i < minLength; i++)
            if (CharactersUI[i] == go)
                CurrCharInfo = allCharacters[i];
        SelectCharacter();

    }


    private void Update()
    {
        if (allCharacters.Count <= 0)
        {
            CreateCharacterButton.transform.parent.gameObject.SetActive(true);
            DeleteCharacterButton.transform.parent.gameObject.SetActive(false);
        }
        else if (allCharacters.Count >= Constants.MAXIMUM_CHARACTERS)
        {
            CreateCharacterButton.transform.parent.gameObject.SetActive(false);
            DeleteCharacterButton.transform.parent.gameObject.SetActive(true);
        }

        CurrCharInfo.Character.tag = CurrCharInfo.breed.ToString();
        CurrCharInfo.Character.layer = LayerMask.NameToLayer("Player");
    }

    private void DisplayCharacterInfo(GameObject character, CharacterInfo info)
    {
        character.GetComponent<CanvasGroup>().alpha = 1;
        character.transform.GetChild(0).GetComponent<Text>().text = info.race.ToString();
        character.transform.GetChild(1).GetComponent<Text>().text = info.breed.ToString();

    }

    public void DeleteCharacter()
    {
        Destroy(CurrCharInfo.Character);
        allCharacters.Remove(CurrCharInfo);
        minLength = Mathf.Min(allCharacters.Count, CharactersUI.Length);

        for (int i = 0; i < CharactersUI.Length; i++)
        {
            CharactersUI[i].GetComponent<CanvasGroup>().alpha = 0;
            if (i < minLength)
            {
                DisplayCharacterInfo(CharactersUI[i], allCharacters[i]);
                CurrCharInfo = allCharacters[0];

            }
        }

        SelectCharacter();
        SaveSystem.SaveAllCharactersOverwrite(allCharacters);
    }


    public void DoNotDestroy()
    {
        DontDestroyOnLoad(CurrCharInfo.Character);
    }

}
