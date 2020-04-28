using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSelection : MonoBehaviour
{
    public static CharacterInfo ChosenCharacter;
    public Button CreateCharacterButton;
    public Button DeleteCharacterButton;

    public Vector3 Position;
    public GameObject MalePrefab;
    public GameObject FemalePrefab;
    public List<CharacterInfo> allCharacters;

    public GameObject[] CharacterInfoUIButtons;

    CharacterInfo SelectedCharacter;
    int MinArrayLength;

    private void Awake()
    {
        InitScene();
    }

    public void InitScene()
    {
        allCharacters = SaveSystem.LoadAllCharacters();
        print(allCharacters[0].breed);

        MinArrayLength = Mathf.Min(allCharacters.Count, CharacterInfoUIButtons.Length);


        for (int i = 0; i < MinArrayLength; i++)
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
            DisplayCharacterInfo(CharacterInfoUIButtons[i], allCharacters[i]);

            if (SelectedCharacter == null)
                SelectedCharacter = allCharacters[0];
        }
        DisplaySelectedCharacter();
    }

    public void DisplaySelectedCharacter()
    {
        for (int i = 0; i < MinArrayLength; i++)
            allCharacters[i].Character.SetActive(allCharacters[i] == SelectedCharacter);
    }

    public void UpdateSelectedCharacter(GameObject go)
    {
        for (int i = 0; i < MinArrayLength; i++)
            if (CharacterInfoUIButtons[i] == go)
                SelectedCharacter = allCharacters[i];
        DisplaySelectedCharacter();

    }


    private void Update()
    {
        DisplayUIButtons();

        if (allCharacters.Count > 0)
        {
            SelectedCharacter.Character.tag = SelectedCharacter.breed.ToString();
            SelectedCharacter.Character.layer = LayerMask.NameToLayer("Player");
            ChosenCharacter = SelectedCharacter;
        }
    }

    private void DisplayUIButtons()
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
        else
        {
            CreateCharacterButton.transform.parent.gameObject.SetActive(true);
            DeleteCharacterButton.transform.parent.gameObject.SetActive(true);
        }

    }

    private void DisplayCharacterInfo(GameObject character, CharacterInfo info)
    {
        character.GetComponent<CanvasGroup>().alpha = 1;
        character.transform.GetChild(0).GetComponent<Text>().text = info.race.ToString();
        character.transform.GetChild(1).GetComponent<Text>().text = info.breed.ToString();

    }

    public void DeleteCharacter()
    {
        Destroy(SelectedCharacter.Character);
        allCharacters.Remove(SelectedCharacter);
        MinArrayLength = Mathf.Min(allCharacters.Count, CharacterInfoUIButtons.Length);

        for (int i = 0; i < CharacterInfoUIButtons.Length; i++)
        {
            CharacterInfoUIButtons[i].GetComponent<CanvasGroup>().alpha = 0;
            if (i < MinArrayLength)
            {
                DisplayCharacterInfo(CharacterInfoUIButtons[i], allCharacters[i]);
                SelectedCharacter = allCharacters[0];

            }
        }

        DisplaySelectedCharacter();
        SaveSystem.SaveAllCharactersOverwrite(allCharacters);
    }


    public void DoNotDestroy()
    {
        DontDestroyOnLoad(SelectedCharacter.Character);
    }

}
