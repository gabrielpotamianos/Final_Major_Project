using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
public class ButtonPressed : MonoBehaviour
{
    public float ShiftValue;
    Button button;
    Vector3 initPos;
    bool buttonPressed = true;

    void Start()
    {
        button = GetComponentInParent<Button>();
        initPos = transform.position;
    }

    void Update()
    {
        if (!buttonPressed)
        {
            if (!button.interactable)
                transform.position = new Vector3(initPos.x, initPos.y - ShiftValue, initPos.z);
            else transform.position = new Vector3(initPos.x, initPos.y, initPos.z);
        }

    }

    public void ButtonPress()
    {
        buttonPressed = true;
        transform.position = new Vector3(initPos.x, initPos.y - ShiftValue, initPos.z);
    }

    public void ButtonRelease()
    {
        transform.position = new Vector3(initPos.x, initPos.y, initPos.z);
        buttonPressed = false;
    }
}
