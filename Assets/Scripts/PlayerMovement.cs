using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;

    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [HideInInspector] public float walkSpeed;
    [HideInInspector] public float sprintSpeed;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;
    Animator animator;

    bool isAutoMove = false;
    public Vector3 moveTarget;
    public bool isOnNewPillar = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        animator = GetComponentInChildren<Animator>();

        readyToJump = true;
    }

    private void Update()
    {
        // ground check
        //grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround);

        MyInput();
        SpeedControl();

        //// handle drag
        //if (grounded)
        //    rb.linearDamping = groundDrag;
        //else
        //    rb.linearDamping = 0;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        if (isAutoMove)
        {
            moveDirection = (moveTarget - transform.position).normalized;
            //if (Vector3.Distance(transform.position, moveTarget) < 0.1f)
            if (Mathf.Abs(transform.position.z - moveTarget.z) < 0.1f)
            {
                animator.SetBool("IsMove", false);
                rb.linearVelocity = Vector3.zero;
                isAutoMove = false;
                moveTarget = Vector3.zero;
                Destroy(FindFirstObjectByType<BridgeSpawner>().currentBridge);
            }
            else
            {
                animator.SetBool("IsMove", true);
                rb.linearVelocity = new Vector3(moveDirection.x * moveSpeed, rb.linearVelocity.y, moveDirection.z * moveSpeed);
            }
        }
        else
        {
            // calculate movement direction
            moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

            if (moveDirection == Vector3.zero)
            {
                animator.SetBool("IsMove", false);
            }
            else
            {
                animator.SetBool("IsMove", true);
            }

            rb.linearVelocity = new Vector3(moveDirection.x * moveSpeed * 0.25f, rb.linearVelocity.y, moveDirection.z * moveSpeed * 0.25f);
        }
    }

    public void MoveOnBridge(Vector3 target)
    {
        moveTarget = target;
        isAutoMove = true;
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        // when to jump
        //if (Input.GetKey(jumpKey) && readyToJump && grounded)
        //{
        //    readyToJump = false;

        //    Jump();

        //    Invoke(nameof(ResetJump), jumpCooldown);
        //}
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Plane"))
        {
            GameController.Instance.isLose = true;
        }
        else
        if (collision.gameObject == FindFirstObjectByType<PillarController>().nextPillar)
        {
            GameController.Instance.AddScore(1);
            isOnNewPillar = true;
        }
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        // limit velocity if needed
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }
    private void Jump()
    {
        // reset y velocity
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        readyToJump = true;
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * .3f);
    }
}