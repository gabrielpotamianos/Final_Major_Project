﻿using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //
    //PUBLIC VARIABLES
    //
    #region Public Variables

    public new GameObject camera;

    [Range(0, 1)]
    public float rayLength;
    public float speed = 5;

    #endregion

    //
    //PRIVATE VARIABLES
    //
    #region Private Variables

    bool grounded;
    Rigidbody rigid;
    Vector3 direction;
    PlayerData playerData;
    Quaternion lastRotation;
    #endregion


    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        playerData = GetComponent<PlayerData>();
    }

    private void Start()
    {
        camera = Camera.main.gameObject;
    }


    private void Update()
    {
        if (camera == null)
            camera = Camera.main.gameObject;
    }

    private void FixedUpdate()
    {
        if (playerData.Alive)
        {
            Rotate();

            Move();
        }
    }

    private void Move()
    {

        Vector2 axisInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (axisInput.x!=0 || axisInput.y!=0)
        {
            MageCombatSystem.InteruptCast = true;

            //Get the Input Axis
            Vector3 rightDir = camera.transform.right;
            Vector3 forwardDir = camera.transform.forward;

            //Exclude Y Axis to avoid tilting forward or backwards
            rightDir.y = 0;
            forwardDir.y = 0;
            
            //Apply velocity
            direction = (axisInput.x * rightDir + axisInput.y * forwardDir).normalized;

            Vector3 desiredVelocity = direction * Time.deltaTime * speed;
            rigid.velocity = new Vector3(desiredVelocity.x, rigid.velocity.y, desiredVelocity.z);
        }
        else
        {
            MageCombatSystem.InteruptCast = false;
            rigid.velocity = new Vector3(0, rigid.velocity.y, 0);
        }

        axisInput = axisInput.normalized;
        playerData.animator.SetFloat("SpeedMovement", axisInput.magnitude);

    }

    private void Rotate()
    {
        //Face towards the moving direction
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            direction = (Input.GetAxisRaw("Horizontal") * camera.transform.right + Input.GetAxisRaw("Vertical") * camera.transform.forward).normalized;
            Vector3 newDirection = new Vector3(direction.x, 0.0f, direction.z);
            newDirection *= Time.deltaTime;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(newDirection), 0.2f);
            lastRotation = transform.rotation;
        }
    }




}




