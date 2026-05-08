using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public TextMeshProUGUI scoreText;
    public int score = 0;

    public GameObject gameOverPanel;

    private void Awake() => Instance = this;

    public void AddScore(int amount)
    {
        score += amount;
        if (scoreText) scoreText.text = "Candies: " + score;
    }

    public void GameStart()
    {
        score = 0;
        gameOverPanel.SetActive(false);
    }

    public void GameOver()
    {
        gameOverPanel.SetActive(true);
    }
}