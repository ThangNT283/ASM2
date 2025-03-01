using System.Collections;
using UnityEngine;

public class BridgeSpawner : MonoBehaviour
{
    public GameObject bridgePrefab; // Assign the bridge prefab in the Inspector
    private GameObject currentBridge; // Stores the active bridge
    public Transform spawnPoint; // Where the bridge spawns (should be near the player)
    public float growthSpeed = 2f;

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

        if (Input.GetMouseButtonUp(0)) // Release to stop growing & rotate
        {
            isGrowing = false;
            isSpawning = false;
            StartCoroutine(RotateBridge());
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
    }

    private IEnumerator RotateBridge()
    {
        if (currentBridge == null) yield break;

        float elapsedTime = 0;
        float duration = 0.3f;
        Quaternion startRotation = currentBridge.transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(90, player.eulerAngles.y, player.eulerAngles.z);

        while (elapsedTime < duration)
        {
            currentBridge.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        currentBridge.transform.rotation = targetRotation;
    }
}
