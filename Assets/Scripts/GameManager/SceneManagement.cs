using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneManagement : MonoBehaviour
{
    [Range(0,1)]
    public float FadeInSpeed;
    public GameObject FadeInPanel;
    public Slider slider;


    public void LoadScene(string name)
    {
        StartCoroutine(FadeInScene(name));
    }

    IEnumerator FadeInScene(string name)
    {
        while(FadeInPanel.GetComponent<Image>().color.a<1)
        {
            Color temporary =FadeInPanel.GetComponent<Image>().color;
            temporary.a+= 0.1f * FadeInSpeed;
            FadeInPanel.GetComponent<Image>().color = temporary;
            yield return null;
        }

        yield return new WaitForSeconds(0.4f);
        StartCoroutine(LoadSceneIE(name));
    }


    IEnumerator LoadSceneIE(string name)
    {
        slider.gameObject.SetActive(true);
        AsyncOperation async = SceneManager.LoadSceneAsync(name);
        while(!async.isDone)
        {
            float progress = Mathf.Clamp01(async.progress / .9f);
            slider.value = progress;
            yield return null;
        }
    }
}
