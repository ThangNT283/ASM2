using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    #region Variables
    public static GameController Instance { get; private set; }

    [Header("Waves")]
    [SerializeField] private int _mediumWave = 6;
    [SerializeField] private int _hardWave = 11;
    [SerializeField] private int _currentWave = 1;

    [Header("Status")]
    [SerializeField] private int _score = 0;
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private int _pillar = 0;
    [SerializeField] private TextMeshProUGUI _pillarText;
    [SerializeField] private int _lives = 3;
    [SerializeField] private Transform _livesUI;
    [SerializeField] private bool _isLose = false;

    private bool isGetComponent = false;

    public int Score { get => _score; set => _score = value; }
    public bool IsLose { get => _isLose; set => _isLose = value; }
    public int CurrentWave { get => _currentWave; set => _currentWave = value; }
    public int Lives { get => _lives; set => _lives = value; }
    public bool IsChangeCameraPos { get; set; } = false;
    public bool IsRestart { get; set; } = false;
    #endregion

    // Singleton pattern
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

        Initialize();
    }

    private void Update()
    {
        if (IsRestart)
        {
            IsRestart = false;
            isGetComponent = true;
            SceneManager.LoadScene("MainScene");
        }

        try
        {
            if (isGetComponent && (_scoreText == null || _pillarText == null || _livesUI == null))
            {
                Initialize();
            }
        }
        catch (System.Exception e)
        {

        }

        if (_scoreText != null && _pillarText != null && _livesUI != null)
        {
            isGetComponent = false;
        }

        if (_isLose)
        {
            IsLose = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            //CursorLockMode.None;
            SceneManager.LoadScene("EndScene");
        }
    }

    #region Methods
    public void Initialize()
    {
        _scoreText = FindFirstObjectByType<ScoreText>().GetComponent<TextMeshProUGUI>();
        _pillarText = FindFirstObjectByType<PillarText>().GetComponent<TextMeshProUGUI>();
        _livesUI = FindFirstObjectByType<Lives>().gameObject.transform;

        if (_scoreText == null || _pillarText == null || _livesUI == null) return;

        _score = 0;
        _scoreText.text = _score.ToString();
        _pillar = 0;
        _pillarText.text = _pillar.ToString();
        foreach (Transform child in _livesUI)
        {
            child.gameObject.GetComponent<Animator>().SetBool("isDisappear", false);
        }
        _isLose = false;
        _currentWave = 1;
    }

    // Increase score and update UI
    public void AddScore(int value)
    {
        _score += value;
        _scoreText.text = _score.ToString();
    }

    public void AddPillar()
    {
        _pillar++;
        _pillarText.text = _pillar.ToString();
    }

    public void LoseLife()
    {
        _lives--;
        GameObject heart = _livesUI.GetChild(_lives).gameObject;
        heart.GetComponent<Animator>().SetBool("isDisappear", true);
        //heart.SetActive(false);
        if (_lives <= 0) _isLose = true;
    }

    // Check if the current wave is easy
    public bool IsEasyWave()
    {
        return _currentWave < _mediumWave;
    }

    // Check if the current wave is medium
    public bool IsMediumWave()
    {
        return _currentWave >= _mediumWave && _currentWave < _hardWave;
    }
    #endregion
}
