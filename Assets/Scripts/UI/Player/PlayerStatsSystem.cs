using System.IO;
using TMPro;
using UnityEngine;

[System.Serializable]
public class PlayerStatsSaveData
{
    public int totalCoinsCollected;
    public int totalCaramelCollected;
    public int totalCandiesCollected;
    public float totalDistanceTraveled;
    public int totalDeaths;
    public int totalQuestsCompleted;
    public int totalGameLaunches;
    public int totalMatchesPlayed;
    public int totalJumps;
    public int totalSlides;
    public int bestRunCoins;
    public int bestRunCaramel;
    public int bestRunCandies;
    public float bestRunDistance;
    public int bestRunJumps;
    public int bestRunSlides;
}

public class PlayerStatsSystem : MonoBehaviour
{
    public static PlayerStatsSystem Instance;

    private const string SAVE_FILE = "player_stats.json";

    [Header("Records (Best per match)")]
    public int _bestRunCoins;
    public TMP_Text _bestRunCoinsText;
    public int _bestRunCaramel;
    public TMP_Text _bestRunCaramelText;
    public int _bestRunCandies;
    public TMP_Text _bestRunCandiesText;
    public int _bestRunDistance;
    public TMP_Text _bestRunDistanceText;
    public int _bestRunJumps;
    public TMP_Text _bestRunJumpsText;
    public int _bestRunSlides;
    public TMP_Text _bestRunSlidesText;

    [Header("Lifetime Stats")]
    public int _totalCoinsCollected;
    public TMP_Text _totalCoinsCollectedText;
    public int _totalCaramelCollected;
    public TMP_Text _totalCaramelCollectedText;
    public int _totalCandiesCollected;
    public TMP_Text _totalCandiesCollectedText;
    public int _totalDistanceTraveled;
    public TMP_Text _totalDistanceTraveledText;
    public int _totalDeaths;
    public TMP_Text _totalDeathsText;
    public int _totalQuestsCompleted;
    public TMP_Text _totalQuestsCompletedText;
    public int _totalGameLaunches;
    public TMP_Text _totalGameLaunchesText;
    public int _totalMatchesPlayed;
    public TMP_Text _totalMatchesPlayedText;
    public int _totalJumps;
    public TMP_Text _totalJumpsText;
    public int _totalSlides;
    public TMP_Text _totalSlidesText;

    [Header("Other")]
    private int _runCoins;
    private int _runCaramel;
    private int _runCandies;
    private int _runJumps;
    private int _runSlides;

    private string SavePath => Path.Combine(Application.persistentDataPath, SAVE_FILE);

    private void Awake() => Instance = this;

    private void Start()
    {
        Load();
        GameManager.Instance.OnStartGame += AddMatchPlayed;
        GameManager.Instance.OnStartGame += ResetRunStats;
        GameManager.Instance.OnGameOver += OnRunEnd;
        RefreshUI();

        _totalGameLaunches++;
        Save();
    }

    #region Collect

    public void AddCoins(int amount = 1)
    {
        _totalCoinsCollected += amount;
        _runCoins += amount;
        if (_runCoins > _bestRunCoins) _bestRunCoins = _runCoins;
        RefreshUI();
        Save();
    }

    public void AddCaramel(int amount = 1)
    {
        _totalCaramelCollected += amount;
        _runCaramel += amount;
        if (_runCaramel > _bestRunCaramel) _bestRunCaramel = _runCaramel;
        RefreshUI();
        Save();
    }

    public void AddCandy(int amount = 1)
    {
        _totalCandiesCollected += amount;
        _runCandies += amount;
        if (_runCandies > _bestRunCandies) _bestRunCandies = _runCandies;
        RefreshUI();
        Save();
    }

    #endregion

    #region Game

    public void AddMatchPlayed()
    {
        _totalMatchesPlayed++;
        RefreshUI();
        Save();
    }

    public void AddJump()
    {
        _totalJumps++;
        _runJumps++;
        if (_runJumps > _bestRunJumps) _bestRunJumps = _runJumps;
        RefreshUI();
        Save();
    }

    public void AddSlide()
    {
        _totalSlides++;
        _runSlides++;
        if (_runSlides > _bestRunSlides) _bestRunSlides = _runSlides;
        RefreshUI();
        Save();
    }

    public void AddDeath()
    {
        _totalDeaths++;
        RefreshUI();
        Save();
    }

    #endregion

    #region Distance

    public void OnRunEnd()
    {
        float runDistance = GameManager.Instance.GetPlayer.GetPlayerTransform.position.z;
        _totalDistanceTraveled += (int)runDistance;
        if (runDistance > _bestRunDistance) _bestRunDistance = (int)runDistance;
        RefreshUI();
        Save();
    }

    #endregion

    #region Quests

    public void AddQuestComplete()
    {
        _totalQuestsCompleted++;
        RefreshUI();
        Save();
    }

    #endregion

    #region Run

    public void ResetRunStats()
    {
        _runCoins = 0;
        _runCaramel = 0;
        _runCandies = 0;
        _runJumps = 0;
        _runSlides = 0;
    }

    #endregion

    #region Save / Load

    public void Save()
    {
        PlayerStatsSaveData data = new()
        {
            bestRunCoins = _bestRunCoins,
            bestRunCaramel = _bestRunCaramel,
            bestRunCandies = _bestRunCandies,
            bestRunDistance = _bestRunDistance,
            bestRunJumps = _bestRunJumps,
            bestRunSlides = _bestRunSlides,

            totalCoinsCollected = _totalCoinsCollected,
            totalCaramelCollected = _totalCaramelCollected,
            totalCandiesCollected = _totalCandiesCollected,
            totalDistanceTraveled = _totalDistanceTraveled,
            totalDeaths = _totalDeaths,
            totalQuestsCompleted = _totalQuestsCompleted,
            totalGameLaunches = _totalGameLaunches,
            totalMatchesPlayed = _totalMatchesPlayed,
            totalJumps = _totalJumps,
            totalSlides = _totalSlides,
        };
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
    }

    public void Load()
    {
        if (!File.Exists(SavePath)) return;
        string json = File.ReadAllText(SavePath);
        PlayerStatsSaveData data = JsonUtility.FromJson<PlayerStatsSaveData>(json);
        _bestRunCoins = data.bestRunCoins;
        _bestRunCaramel = data.bestRunCaramel;
        _bestRunCandies = data.bestRunCandies;
        _bestRunDistance = (int)data.bestRunDistance;
        _bestRunJumps = data.bestRunJumps;
        _bestRunSlides = data.bestRunSlides;

        _totalCoinsCollected = data.totalCoinsCollected;
        _totalCaramelCollected = data.totalCaramelCollected;
        _totalCandiesCollected = data.totalCandiesCollected;
        _totalDistanceTraveled = (int)data.totalDistanceTraveled;
        _totalDeaths = data.totalDeaths;
        _totalQuestsCompleted = data.totalQuestsCompleted;
        _totalGameLaunches = data.totalGameLaunches;
        _totalMatchesPlayed = data.totalMatchesPlayed;
        _totalJumps = data.totalJumps;
        _totalSlides = data.totalSlides;
    }

    #endregion

    #region UI

    private void RefreshUI()
    {
        if (_bestRunCoinsText) _bestRunCoinsText.text = $"Best amount of Coins: {_bestRunCoins}";
        if (_bestRunCaramelText) _bestRunCaramelText.text = $"Best amount of Caramel: {_bestRunCaramel}";
        if (_bestRunCandiesText) _bestRunCandiesText.text = $"Best amount of Candies: {_bestRunCandies}";
        if (_bestRunDistanceText) _bestRunDistanceText.text = $"Best Distance: {_bestRunDistance}";
        if (_bestRunJumpsText) _bestRunJumpsText.text = _totalDeathsText.text = $"Best amount of Jumps: {_bestRunJumps}";
        if (_bestRunSlidesText) _bestRunSlidesText.text = _totalDeathsText.text = $"Best amount of Slides: {_bestRunSlides}";

        if (_totalCoinsCollectedText) _totalCoinsCollectedText.text = $"Total Coins Collected: {_totalCoinsCollected}";
        if (_totalCaramelCollectedText) _totalCaramelCollectedText.text = $"Total Caramel Collected: {_totalCaramelCollected}";
        if (_totalCandiesCollectedText) _totalCandiesCollectedText.text = $"Total Candies Collected: {_totalCandiesCollected}";
        if (_totalDistanceTraveledText) _totalDistanceTraveledText.text = $"Total Distance Traveled: {_totalDistanceTraveled}";
        if (_totalDeathsText) _totalDeathsText.text = $"Total Deaths: {_totalDeaths}";
        if (_totalQuestsCompletedText) _totalQuestsCompletedText.text = $"Total Quests Completed: {_totalQuestsCompleted}";
        if (_totalGameLaunchesText) _totalGameLaunchesText.text = _totalDeathsText.text = $"Total Game Launches: {_totalGameLaunches}";
        if (_totalMatchesPlayedText) _totalMatchesPlayedText.text = _totalDeathsText.text = $"Total Matches Played: {_totalMatchesPlayed}";
        if (_totalJumpsText) _totalJumpsText.text = _totalDeathsText.text = $"Total Jumps: {_totalJumps}";
        if (_totalSlidesText) _totalSlidesText.text = _totalDeathsText.text = $"Total Slides: {_totalSlides}";
    }

    #endregion
}