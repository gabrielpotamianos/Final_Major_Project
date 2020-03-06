using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectCharacter : BaseClassCharacterCreation
{
    static public string SelectedGameObject;
    bool rotateLeft = false;
    bool rotateRight = false;
    public float RotationSpeed = 2;


    void Update()
    {
        SelectedGameObject = gameObject.tag;
        if (rotateLeft)
            transform.Rotate(new Vector3(0, RotationSpeed, 0));
        else if (rotateRight)
            transform.Rotate(new Vector3(0, -RotationSpeed, 0));
    }

    public void RotateObjectLeft()
    {
        rotateLeft = true;
        rotateRight = false;
    }

    public void RotateObjectRight()
    {
        rotateLeft = false;
        rotateRight = true;
    }

    public void DoNotRotate()
    {
        rotateLeft = false;
        rotateRight = false;
    }

    public void ResetRotation()
    {
        gameObject.transform.eulerAngles = Vector3.zero;
    }

}
