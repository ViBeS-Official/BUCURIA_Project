using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CandyItem
{
    public string name;
    public Sprite icon;
    public int coinValue;
    public CandyRarity rarity;
    public int amount;
}

public enum CandyRarity
{
    Common,
    Rare,
    Epic,
    Legendary
}

public class GameInventory : MonoBehaviour
{
    public static GameInventory Instance;
    private GameInventoryUI _gameInventoryUI;

    [Header("Currencies")]
    public int coins;
    public int caramels;

    [Header("Candy Inventory")]
    public List<CandyItem> candies = new();

    private void Awake() => Instance = this;

    void Start()
    {
        _gameInventoryUI = FindObjectOfType<GameInventoryUI>();
        SaveSystem.Load();
    }

    private void OnApplicationQuit()
    {
        SaveSystem.Save();
    }

    #region Currency

    public void ChangeCoins(int value = 1)
    {
        coins += value;
        if (coins < 0) coins = 0;
        if (_gameInventoryUI) _gameInventoryUI.UpdateCaramels(coins);
    }
    public void SetCoins(int value)
    {
        coins = value;
        if (coins < 0) coins = 0;
        if (_gameInventoryUI) _gameInventoryUI.UpdateCaramels(coins);
    }
    public int GetCoins() => coins;

    public void AddCaramels(int value = 1)
    {
        caramels += value;
        if (caramels < 0) caramels = 0;
        if (_gameInventoryUI) _gameInventoryUI.UpdateCoins(caramels);
    }
    public void SetCaramels(int value)
    {
        caramels = value;
        if (caramels < 0) caramels = 0;
        if (_gameInventoryUI) _gameInventoryUI.UpdateCoins(caramels);
    }

    #endregion

    #region Candy System

    public void AddCandy(string name, Sprite icon, int value, CandyRarity rarity, int amount = 1)
    {
        CandyItem existing = GetCandy(name);
        if (existing != null)
        {
            existing.amount += amount;
            if (existing.amount < 0) existing.amount = 0;
            return;
        }
        candies.Add(new CandyItem
        {
            name = name,
            icon = icon,
            coinValue = value,
            rarity = rarity,
            amount = amount
        });
    }
    public void AddCandy(string name, int amount = 1)
    {
        CandyItem existing = GetCandy(name);
        if (existing != null)
        {
            existing.amount += amount;
            if (existing.amount < 0) existing.amount = 0;
        }
    }

    public CandyItem GetCandy(string name) => candies.Find(c => c.name == name);
    public int GetCandyAmount(string name)
    {
        CandyItem candy = GetCandy(name);
        return candy != null ? candy.amount : 0;
    }

    #endregion

    public void Refresh()
    {
        _gameInventoryUI.TogglePanel(true);
    }
}