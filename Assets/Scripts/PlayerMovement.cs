using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region Variables
    Rigidbody _rb;
    Animator _animator;

    [Header("Movement")]
    [SerializeField] private float _moveSpd;
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _jumpCooldown;
    [SerializeField] private bool _readyToJump = true;
    [SerializeField] private bool _grounded;

    [Header("Settings")]
    [SerializeField] private Transform _orientation;
    [SerializeField] private Transform _ground;
    [SerializeField] private KeyCode _jumpKey;

    private float _horizontalInput;
    private float _verticalInput;

    private Vector3 _moveTarget;
    private bool _isMovingToBridge = false;
    private bool _isOnNewPillar = false;

    public bool IsOnNewPillar { get => _isOnNewPillar; set => _isOnNewPillar = value; }
    #endregion

    #region Unity Methods
    // Initialize components
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;
        _animator = GetComponentInChildren<Animator>();
    }

    // Get player input every frame
    private void Update()
    {
        _grounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);
        MyInput();

        if (_isMovingToBridge && Mathf.Abs(transform.position.z - _moveTarget.z) < 0.1f)
        {
            _moveTarget = Vector3.zero;
            _isMovingToBridge = false;
            Destroy(FindFirstObjectByType<BridgeSpawner>().CurrentBridge);
        }
    }

    // Move player every fixed frame
    private void FixedUpdate()
    {
        MovePlayer();
    }

    // Check collision with other objects
    private void OnCollisionEnter(Collision collision)
    {
        // Lose when hit the plane
        if (collision.gameObject.CompareTag("Plane"))
        {
            GameController.Instance.IsLose = true;
        }
        // Add score when reach the next pillar
        else if (collision.gameObject == FindFirstObjectByType<PillarController>().NextPillar)
        {
            GameController.Instance.AddScore(1);
            GameController.Instance.AddPillar();
            _isOnNewPillar = true;
        }
    }
    #endregion

    #region Methods
    // Move player in automatic mode or manual mode
    private void MovePlayer()
    {
        Vector3 moveDirection;

        //if (_isAutoMove)
        //{
        //    // Automatic mode
        //    moveDirection = (_moveTarget - transform.position).normalized;

        //    // Stop moving when close to target
        //    if (Mathf.Abs(transform.position.z - _moveTarget.z) < 0.1f)
        //    {
        //        _animator.SetBool("IsMove", false);
        //        _rb.linearVelocity = Vector3.zero;
        //        _isAutoMove = false;
        //        _moveTarget = Vector3.zero;

        //        // Destroy the bridge after player moves
        //        Destroy(FindFirstObjectByType<BridgeSpawner>().CurrentBridge);
        //    }
        //    else
        //    {
        //        _animator.SetBool("IsMove", true);
        //        _rb.linearVelocity = new Vector3(moveDirection.x * _moveSpd, _rb.linearVelocity.y, moveDirection.z * _moveSpd);
        //    }
        //}
        //else
        //{
        // Manual mode
        moveDirection = _orientation.forward * _verticalInput + _orientation.right * _horizontalInput;

        // Stop moving when no input
        if (moveDirection == Vector3.zero) _animator.SetBool("IsMove", false);
        else _animator.SetBool("IsMove", true);

        int multiplier = FindFirstObjectByType<Bridge>() ? 3 : 1;
        _rb.linearVelocity = new Vector3(moveDirection.x * _moveSpd * 0.25f * multiplier,
            _rb.linearVelocity.y,
            moveDirection.z * _moveSpd * 0.25f * multiplier);
        //}

        // Keep player on the ground
        _ground.position = new Vector3(transform.position.x, _ground.position.y, transform.position.z);
    }

    // Enable automatic mode for player movement
    public void StartAutoMove(Vector3 target)
    {
        _moveTarget = target;
        _isMovingToBridge = true;
    }

    // Get player input
    private void MyInput()
    {
        _horizontalInput = Input.GetAxis("Horizontal");
        _verticalInput = Input.GetAxis("Vertical");

        if (Input.GetKey(_jumpKey) && _readyToJump)
        {
            _readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), _jumpCooldown);
        }
    }

    // Make player jump 
    private void Jump()
    {
        // Reset y velocity
        _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);

        // Add force to jump
        _rb.AddForce(transform.up * _jumpForce, ForceMode.Impulse);
    }

    // Reset jump status
    private void ResetJump()
    {
        _readyToJump = true;
    }
    #endregion
}