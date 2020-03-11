using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageManager : MonoBehaviour
{
    Text message;
    GameObject MessagePanel;
    IEnumerator MessageClearer;



    #region Singleton
    static public MessageManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("You have MORE than ONE MESSAGE MANAGER instances!");
            return;
        }
        instance = this;



        message = GameObject.Find(Constants.MESSAGE_PANEL).transform.GetChild(0).GetComponent<Text>();
        MessagePanel = GameObject.Find(Constants.MESSAGE_PANEL).gameObject;
        MessageClearer = ClearMessage();
        MessagePanel.SetActive(false);

    }
    #endregion


    public void DisplayMessage(string Message)
    {
        StopCoroutine(MessageClearer);

        MessagePanel.SetActive(true);
        message.text = Message;

        MessageClearer = ClearMessage();
        StartCoroutine(MessageClearer);

    }
    public void DisplayMessage(string Message, float time)
    {
        StopCoroutine(MessageClearer);

        MessagePanel.SetActive(true);
        message.text = Message;

        MessageClearer = ClearMessage(time);
        StartCoroutine(MessageClearer);
    }

    public void KillMessage()
    {
        StopCoroutine(MessageClearer);
        MessageClearer = ClearMessage(0);
        StartCoroutine(MessageClearer);

    }

    IEnumerator ClearMessage()
    {
        yield return new WaitForSeconds(2);
        message.text = "";
        MessagePanel.SetActive(false);
    }

    IEnumerator ClearMessage(float time)
    {
        yield return new WaitForSeconds(time);
        message.text = "";
        MessagePanel.SetActive(false);
    }


}
