using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public TextMeshProUGUI scoreText;

    private void Awake() => Instance = this;

    public void UpdateScore(int score)
    {
        if (scoreText) scoreText.text = "Candies: " + score;
    }
}