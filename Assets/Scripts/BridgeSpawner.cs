using System.Collections;
using UnityEngine;

public class BridgeSpawner : MonoBehaviour
{
    public GameObject bridgePrefab; // Assign the bridge prefab in the Inspector
    private GameObject currentBridge; // Stores the active bridge
    public Transform spawnPoint; // Where the bridge spawns (should be near the player)
    public float growthSpeed = 2f;
    public float forceAmount = 500f; // Adjust the force strength

    [SerializeField] private Transform player;
    private bool isGrowing = false;
    private bool isSpawning = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Click to spawn a new bridge
        {
            SpawnBridge();
            isGrowing = true;
        }

        if (Input.GetMouseButtonUp(0)) // Release to stop growing & apply force at top
        {
            isGrowing = false;
            isSpawning = false;
            StartCoroutine(ApplyForceAtTop());
        }

        if (isGrowing && currentBridge != null)
        {
            currentBridge.transform.localScale += new Vector3(0, growthSpeed * Time.deltaTime, 0);
        }
    }

    void SpawnBridge()
    {
        if (isSpawning) return;
        isSpawning = true;
        currentBridge = Instantiate(bridgePrefab, spawnPoint.position, Quaternion.identity);
        currentBridge.transform.rotation = player.transform.rotation;

        // Add Rigidbody if not already attached
        if (!currentBridge.GetComponent<Rigidbody>())
        {
            Rigidbody rb = currentBridge.AddComponent<Rigidbody>();
            rb.isKinematic = true; // Prevent immediate physics interactions
            rb.mass = 5;
        }
    }

    private IEnumerator ApplyForceAtTop()
    {
        if (currentBridge == null) yield break;

        // Enable Rigidbody Physics
        Rigidbody rb = currentBridge.GetComponent<Rigidbody>();
        rb.isKinematic = false;  // Allow physics to take control
        rb.useGravity = true;     // Enable gravity

        // Calculate the top position of the bridge
        Vector3 topPosition = currentBridge.transform.position + (currentBridge.transform.up * (currentBridge.transform.localScale.y * 2));

        // Apply force at the top
        rb.AddForceAtPosition(Vector3.forward * forceAmount, topPosition, ForceMode.Impulse);

        yield return null; // Just to allow physics to process
    }
}
