using System.Collections;
using UnityEngine;

public class PillarController : MonoBehaviour
{
    [Header("Pillar")]
    public GameObject pillar;
    public GameObject currentPillar;
    public GameObject nextPillar;

    [Header("Key Bind")]
    public KeyCode key;

    void Awake()
    {
        currentPillar = Instantiate(pillar, new Vector3(0, 0, 0), Quaternion.identity);
    }

    void Start()
    {
        StartCoroutine(SpawnPillars());
    }

    void Update()
    {

    }

    private IEnumerator SpawnPillars()
    {
        // Note: <40 is for testing
        while (!GameController.Instance.isLose && GameController.Instance.currentWave < 40)
        {
            Vector3 currentPillarPos = currentPillar.transform.position;
            GameObject nextPillar = Instantiate(pillar,
                    new Vector3(currentPillarPos.x, 0, currentPillarPos.z + Random.Range(1f, 5f)),
                    Quaternion.identity);

            if (GameController.Instance.currentWave < GameController.Instance.mediumWave)
            {
                // Easy Wave: Ffixed pillars
                yield return null;
            }
            else if (GameController.Instance.currentWave < GameController.Instance.hardWave)
            {
                // Medium Wave: Moving pillar
                yield return StartCoroutine(MovePillar(nextPillar, currentPillarPos));
            }
            else
            {
                // Hard Wave: Randomize fixed, moving or height changing pillar
                yield return RandomizePillar(nextPillar);
            }

            yield return new WaitForSeconds(1f);
            currentPillar = nextPillar;
            GameController.Instance.currentWave++;
        }
    }

    private IEnumerator MovePillar(GameObject pillar, Vector3 currentPillarPos)
    {
        bool isStopped = false;
        float moveSpeed = Random.Range(1.5f, 4f);
        float minZ = currentPillarPos.z + 1.2f;
        float maxZ = pillar.transform.position.z + 2.2f;
        float minX = pillar.transform.position.x - 2.5f;
        float maxX = pillar.transform.position.x + 2.5f;
        bool isMovingZ = Random.value > 0.5;

        while (!isStopped)
        {
            Vector3 newPos = pillar.transform.position;
            if (isMovingZ)
            {
                newPos.z = Mathf.PingPong(Time.time * moveSpeed, maxZ - minZ) + minZ;
            }
            else
            {
                newPos.x = Mathf.PingPong(Time.time * moveSpeed, maxX - minX) + minX;
            }
            pillar.transform.position = newPos;

            if (Input.GetKeyDown(key)) isStopped = true;
            yield return null;
        }
    }

    private IEnumerator ScalePillar(GameObject pillar)
    {
        bool isStopped = false;
        float scaleSpeed = Random.Range(1f, 2.5f);
        float minScale = 0.5f;
        float maxScale = 2f;

        while (!isStopped)
        {
            float newScaleY = Mathf.PingPong(Time.time * scaleSpeed, maxScale - minScale) + minScale;
            pillar.transform.localScale = new Vector3(pillar.transform.localScale.x, newScaleY, pillar.transform.localScale.z);

            if (Input.GetKeyDown(key)) isStopped = true;
            yield return null;
        }
    }

    private IEnumerator RandomizePillar(GameObject pillar)
    {
        Vector3 currentPillarPos = currentPillar.transform.position;
        float value = Random.Range(0f, 3f);

        if (value <= 1f)
        {
            yield return null;
        }
        else if (value <= 2f)
        {
            yield return StartCoroutine(MovePillar(pillar, currentPillarPos));
        }
        else
        {
            yield return StartCoroutine(ScalePillar(pillar));
        }
    }
}