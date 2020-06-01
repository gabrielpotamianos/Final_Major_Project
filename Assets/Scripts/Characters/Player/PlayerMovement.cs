using UnityEngine;
using UnityEngine.UI;
public class PlayerMovement : MonoBehaviour
{


    public new GameObject camera;
    public float JumpForce;
    public float JumpRayDistance;

    public float speed = 5;
    public bool OnGround = true;
    public LayerMask JumpLayerMask;
    public static PlayerMovement instance;



    Rigidbody rigid;
    Vector3 direction;
    Quaternion lastRotation;
    RaycastHit hit;
    bool grounded;
    bool jumping;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
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
        grounded = Physics.Raycast(transform.position - new Vector3(0, -0.5f, 0), Vector3.down, out hit, JumpRayDistance, JumpLayerMask);
        if (PlayerData.instance.Alive)
            Jump();

        OnGround = !PlayerData.instance.animator.GetBool("Jump") && !PlayerData.instance.animator.GetBool("Land");
        SpellChecks.Grounded = OnGround;
    }

    private void FixedUpdate()
    {
        if (PlayerData.instance.Alive)
        {
            Move();
            Rotate();
        }
    }

    private void Move()
    {

        Vector2 axisInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;


        if (axisInput.y != 0 || axisInput.x != 0)
        {
            MageCombatSystem.InteruptCast = true;
            PlayerData.instance.animator.SetBool("Looting", false);
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
        PlayerData.instance.animator.SetFloat("SpeedMovement", axisInput.magnitude);


    }

    private void Rotate()
    {
        //Face towards the moving direction
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            direction = (Input.GetAxisRaw("Horizontal") * camera.transform.right + Input.GetAxisRaw("Vertical") * camera.transform.forward).normalized;
            Vector3 newDirection = new Vector3(direction.x, 0.0f, direction.z);
            newDirection *= Time.deltaTime;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(newDirection), 0.4f);
            lastRotation = transform.rotation;
        }
    }

    private void Jump()
    {
        //Jump if on the ground and space bar was hit
        if (Input.GetKeyDown(KeyCode.Space) && grounded && !jumping)
        {
            MageCombatSystem.InteruptCast = true;
            PlayerData.instance.animator.SetBool("Jump", true);

        }

        //Land if velocity is on negative Y and raycast detected ground
        if (jumping && Mathf.Round(rigid.velocity.y) < 0 && grounded)
            PlayerData.instance.animator.SetBool("Land", true);

    }

    private void JumpAnimationEvent()
    {
        jumping = true;

        //Reset Y or will create a bug on declined zone or a valley ( creates negative Y and plays the Land animation)
        rigid.velocity = new Vector3(rigid.velocity.x, 0, rigid.velocity.z);
        rigid.velocity = new Vector3(rigid.velocity.x, JumpForce, rigid.velocity.z);
    }

    private void Land()
    {
        jumping = false;
        PlayerData.instance.animator.SetBool("Land", false);
        PlayerData.instance.animator.SetBool("Jump", jumping);
        MageCombatSystem.InteruptCast = false;


    }




}




