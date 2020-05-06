using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //
    //PUBLIC VARIABLES
    //
    #region Public Variables

    public new GameObject camera;
    public float JumpForce;
    public float JumpRayDistance;
    [Range(0, 1)]
    public float rayLength;
    public float speed = 5;

    public static PlayerMovement instance;
    #endregion

    //
    //PRIVATE VARIABLES
    //
    #region Private Variables

    bool jumping;
    Rigidbody rigid;
    Vector3 direction;
    PlayerData playerData;
    Quaternion lastRotation;
    #endregion
    RaycastHit hit;

    bool grounded;


    private void Awake()
    {
        // if (instance == null)
        //     instance = this;
        // else Debug.LogError("More than One Player Movement instances!");

        rigid = GetComponent<Rigidbody>();
        playerData = GetComponent<PlayerData>();
    }

    private void Start()
    {
        if (instance == null)
            instance = this;
        else Debug.LogError("More than One Player Movement instances!");

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
            print(grounded);
            grounded = Physics.Raycast(transform.position - new Vector3(0, -0.5f, 0), Vector3.down, out hit, JumpRayDistance, 1 << LayerMask.NameToLayer("Ground"));
            if (Input.GetKeyDown(KeyCode.Space) && grounded && !jumping)
                Jump();
            if (jumping &&  Mathf.Round(rigid.velocity.y) < 0 && grounded)
                StartLanding();

            playerData.animator.SetBool("Jump", jumping);

            Debug.DrawRay(transform.position - new Vector3(0, -0.5f, 0), Vector3.down * JumpRayDistance, Color.yellow);

            Rotate();

            Move();
        }
    }

    private void Move()
    {

        Vector2 axisInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (axisInput.x != 0 || axisInput.y != 0)
        {
            MageCombatSystem.InteruptCast = true;
            playerData.animator.SetBool("Looting", false);
            Looting.instance.HideInventory();

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

    private void StartLanding()
    {
        playerData.animator.SetBool("Land", true);
    }
    private void Jump()
    {
        jumping = true;
    }

    private void JumpAnimationEvent()
    {
        //rigid.AddForce(new Vector3(0, 1, 0) * JumpForce * Time.deltaTime, ForceMode.VelocityChange);
        rigid.velocity = new Vector3(rigid.velocity.x, 0, rigid.velocity.z);
        rigid.velocity = new Vector3(rigid.velocity.x, JumpForce, rigid.velocity.z);
    }

    private void Land()
    {
        rigid.velocity = new Vector3(rigid.velocity.x, 0, rigid.velocity.z);

        jumping = false;
        playerData.animator.SetBool("Land", false);

    }




}




