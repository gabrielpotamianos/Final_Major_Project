using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
public class InGameMenu : MonoBehaviour
{
    public GameObject MenuPanel;
    CanvasGroup canvas;
    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        MenuPanel = GameObject.Find("MenuPanel");
        canvas = MenuPanel.GetComponent<CanvasGroup>();
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            ToogleMenu();
    }

    public void ToogleMenu()
    {
        canvas.alpha = canvas.alpha == 1 ? 0 : 1;
        canvas.blocksRaycasts = canvas.alpha == 1;
        canvas.interactable = canvas.alpha == 1;
        Time.timeScale = canvas.alpha == 1 ? 0 : 1;
    }

    public void ResumeGame()
    {
        canvas.alpha = 0;
        canvas.blocksRaycasts = false;
        canvas.interactable = false;
        Time.timeScale = 1.0f;
    }

    public void QuitApplication()
    {
        Application.Quit();
    }

    public void CompleteAction()
    {
        Time.timeScale = 1;
        canvas.alpha = 0;
        canvas.blocksRaycasts = false;
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
        CharacterSelection.ChosenCharacter.CameraRotation = new Vector3(0,PlayerData.instance.transform.rotation.eulerAngles.y,0);

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
