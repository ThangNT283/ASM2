using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillarController : MonoBehaviour
{
    #region Variables
    private PlayerMovement _player;

    [Header("Pillar")]
    [SerializeField] private GameObject _pillar;
    [SerializeField] private GameObject _currentPillar;
    [SerializeField] private GameObject _nextPillar;

    [Header("Key Bind")]
    [SerializeField] private KeyCode _key;

    [Header("Items")]
    [SerializeField] private List<GameObject> _items;

    public GameObject NextPillar { get => _nextPillar; set => _nextPillar = value; }
    #endregion

    #region Unity Methods
    // Initialize components
    void Awake()
    {
        _player = FindFirstObjectByType<PlayerMovement>();
        _currentPillar = Instantiate(_pillar, new Vector3(0, 0, 0), Quaternion.identity);
    }

    // Start spawning pillars
    void Start()
    {
        StartCoroutine(SpawnPillars());
    }
    #endregion

    #region Methods
    private IEnumerator SpawnPillars()
    {
        while (!GameController.Instance.IsLose)
        {
            // Stop spawning when lose
            if (GameController.Instance.IsLose) yield return null;

            // Spawn next pillar
            Vector3 currentPillarPos = _currentPillar.transform.position;
            float randomValue = Random.Range(0f, 3f);
            float minDistance = randomValue <= 2f ? 1f : 2.5f;
            _nextPillar = Instantiate(_pillar,
                    new Vector3(currentPillarPos.x, 0, currentPillarPos.z + Random.Range(minDistance, 5f)),
                    Quaternion.identity);

            if (GameController.Instance.IsEasyWave())
            {
                // Easy Wave: Fixed pillars
                yield return null;
            }
            else if (GameController.Instance.IsMediumWave())
            {
                // Medium Wave: Moving pillar
                yield return StartCoroutine(MovePillar(_nextPillar, currentPillarPos));
            }
            else
            {
                // Hard Wave: Randomize fixed, moving or height changing pillar
                yield return RandomizePillar(_nextPillar, randomValue);
            }

            // Wait until player reach the next pillar
            yield return new WaitUntil(() => _player.IsOnNewPillar);

            // Reset player status and update datas
            _player.IsOnNewPillar = false;
            //Destroy(_currentPillar);
            _currentPillar = _nextPillar;
            GameController.Instance.CurrentWave++;
            yield return new WaitForSeconds(1f);

        }
    }

    // Randomize pillar type: fixed, moving or height changing
    private IEnumerator RandomizePillar(GameObject pillar, float randomValue)
    {
        Vector3 currentPillarPos = _currentPillar.transform.position;

        if (randomValue <= 1f)
        {
            yield return null;
        }
        else if (randomValue <= 2f)
        {
            yield return StartCoroutine(MovePillar(pillar, currentPillarPos));
        }
        else
        {
            yield return StartCoroutine(ChangeHeightPillar(pillar));
        }

        // Randomly spawn item on pillar
        if (Random.Range(0f, 1f) <= 0.5f) SpawnItem(pillar);
    }

    // Move pillar back and forth vertically or horizontally
    private IEnumerator MovePillar(GameObject pillar, Vector3 currentPillarPos)
    {
        bool isStopped = false;
        bool isMovingVertically = Random.value > 0.5;

        float moveSpeed = Random.Range(1.5f, 4f);
        float minX = pillar.transform.position.x - 2.5f;
        float maxX = pillar.transform.position.x + 2.5f;
        float minZ = currentPillarPos.z + 1.2f;
        float maxZ = pillar.transform.position.z + 2.2f;

        do
        {
            Vector3 newPos = pillar.transform.position;
            if (isMovingVertically)
            {
                newPos.z = Mathf.PingPong(Time.time * moveSpeed, maxZ - minZ) + minZ;
            }
            else
            {
                newPos.x = Mathf.PingPong(Time.time * moveSpeed, maxX - minX) + minX;
            }
            pillar.transform.position = newPos;

            // Stop moving when player press the key
            //if (Input.GetKeyDown(_key)) isStopped = true;
            if (Input.GetMouseButtonDown(0)) isStopped = true;
            yield return null;
        } while (!isStopped);
    }

    // Change pillar height back and forth
    private IEnumerator ChangeHeightPillar(GameObject pillar)
    {
        bool isStopped = false;

        float scaleSpeed = Random.Range(1f, 2.5f);
        float minScale = 0.5f;
        float maxScale = 1.5f;

        do
        {
            float newScaleY = Mathf.PingPong(Time.time * scaleSpeed, maxScale - minScale) + minScale;
            pillar.transform.localScale = new Vector3(pillar.transform.localScale.x, newScaleY, pillar.transform.localScale.z);

            // Stop changing height when player press the key
            // if (Input.GetKeyDown(_key)) isStopped = true;
            if (Input.GetMouseButtonDown(0)) isStopped = true;

            yield return null;
        } while (!isStopped);
    }

    // Spawn a random item on top of pillar
    private void SpawnItem(GameObject pillar)
    {
        // Get top position of pillar
        Vector3 topPillarPos = pillar.transform.position + (pillar.transform.up * pillar.transform.localScale.y);
        topPillarPos.y += 0.5f;

        // Randomly spawn item
        System.Random rnd = new System.Random();
        GameObject item = Instantiate(_items[rnd.Next(_items.Count)], topPillarPos, Quaternion.identity);
        item.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
    }
    #endregion
}