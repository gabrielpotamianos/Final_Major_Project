using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Map : MonoBehaviour
{
    protected Camera currCamera;
    protected GameObject PlayerGameObject;
    protected GameObject PlayerPointer;
    protected float previousShadowDistance;

    private GameObject MapGameObject;
    private RectTransform CanvasRect;

    public virtual void Start()
    {
        PlayerGameObject = CharacterSelection.ChosenCharacter.Character;
        currCamera = GetComponent<Camera>();
        MapGameObject = GameObject.Find("WholeMap");
        PlayerPointer = GameObject.Find("Map-Pointer-Player");
        CanvasRect = MapGameObject.transform.GetComponent<RectTransform>();
        currCamera.enabled = false;
        MapGameObject.SetActive(false);

    }

    public virtual void Update()
    {

        Vector2 ViewportPosition = currCamera.WorldToViewportPoint(PlayerGameObject.transform.position);
        Vector2 WorldObject_ScreenPosition = new Vector2(
        ((ViewportPosition.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x / 2.0f)),
        ((ViewportPosition.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y / 2.0f)));


        PlayerPointer.GetComponent<RectTransform>().anchoredPosition = WorldObject_ScreenPosition;
        PlayerPointer.transform.rotation = Quaternion.Euler(0, 0, -PlayerGameObject.transform.eulerAngles.y);

        if (Input.GetKeyDown(KeyCode.M))
        {
            currCamera.enabled = true;
            Target.instance.enabled = MapGameObject.activeSelf;
            MapGameObject.SetActive(MapGameObject.activeSelf ? false : true);
        }
        else if (!Input.GetKeyDown(KeyCode.M))
            currCamera.enabled = false;


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

}