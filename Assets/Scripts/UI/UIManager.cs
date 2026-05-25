using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject _menuPanel;
    public GameObject _gamePanel;

    public TMP_Text _caramelScoreText;
    public int _caramelScore = 0;
    public TMP_Text _coinScoreText;
    public int _coinScore = 0;
    public TMP_Text _candiesScoreText;
    public int _candiesScore = 0;

    public TMP_Text _distanceText;

    public GameObject _gameOverPanel;

    private void Awake() => Instance = this;

    private void Update()
    {
        if (!GameManager.Instance || GameManager.Instance.GetPlayer == null) return;
        if (_distanceText) _distanceText.text = $"Distance: {(int)GameManager.Instance.GetPlayer.GetPlayerTransform.position.z} m";
    }

    public void AddScore(CollectibleType collectibleType, int amount)
    {
        switch (collectibleType)
        {
            case CollectibleType.Caramel:
                _caramelScore += amount;
                if (_caramelScoreText) _caramelScoreText.text = $"Caramels: {_caramelScore}";
                break;
            case CollectibleType.Coin:
                _coinScore += amount;
                if (_coinScoreText) _coinScoreText.text = $"Moneys: {_coinScore}";
                break;
            case CollectibleType.Candy:
                _candiesScore += amount;
                if (_candiesScoreText) _candiesScoreText.text = $"Candies: {_candiesScore}";
                break;
        }
    }

    public void GameStart()
    {
        _caramelScore = 0;
        if (_caramelScoreText) _caramelScoreText.text = $"Caramels: 0";
        _coinScore = 0;
        if (_coinScoreText) _coinScoreText.text = $"Coins: 0";
        _candiesScore = 0;
        if (_candiesScoreText) _candiesScoreText.text = $"Candies: 0";
        _gameOverPanel.SetActive(false);
        _menuPanel.SetActive(false);
        _gamePanel.SetActive(true);
    }

    public void GameOver()
    {
        GameInventory.Instance.AddCaramels(_caramelScore);
        GameInventory.Instance.AddCoins(_coinScore);
        _gameOverPanel.SetActive(true);
    }

    public void Menu()
    {
        _gamePanel.SetActive(false);
        _menuPanel.SetActive(true);
    }
}