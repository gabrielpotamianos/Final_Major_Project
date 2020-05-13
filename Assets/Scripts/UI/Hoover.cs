using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class Hoover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    GameObject tooltip;
    Text ToolTipText;
    

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        tooltip=GameObject.FindGameObjectWithTag("ToolTip");
        ToolTipText=tooltip.GetComponentInChildren<Text>();
    }




    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        tooltip.SetActive(true);
        ToolTipText.text+="I got here";
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        tooltip.SetActive(false);
        ToolTipText.text+= Environment.NewLine;
    }
}
