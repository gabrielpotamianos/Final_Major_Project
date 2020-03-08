using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform target;

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



    private void Awake()
    {
        CharacterInSelection.SelectedGameObject = "Rogue";
        mousePos = Input.mousePosition;
    }

    // Start is called before the first frame update
    void Start()
    {
        Vector3 rotation = transform.rotation.eulerAngles;
        rotX = rotation.x;
        rotY = rotation.y;


        //target = GameObject.FindGameObjectWithTag(SelectCharacter.SelectedGameObject).transform;
        target = GameObject.FindGameObjectWithTag("Warrior").transform;
        

    }
    private void Update()
    {
        target = GameObject.FindGameObjectWithTag("Warrior").transform;

        #region Camera Rotation

        if (Input.GetMouseButton(1))
        {

            Cursor.visible = false;

            mouseX = Input.GetAxis("Mouse X");
            mouseY = Input.GetAxis("Mouse Y");

            rotY += mouseX * CameraSensitivity * Time.smoothDeltaTime;
            rotX += mouseY * CameraSensitivity * Time.smoothDeltaTime;

            rotX = Mathf.Clamp(rotX, -ClampX, ClampX);

            Quaternion rotation = Quaternion.Euler(rotX, rotY, 0.0f);

            transform.rotation = rotation;

        }
        else if(Input.GetMouseButtonUp(1))
        {
            Cursor.visible = true;
        }
        #endregion
    }



    void LateUpdate()
    {
        cameraUpdate();
    }

    void cameraUpdate()
    {
        Vector3 lerp = Vector3.SmoothDamp(transform.position, target.position + offsetCameraBase, ref velocity, CameraSpeed);
        transform.position = lerp;

       
    }
}
