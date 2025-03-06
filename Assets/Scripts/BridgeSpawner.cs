using System.Collections;
using UnityEngine;

public class BridgeSpawner : MonoBehaviour
{
    #region Variables
    [SerializeField] private GameObject _bridge;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private GameObject _currentBridge;
    [SerializeField] private float _growthSpd = 2f;
    [SerializeField] private float _fallingForce = 1f;
    [SerializeField] private GameObject _obstacle;

    private Transform _player;

    private bool _isGrowing = false;
    private bool _isSpawning = false;
    private bool _isFalling = false;

    public GameObject CurrentBridge { get => _currentBridge; }
    #endregion

    #region Unity Methods
    private void Awake()
    {
        _player = FindFirstObjectByType<PlayerMovement>().transform;
    }

    void Update()
    {
        // Hold left mouse button to start growing the bridge
        if (!_isFalling && Input.GetMouseButtonDown(0))
        {
            SpawnBridge();
            _isGrowing = true;
        }

        // Release left mouse button to stop growing & apply force at top to make it fall
        if (Input.GetMouseButtonUp(0) && !_isFalling)
        {
            _isGrowing = false;
            _isSpawning = false;
            _isFalling = true;
            StartCoroutine(ApplyTopForce());
        }

        // Grow the bridge
        if (_isGrowing && _currentBridge != null)
        {
            _currentBridge.transform.localScale += new Vector3(0, _growthSpd * Time.deltaTime, 0);
        }
    }
    #endregion

    #region Methods
    // Spawn a bridge at the spawn point
    void SpawnBridge()
    {
        if (_isSpawning) return;

        _isSpawning = true;
        _currentBridge = Instantiate(_bridge, _spawnPoint.position, Quaternion.identity);
        _currentBridge.transform.rotation = _player.transform.rotation; // Align with player rotation

        // Add Rigidbody if not already attached
        if (!_currentBridge.GetComponent<Rigidbody>())
        {
            Rigidbody rb = _currentBridge.AddComponent<Rigidbody>();
            rb.isKinematic = true; // Prevent immediate physics interactions
            rb.mass = 5;
        }
    }

    // Apply force at the top of the bridge to make it fall
    private IEnumerator ApplyTopForce()
    {
        if (_currentBridge == null) yield break;

        // Enable physics
        Rigidbody rb = _currentBridge.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.useGravity = true;

        // Get bridge top position
        Vector3 topPosition = _currentBridge.transform.position +
            (_currentBridge.transform.up * (_currentBridge.transform.localScale.y * 2));

        // Apply force at the top position
        rb.AddForceAtPosition(Vector3.forward * _fallingForce, topPosition, ForceMode.Impulse);

        // Wait for the bridge to settle
        yield return new WaitForSeconds(2.5f);
        _isFalling = false;
        _currentBridge.GetComponent<BoxCollider>().enabled = true;

        // Get the new top position after the bridge has settled
        Vector3 settledTopPosition = _currentBridge.transform.position +
            (_currentBridge.transform.up * _currentBridge.transform.localScale.y);

        // Randomly spawn an obstacle on the bridge
        if (Random.value <= 1f)
        {
            Vector3 obstaclePosition = _currentBridge.transform.position +
                (_currentBridge.transform.up * (_currentBridge.transform.localScale.y) / 2);
            obstaclePosition.y += 0.2f;
            Instantiate(_obstacle, obstaclePosition, Quaternion.identity);
        }

        // Move the player to the top point
        _player.GetComponent<PlayerMovement>().StartAutoMove(settledTopPosition);

        yield return null;
    }
    #endregion
}
