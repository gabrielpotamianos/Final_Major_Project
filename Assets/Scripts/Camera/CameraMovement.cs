using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//C#
using System.Runtime.InteropServices;
// 

public class CameraMovement : MonoBehaviour
{
    [DllImport("user32.dll")]
    static extern bool SetCursorPos(int X, int Y);

    public Transform target;
    public Slider AnghelSlider;

    float mouseX;
    float mouseY;
    float rotX;
    float rotY;

    public float CameraSensitivity = 150f;
    public float ClampX = 70f;
    public Vector3 offsetCameraBase;
    Vector3 mousePos;
    public Vector3 velocity = Vector3.zero;
    [Range(0, 1)]
    public float CameraSpeed = 120;



    int xPos = 30, yPos = 1000;

    public CameraCollision CameraCollision
    {
        get => default;
        set
        {
        }
    }

    public static CameraMovement Instance;

    private void Awake()
    {
        if (Instance != null)
            Debug.LogError("More than one Camera Movement Scripts!!!");
        else Instance = this;
        mousePos = Input.mousePosition;
    }

    // Start is called before the first frame update
    void Start()
    {
        Vector3 rotation = transform.rotation.eulerAngles;
        rotX = rotation.x;
        rotY = rotation.y;


        target = GameObject.FindGameObjectWithTag(CharacterSelection.ChosenCharacter.breed.ToString()).transform;


    }

    bool onRotation = false;
    /// <summary>
    /// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
    /// </summary>
    void FixedUpdate()
    {

        #region Camera Rotation

        if (Input.GetMouseButton(1))
        {
            if (!onRotation)
            {
                xPos = (int)Input.mousePosition.x;
                yPos = (int)Input.mousePosition.y;
                onRotation = true;
            }
            Cursor.visible = false;

            mouseX = Input.GetAxis("Mouse X");
            mouseY = Input.GetAxis("Mouse Y");

            AnghelSlider.value = Mathf.Clamp(AnghelSlider.value, 0.05f, 1);

            rotY += mouseX * CameraSensitivity * AnghelSlider.value;
            rotX += mouseY * CameraSensitivity * AnghelSlider.value;

            rotX = Mathf.Clamp(rotX, -ClampX, ClampX);

            Quaternion rotation = Quaternion.Euler(rotX, rotY, 0.0f);

            transform.rotation = rotation;
        }
        else if (Input.GetMouseButtonUp(1) || !Input.GetMouseButtonDown(0) && !Cursor.visible)
        {
            Cursor.visible = true;
            SetCursorPos(xPos, Screen.height - yPos);//Call this when you want to set the mouse position
            onRotation = false;
        }
        #endregion

    }

    void LateUpdate()
    {
        cameraUpdate();
    }

    void cameraUpdate()
    {
        Vector3 lerp = Vector3.SmoothDamp(transform.position, target.position + offsetCameraBase, ref velocity, CameraSpeed * Time.deltaTime);
        transform.position = lerp;
    }

    public void SetRotation(float x, float y)
    {
        rotX = x;
        rotY = y;
    }
}
