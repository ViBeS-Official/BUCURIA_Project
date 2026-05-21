using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject _menuPanel;
    public GameObject _gamePanel;

    public TMP_Text _scoreText;
    public int _score = 0;
    public TMP_Text _distanceText;

    public GameObject _gameOverPanel;

    private void Awake() => Instance = this;

    private void Update()
    {
        if (!GameManager.Instance || GameManager.Instance.GetPlayer == null) return;
        if (_distanceText) _distanceText.text = $"{(int)GameManager.Instance.GetPlayer.GetPlayerTransform.position.z} m";
    }

    public void AddScore(int amount)
    {
        _score += amount;
        if (_scoreText) _scoreText.text = $"Candies: {_score}";
    }

    public void GameStart()
    {
        _score = 0;
        if (_scoreText) _scoreText.text = $"Candies: 0";
        _gameOverPanel.SetActive(false);
        _menuPanel.SetActive(false);
        _gamePanel.SetActive(true);
    }

    public void GameOver()
    {
        _gameOverPanel.SetActive(true);
    }

    public void Menu()
    {
        _gamePanel.SetActive(false);
        _menuPanel.SetActive(true);
    }
}