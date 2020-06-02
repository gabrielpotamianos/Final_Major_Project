using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//C#
using System.Runtime.InteropServices;
// 

public class CameraMovement : MonoBehaviour
{
    //Used dll to maintain cursor position after rotation
    [DllImport("user32.dll")]
    static extern bool SetCursorPos(int X, int Y);

    [Range(0, 1)]
    public float CameraSpeed = Constants.CAMERA_MOVEMENT_SPEED;

    //Slider from Settings Menu
    public Slider RotationSensitivity;

    //The camera position offset
    public Vector3 OffsetCameraBase;

    //Velocity needed for SmoothDamp
    Vector3 Velocity = Vector3.zero;

    //Player reference
    Transform Player;
    
    //Base value of camera sensitivity
    float CameraSensitivity = Constants.CAMERA_SENSITIVITY;

    //Do not allow over rotation
    float ClampX = Constants.CAMERA_CLAMP_X;

    //Mouse Position and Rotations
    float MouseX, MouseY, RotX, RotY;

    //Mouse positions before rotation
    int SavedPosX = 30, SavedPosY = 1000;

    bool MousePosSaved = false;



    public static CameraMovement Instance;

    private void Awake()
    {
        //Singleton Assignment
        if (Instance != null)
            Debug.LogError(Constants.SINGLETON_ERROR + this.name.ToString());
        else Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        //Save Starting Rotation
        Vector3 rotation = transform.rotation.eulerAngles;
        RotX = rotation.x;
        RotY = rotation.y;

        //Get the Player's Transform
        Player = CharacterSelection.ChosenCharacter.Character.transform;

    }

    private void Update()
    {

        #region Camera Rotation

        //Check for Right Click Input
        if (Input.GetMouseButton(1))
        {
            //Is this the first frame on rotation?
            if (!MousePosSaved)
            {
                //Then Save Mouse Position 
                SavedPosX = (int)Input.mousePosition.x;
                SavedPosY = (int)Input.mousePosition.y;

                //Do not enter again until the release of the button
                MousePosSaved = true;
            }

            Cursor.visible = false;

            MouseX = Input.GetAxis("Mouse X");
            MouseY = Input.GetAxis("Mouse Y");

            //Do not allow the slider value to go under 0.015f
            RotationSensitivity.value = Mathf.Clamp(RotationSensitivity.value, Constants.CAMERA_SLIDER_CLAMP_MIN, Constants.CAMERA_SLIDER_CAMP_MAX);

            //Calculate Rotation
            RotY += MouseX * CameraSensitivity * RotationSensitivity.value;
            RotX += MouseY * CameraSensitivity * RotationSensitivity.value;

            //Limit the X rotation to the clamp boundaries
            RotX = Mathf.Clamp(RotX, -ClampX, ClampX);

            //Get Quaternion
            Quaternion rotation = Quaternion.Euler(RotX, RotY, 0.0f);

            //Apply Rotation
            transform.rotation = rotation;
        }

        //If the Right Button has been released 
        else if (Input.GetMouseButtonUp(1) || !Input.GetMouseButtonDown(0) && !Cursor.visible)
        {
            Cursor.visible = true;

            //Reset Cursor to its initial position on the screen 
            SetCursorPos(SavedPosX, Screen.height - SavedPosY);

            //Allows to save initial position on rotation beginning again
            MousePosSaved = false;
        }
        #endregion

    }


    void LateUpdate()
    {
        cameraUpdate();
    }

    void cameraUpdate()
    {
        //Follows the player
        Vector3 lerp = Vector3.SmoothDamp(transform.position, Player.position + OffsetCameraBase, ref Velocity, CameraSpeed * Time.deltaTime);
        transform.position = lerp;
    }

    public void SetRotation(float x, float y)
    {
        RotX = x;
        RotY = y;
    }
}
