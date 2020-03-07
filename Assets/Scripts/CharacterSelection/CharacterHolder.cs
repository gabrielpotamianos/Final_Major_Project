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
    CharacterInfo CurrCharInfo;
    public List<CharacterInfo> allCharacters;

    public GameObject[] CharactersUI;


    private void Awake()
    {
        allCharacters = SaveSystem.LoadAllCharacters();

        for (int i = 0; i < allCharacters.Count; i++)
        {
            switch (allCharacters[i].race)
            {

                case CharacterInfo.Race.Female:
                    {
                        allCharacters[i].Character = Instantiate(FemalePrefab, Position, Quaternion.identity);
                        break;
                    }
                case CharacterInfo.Race.Male:
                    {
                        allCharacters[i].Character = Instantiate(MalePrefab, Position, Quaternion.identity);
                        break;
                    }
                default:
                    break;
            }
            DisplayCharacterInfo(CharactersUI[i], allCharacters[i]);
            CurrCharInfo = allCharacters[0];

        }
        ChooseCharacter();

    }

    public void ChooseCharacter()
    {
        foreach (CharacterInfo charInfo in allCharacters)
        {
            charInfo.Character.SetActive(charInfo == CurrCharInfo);
            print(CurrCharInfo.Character.name);
        }
    }

    public void UpdateCurrentSelectedCharacter(GameObject go)
    {
        foreach (CharacterInfo CharInfo in allCharacters)
        {
            if (CharInfo.Character == go)
            {
                CurrCharInfo = CharInfo;
                break;
            }
        }
        ChooseCharacter();

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


//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.SceneManagement;
//using UnityEditor.SceneManagement;

//public class CharacterHolder : MonoBehaviour
//{
//    public Vector3 Position;
//    public GameObject MalePrefab;
//    public GameObject FemalePrefab;
//    // CharacterInSelection.Race currRace;
//    private void Awake()
//    {
//        // currRace= SaveSystem.LoadNewCharacter();
//        List<CharacterInSelection.Race> races = SaveSystem.LoadAllCharacters();
//        for (int i = 0; i < races.Count; i++)
//        {
//            switch (races[i])
//            {
//                case CharacterInSelection.Race.Female:
//                    Instantiate(FemalePrefab, Position + new Vector3(i, 0, 0), Quaternion.identity);
//                    break;
//                case CharacterInSelection.Race.Male:
//                    Instantiate(MalePrefab, Position + new Vector3(i, 0, 0), Quaternion.identity);
//                    break;
//                default:
//                    break;
//            }
//        }
//    }


//    // Start is called before the first frame update
//    void Start()
//    {
//    }

//    // Update is called once per frame
//    void Update()
//    {

//        if (Input.GetKeyDown(KeyCode.R))
//            SceneManager.LoadScene("CharacterCreation");
//    }
//}
