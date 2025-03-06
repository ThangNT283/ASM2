using TMPro;
using UnityEngine;

public class FinalScore : MonoBehaviour
{
    public TextMeshProUGUI text;

    private void Awake()
    {
        text.text = "SCORE: " + GameController.Instance.Score.ToString();
    }
}
