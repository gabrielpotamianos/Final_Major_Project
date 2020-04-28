using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Minimap : MonoBehaviour
{
    public float MinOrtographicSize;
    public float MaxOrtographicSize;
    public float AmoutToZoomBy;
    float previousShadowDistance;

    GameObject MinimapPointer;
    GameObject player;
    Camera CurrCamera;
    Button Zoom_In_Button;
    Button Zoom_Out_Button;

    void Start()
    {
        player = GameObject.FindWithTag(CharacterSelection.ChosenCharacter.breed.ToString());
        MinimapPointer = GameObject.Find("Minimap-Pointer-Player");
        CurrCamera = GetComponent<Camera>();
        }


    void Update()
    {
        transform.position = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
        MinimapPointer.transform.rotation = Quaternion.Euler(0, 0, -player.transform.eulerAngles.y);
        if(Zoom_In_Button)
            Zoom_In_Button.interactable=CurrCamera.orthographicSize>MinOrtographicSize;
        if(Zoom_Out_Button)
            Zoom_Out_Button.interactable=CurrCamera.orthographicSize<MaxOrtographicSize;
    }

    void OnPreRender()
    {
        previousShadowDistance = QualitySettings.shadowDistance;
        QualitySettings.shadowDistance = 0;
    }
    void OnPostRender()
    {
        QualitySettings.shadowDistance = previousShadowDistance;
    }


    public void ZoomIn()
    {
        if (!Zoom_In_Button) Zoom_In_Button = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        CurrCamera.orthographicSize -= (CurrCamera.orthographicSize - AmoutToZoomBy >= MinOrtographicSize) ? AmoutToZoomBy : (CurrCamera.orthographicSize - AmoutToZoomBy);
    }
    public void ZoomOut()
    {
        if (!Zoom_Out_Button) Zoom_Out_Button = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        CurrCamera.orthographicSize += (CurrCamera.orthographicSize + AmoutToZoomBy <= MaxOrtographicSize) ? AmoutToZoomBy : (MaxOrtographicSize - CurrCamera.orthographicSize);
    }
}
