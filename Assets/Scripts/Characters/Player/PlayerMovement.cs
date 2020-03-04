using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Configuration;

public class PlayerMovement : MonoBehaviour
{
    //
    //PUBLIC VARIABLES
    //
    #region Public Variables

    public GameObject camera;

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
        //if (!gameObject.tag.Equals(SelectCharacter.SelectedGameObject))
        //    gameObject.SetActive(false);
        rigid = GetComponent<Rigidbody>();
        playerData = GetComponent<PlayerData>();
    }


    private void Update()
    {

        RaycastHit hit;
        grounded = Physics.Raycast(transform.position + new Vector3(0, 0.1f, 0), new Vector3(0, -rayLength, 0), out hit, 1);
        //if (!grounded) rayLength += 0.005f;
        //else rayLength = 0.001f;
        Debug.DrawRay(transform.position, new Vector3(0, -rayLength, 0), Color.yellow);
        //print(hit.collider.name);
        //print(fps = (1.0f / Time.deltaTime));

    }

    private void FixedUpdate()
    {
        if (playerData.defaultStats.Alive)
        {
            Rotate();
            // if (grounded)
            Move();
        }
    }

    private void Move()
    {

        Vector2 axisInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            //  print("Moving");
            //Get the Input Axis
            Vector3 rightDir = camera.transform.right;
            Vector3 forwardDir = camera.transform.forward;

            //Exclude Y Axis to avoid tilting forward or backwards
            rightDir.y = 0;
            forwardDir.y = 0;

            //Apply velocity
            direction = (Input.GetAxis("Horizontal") * rightDir + Input.GetAxis("Vertical") * forwardDir).normalized;
            Vector3 temp = direction * Time.fixedDeltaTime * speed;
            rigid.velocity = new Vector3(temp.x, rigid.velocity.y, temp.z);
            // print(rigid.velocity);

        }
        else rigid.velocity = new Vector3(0, rigid.velocity.y, 0);

        axisInput = axisInput.normalized;
        playerData.anim.SetFloat("SpeedMovement", axisInput.magnitude);

    }

    private void Rotate()
    {
        //Face towards the moving direction
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            direction = (Input.GetAxisRaw("Horizontal") * camera.transform.right + Input.GetAxisRaw("Vertical") * camera.transform.forward).normalized;
            Vector3 newDirection = new Vector3(direction.x, 0.0f, direction.z);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(newDirection), 0.2f);
            lastRotation = transform.rotation;
        }
        //else transform.rotation = lastRotation;
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if(collision.gameObject.layer.Equals(LayerMask.NameToLayer("Ground")))
    //        grounded = true;
    //}
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer.Equals(LayerMask.NameToLayer("Ground")))
            grounded = true;
    }

    //private void OnCollisionStay2D(Collision2D collision)
    //{
    //    if (collision.gameObject.layer.Equals(LayerMask.NameToLayer("Ground")))
    //        grounded = false;
    //}


}




