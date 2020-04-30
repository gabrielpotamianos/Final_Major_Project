using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterToCreate : MonoBehaviour
{
    bool rotateLeft = false;
    bool rotateRight = false;
    public float RotationSpeed = 2;
    public CharacterInfo info;

    void Update()
    {
        if (rotateLeft)
            transform.Rotate(new Vector3(0, RotationSpeed*Time.deltaTime, 0));
        else if (rotateRight)
            transform.Rotate(new Vector3(0, -RotationSpeed*Time.deltaTime, 0));
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
