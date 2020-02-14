using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIElementTo3DObj : MonoBehaviour
{

    public Transform Target;
    public bool attachedToObj;

    // Update is called once per frame
    void Update()
    {
        HideEmptyBar();
        ShowEmptyBar();
    }

    private void LateUpdate()
    {
        if (attachedToObj)
        {
            Vector3 screenPoint = Camera.main.WorldToViewportPoint(Target.transform.position);

            bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
            if (onScreen)
            {
                ToogleUIObjVisibility(true);
                gameObject.transform.position = Camera.main.WorldToScreenPoint(Target.transform.position);
            }
            else ToogleUIObjVisibility(false);
        }
    }

    private void ToogleUIObjVisibility(bool Visible)
    {
        GetComponent<CanvasGroup>().alpha = Visible ? 1 : 0;

    }

    public void HideEmptyBar()
    {
        if (GetComponent<Slider>().value <= 0)
            transform.GetChild(1).gameObject.SetActive(false);
    }

    public void ShowEmptyBar()
    {
        if (GetComponent<Slider>().value > 0)
            transform.GetChild(1).gameObject.SetActive(true);

    }
}
