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

    [Header("Currencies")]
    public int coins;
    public int caramels;

    [Header("Candy Inventory")]
    public List<CandyItem> candies = new();

    private void Awake() => Instance = this;

    #region Currency

    public void AddCoins(int value)
    {
        coins += value;
        if (coins < 0) coins = 0;
    }

    public void AddCaramels(int value)
    {
        caramels += value;
        if (caramels < 0) caramels = 0;
    }

    #endregion

    #region Candy System

    public void AddCandy(string name, Sprite icon, int value, CandyRarity rarity, int amount = 1)
    {
        CandyItem existing = GetCandy(name);
        if (existing != null)
        {
            existing.amount += amount;
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

    public CandyItem GetCandy(string name) => candies.Find(c => c.name == name);
    public int GetCandyAmount(string name)
    {
        CandyItem candy = GetCandy(name);
        return candy != null ? candy.amount : 0;
    }

    #endregion
}