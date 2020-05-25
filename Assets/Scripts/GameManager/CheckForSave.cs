
using UnityEngine;
using UnityEngine.UI;
public class CheckForSave : MonoBehaviour
{


    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        gameObject.GetComponent<Button>().interactable = SaveSystem.HasSave();
    }


    public void BackToMenuIfNoSave()
    {
        var SceneManagement=GetComponent<SceneManagement>();

        if(SaveSystem.HasSave())
            SceneManagement.LoadScene("CharacterSelection");
        else SceneManagement.LoadScene("MainMenu");
    }
}
