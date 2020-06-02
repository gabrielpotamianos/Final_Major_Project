using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Minimap : Map
{
    public float MinOrtographicSize;
    public float MaxOrtographicSize;
    public float AmoutToZoomBy;

    Button Zoom_In_Button;
    Button Zoom_Out_Button;

    public override void Start()
    {
        PlayerGameObject = CharacterSelection.ChosenCharacter.Character;
        PlayerPointer = GameObject.Find("Minimap-Pointer-Player");
        currCamera = GetComponent<Camera>();

    }


    public override void Update()
    {
        transform.position = new Vector3(PlayerGameObject.transform.position.x, transform.position.y, PlayerGameObject.transform.position.z);
        PlayerPointer.transform.rotation = Quaternion.Euler(0, 0, -PlayerGameObject.transform.eulerAngles.y);
        if (Zoom_In_Button)
            Zoom_In_Button.interactable = currCamera.orthographicSize > MinOrtographicSize;
        if (Zoom_Out_Button)
            Zoom_Out_Button.interactable = currCamera.orthographicSize < MaxOrtographicSize;
    }

    public void ZoomIn()
    {
        if (!Zoom_In_Button) Zoom_In_Button = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        currCamera.orthographicSize -= (currCamera.orthographicSize - AmoutToZoomBy >= MinOrtographicSize) ? AmoutToZoomBy : (currCamera.orthographicSize - AmoutToZoomBy);
    }
    public void ZoomOut()
    {
        if (!Zoom_Out_Button) Zoom_Out_Button = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        currCamera.orthographicSize += (currCamera.orthographicSize + AmoutToZoomBy <= MaxOrtographicSize) ? AmoutToZoomBy : (MaxOrtographicSize - currCamera.orthographicSize);
    }
}
