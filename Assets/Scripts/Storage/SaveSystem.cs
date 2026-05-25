using System.IO;
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    public int coins;
    public int caramel;

    public List<CandySaveData> candies = new();
}

[System.Serializable]
public class CandySaveData
{
    public string name;
    public int amount;
}

public static class SaveSystem
{
    private static string SavePath => Path.Combine(Application.persistentDataPath, "save.json");

    public static void Save()
    {
        if (GameInventory.Instance == null) return;
        SaveData data = new()
        {
            coins = GameInventory.Instance.coins,
            caramel = GameInventory.Instance.caramels,
        };
        foreach (CandyItem item in GameInventory.Instance.candies)
        {
            data.candies.Add(new CandySaveData
            {
                name = item.name,
                amount = item.amount
            });
        }
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
    }

    public static void Load()
    {
        if (!File.Exists(SavePath)) return;
        string json = File.ReadAllText(SavePath);
        SaveData data = JsonUtility.FromJson<SaveData>(json);
        GameInventory.Instance.SetCoins(data.coins);
        GameInventory.Instance.SetCaramels(data.caramel);
        foreach (CandySaveData saveCandy in data.candies)
        {
            CandyItem candy = GameInventory.Instance.GetCandy(saveCandy.name);
            if (candy != null) candy.amount = saveCandy.amount;
        }
    }
}