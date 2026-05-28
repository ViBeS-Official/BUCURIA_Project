using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ShopItemData
{
    public string id;
    public string title;
    public Sprite sprite;
    public int price;
    public int amount;
    public List<string> requiredItems = new();
}

[Serializable]
public class ShopSaveData
{
    public string id;
    public int amount;
}

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;

    [Header("Database")]
    public List<ShopItemData> items = new();

    [Header("Cards")]
    public ShopCard cardPrefab;
    public Transform container;
    public List<ShopCard> cards = new();

    private const string SAVE_KEY = "ShopData";

    private void Awake() => Instance = this;

    private void Start()
    {
        foreach (ShopItemData item in items)
        {
            ShopCard card = Instantiate(cardPrefab, container);
            card.Initialize(item);
            cards.Add(card);
        }
        Load();
        RefreshCards();
    }

    #region Buy

    public bool Buy(string id, int buyAmount = 1)
    {
        ShopItemData item = GetItem(id);
        if (item == null) return false;
        foreach (string requiredId in item.requiredItems) 
            if (!HasItem(requiredId))
            {
                PopupManager.Instance.Show($"Cannot purchase. You must buy {requiredId} item first.");
                return false;
            }
        int totalPrice = item.price * buyAmount;
        if (GameInventory.Instance.GetCoins() < totalPrice)
        {
            PopupManager.Instance.Show($"Not enough coins. You are missing {totalPrice - GameInventory.Instance.GetCoins()} coins.");
            return false;
        }
        GameInventory.Instance.ChangeCoins(-totalPrice);
        item.amount += buyAmount;
        Save();
        RefreshCards();
        return true;
    }

    #endregion

    #region Inventory

    public bool HasItem(string id)
    {
        ShopItemData item = GetItem(id);
        if (item == null) return false;
        return item.amount > 0;
    }

    public int GetAmount(string id)
    {
        ShopItemData item = GetItem(id);
        if (item == null) return 0;
        return item.amount;
    }
    public bool RemoveAmount(string id, int amount = 1)
    {
        ShopItemData item = GetItem(id);
        if (item == null) return false;
        if (item.amount < amount) return false;
        item.amount -= amount;
        Save();
        RefreshCards();
        return true;
    }

    public ShopItemData GetItem(string id)
    {
        return items.Find(x => x.id == id);
    }

    #endregion

    #region UI

    public void RefreshCards()
    {
        foreach (ShopCard card in cards)
        {
            ShopItemData item = GetItem(card.itemId);
            if (item == null) continue;
            card.UpdateCard(item);
        }
    }

    #endregion

    #region Save / Load

    public void Save()
    {
        List<ShopSaveData> save = new();
        foreach (ShopItemData item in items)
        {
            save.Add(new ShopSaveData()
            {
                id = item.id,
                amount = item.amount
            });
        }
        string json = JsonUtility.ToJson(new Serialization<ShopSaveData>(save), true);
        PlayerPrefs.SetString(SAVE_KEY, json);
    }

    public void Load()
    {
        if (!PlayerPrefs.HasKey(SAVE_KEY)) return;
        string json = PlayerPrefs.GetString(SAVE_KEY);
        Serialization<ShopSaveData> data = JsonUtility.FromJson<Serialization<ShopSaveData>>(json);
        foreach (ShopSaveData save in data.items)
        {
            ShopItemData item = GetItem(save.id);
            if (item == null) continue;
            item.amount = save.amount;
        }
    }

    #endregion

    [Serializable]
    public class Serialization<T>
    {
        public List<T> items;
        public Serialization(List<T> items) => this.items = items;
    }
}