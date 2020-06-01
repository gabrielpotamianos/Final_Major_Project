using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.EventSystems;

public class InGameMenu : MonoBehaviour
{
    public GameObject MenuPanel;
    public GameObject SettingsMenu;
    CanvasGroup MainMenuCanvas;
    CanvasGroup SettingsMenuCanvas;
    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        MenuPanel = GameObject.Find("MenuPanel");
        SettingsMenu = GameObject.Find("SettingsPanel");
        MainMenuCanvas = MenuPanel.GetComponent<CanvasGroup>();
        SettingsMenuCanvas = SettingsMenu.GetComponent<CanvasGroup>();
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && SettingsMenuCanvas.alpha != 1)
            ToogleMenu();

        if (Input.GetMouseButton(0))
        {
            PointerEventData ped = new PointerEventData(null);

            //Set required parameters, in this case, mouse position
            ped.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();

            //Raycast it
            EventSystem.current.RaycastAll(ped, results);

            foreach(var a in results)
                print(a.gameObject.name);
        }
    }

    public void ToogleMenu()
    {
        MainMenuCanvas.alpha = MainMenuCanvas.alpha == 1 ? 0 : 1;
        MainMenuCanvas.blocksRaycasts = MainMenuCanvas.alpha == 1;
        MainMenuCanvas.interactable = MainMenuCanvas.alpha == 1;
        Time.timeScale = MainMenuCanvas.alpha == 1 ? 0 : 1;
    }
    public void ToogleMenu(bool value)
    {
        MainMenuCanvas.alpha = value == true ? 1 : 0;
        MainMenuCanvas.blocksRaycasts = value;
        MainMenuCanvas.interactable = value;
    }

    public void ResumeGame()
    {
        MainMenuCanvas.alpha = 0;
        MainMenuCanvas.blocksRaycasts = false;
        MainMenuCanvas.interactable = false;
        Time.timeScale = 1.0f;
    }

    public void OpenSettingsMenu()
    {
        ToogleMenu(false);
        SettingsMenuCanvas.alpha = 1;
        SettingsMenuCanvas.blocksRaycasts = true;
        SettingsMenuCanvas.interactable = true;
    }
    public void CloseSettingsMenu()
    {
        SettingsMenuCanvas.alpha = 0;
        SettingsMenuCanvas.blocksRaycasts = false;
        SettingsMenuCanvas.interactable = false;
        ToogleMenu(true);
    }


    public void QuitApplication()
    {
        Application.Quit();
    }

    public void CompleteAction()
    {
        Time.timeScale = 1;
        MainMenuCanvas.alpha = 0;
        MainMenuCanvas.blocksRaycasts = false;
    }

    public void SaveCharacter()
    {
        CharacterSelection.ChosenCharacter.name = PlayerData.instance.Name;
        CharacterSelection.ChosenCharacter.items = PlayerInventory.instance.GetItems().Keys.ToList();
        CharacterSelection.ChosenCharacter.itemsQuantities = PlayerInventory.instance.GetItems().Values.ToList();
        CharacterSelection.ChosenCharacter.Position = PlayerData.instance.transform.position;
        CharacterSelection.ChosenCharacter.Rotation = PlayerData.instance.transform.eulerAngles;
        CharacterSelection.ChosenCharacter.Gold = PlayerData.instance.gold;
        CharacterSelection.ChosenCharacter.slots = PlayerInventory.instance.GetAllOccupiedSlots();
        CharacterSelection.ChosenCharacter.CameraPosition = CameraMovement.Instance.transform.position;
        CharacterSelection.ChosenCharacter.CameraRotation = new Vector3(0, PlayerData.instance.transform.rotation.eulerAngles.y, 0);

        SaveSystem.SaveCharacterOverwrite(CharacterSelection.ChosenCharacter);
    }

    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        var a = CharacterSelection.ChosenCharacter.Character.scene.GetRootGameObjects();
        if (a.Length > 0)
            foreach (GameObject b in a)
                Destroy(b);


    }
}
