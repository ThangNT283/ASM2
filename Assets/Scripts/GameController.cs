using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    [Header("Game Status")]
    public int score = 0;
    public TextMeshProUGUI scoreText;
    public bool isLose = false;

    [Header("Wave Properties")]
    public int currentWave = 1;
    public int mediumWave = 6;
    public int hardWave = 11;

    public bool isChangeCameraPos = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddScore(int value)
    {
        score += value;
        scoreText.text = "SCORE: " + score.ToString();
    }
}
