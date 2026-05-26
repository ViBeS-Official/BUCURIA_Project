using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

[Serializable]
public class QuestData
{
    public string id;
    public QuestType type;
    public string candyName;
    public int targetAmount;
    public int rewardCoins;
    public bool resetOnRestart = false;

    public string GetDescription()
    {
        string description = type switch
        {
            QuestType.CollectCoins => $"Collect {targetAmount} Coins",
            QuestType.CollectCaramel => $"Collect {targetAmount} Caramel",
            QuestType.CollectCandy => $"Collect {targetAmount} {candyName}",
            QuestType.RunDistance => $"Run {targetAmount} meters",
            _ => id,
        };
        if (resetOnRestart) description += " in one match";
        return description;
    }
}

[Serializable]
public class ActiveQuest
{
    public QuestData data;
    public int progress;
    public bool completed;
    public float startDistance;
}

[Serializable]
public class QuestSaveData
{
    public string questId;
    public int progress;
    public bool completed;
}

[Serializable]
public class QuestSystemSave
{
    public List<QuestSaveData> quests = new();
    public string trackedQuestId;
}

public enum QuestType
{
    CollectCoins,
    CollectCaramel,
    CollectCandy,
    RunDistance
}

public class QuestSystem : MonoBehaviour
{
    public bool CompleteCurrentQuest;
    public bool RegenerateQuests;

    public static QuestSystem Instance;

    [Header("Quest Database")]
    public List<QuestData> questDatabase = new();

    [Header("UI Slots")]
    public QuestUI[] questSlots;

    [Header("Tracked Quest UI")]
    public TMP_Text trackedTitleText;
    public TMP_Text trackedProgressText;

    [Header("Runtime")]
    public List<ActiveQuest> activeQuests = new();
    public ActiveQuest trackedQuest;
    private string SavePath => Path.Combine(Application.persistentDataPath, "quests.json");

    private void Awake() => Instance = this;

    private void Start()
    {
        Load();
        if (activeQuests.Count <= 0) GenerateQuests();
        if (trackedQuest == null && activeQuests.Count > 0) SetTrackedQuest(activeQuests[0]);
        RefreshAllUI();
        RefreshTrackingVisuals();
        GameManager.Instance.OnStartGame += HandleRestart;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance) GameManager.Instance.OnStartGame -= HandleRestart;
        Save();
    }

    private void Update()
    {
        UpdateDistanceQuests();
        RefreshTrackedUI();

        if (CompleteCurrentQuest)
        {
            CompleteCurrentQuest = false;
            CompleteQuest(trackedQuest);
        }
        if (RegenerateQuests)
        {
            RegenerateQuests = false;
            GenerateQuests();
        }
    }

    #region Generate

    private void GenerateQuests()
    {
        activeQuests.Clear();
        List<QuestData> available = new(questDatabase);
        for (int i = 0; i < questSlots.Length; i++)
        {
            if (available.Count <= 0) break;
            int randomIndex = UnityEngine.Random.Range(0, available.Count);
            QuestData selected = available[randomIndex];
            available.RemoveAt(randomIndex);
            ActiveQuest quest = new()
            {
                data = selected,
                progress = 0,
                completed = false,
                startDistance = GameManager.Instance.GetPlayer.GetPlayerTransform.position.z
            };
            activeQuests.Add(quest);
        }
        SetTrackedQuest(activeQuests[0]);
        RefreshAllUI();
        RefreshTrackingVisuals();
    }

    #endregion

    #region Progress

    public void AddProgress(QuestType type, int amount = 1, string targetId = "")
    {
        foreach (ActiveQuest quest in activeQuests)
        {
            if (quest.completed) continue;
            if (quest.data.type != type) continue;
            switch (type)
            {
                case QuestType.CollectCoins:
                    quest.progress += amount;
                    break;
                case QuestType.CollectCaramel:
                    quest.progress += amount;
                    break;
                case QuestType.CollectCandy:
                    if (quest.data.candyName == targetId) quest.progress += amount;
                    break;
            }
            CheckQuest(quest);
        }
        RefreshAllUI();
        Save();
    }

    private void UpdateDistanceQuests()
    {
        foreach (ActiveQuest quest in activeQuests)
        {
            if (quest.completed) continue;
            if (quest.data.type != QuestType.RunDistance) continue;
            float currentDistance = GameManager.Instance.GetPlayer.GetPlayerTransform.position.z;
            int distance = Mathf.FloorToInt(currentDistance - quest.startDistance);
            if (distance <= 0) continue;
            if (quest.data.resetOnRestart) quest.progress = distance;
            else
            {
                quest.progress += distance;
                quest.startDistance = currentDistance;
            }
            CheckQuest(quest);
        }
    }

    #endregion

    #region Complete

    private void CheckQuest(ActiveQuest quest)
    {
        if (quest.progress < quest.data.targetAmount) return;
        CompleteQuest(quest);
    }

    private void CompleteQuest(ActiveQuest quest)
    {
        quest.completed = true;
        GameInventory.Instance.AddCoins(quest.data.rewardCoins);
        PlayerStatsSystem.Instance.AddQuestComplete();
        ReplaceQuest(quest);
        RefreshAllUI();
        Save();
    }

    private void ReplaceQuest(ActiveQuest oldQuest)
    {
        List<QuestData> available = new();
        foreach (QuestData data in questDatabase)
        {
            bool alreadyExists = activeQuests.Exists(q => q.data.id == data.id && !q.completed);
            if (!alreadyExists) available.Add(data);
        }
        if (available.Count <= 0) return;
        int randomIndex = UnityEngine.Random.Range(0, available.Count);
        QuestData selected = available[randomIndex];
        oldQuest.data = selected;
        oldQuest.progress = 0;
        oldQuest.completed = false;
        oldQuest.startDistance = GameManager.Instance.GetPlayer.GetPlayerTransform.position.z;
        if (trackedQuest == oldQuest) SetTrackedQuest(oldQuest);
    }

    #endregion

    #region Tracking

    public void SetTrackedQuest(ActiveQuest quest)
    {
        trackedQuest = quest;
        RefreshTrackedUI();
        RefreshTrackingVisuals();
        Save();
    }

    private void RefreshTrackedUI()
    {
        if (trackedQuest == null) return;
        if (trackedTitleText) trackedTitleText.text = trackedQuest.data.GetDescription();
        if (trackedProgressText) trackedProgressText.text = $"{trackedQuest.progress}/{trackedQuest.data.targetAmount}";
    }

    private void RefreshTrackingVisuals()
    {
        foreach (QuestUI ui in questSlots)
        {
            if (ui == null) continue;
            ui.UpdateTrackingVisual();
        }
    }

    #endregion

    #region Restart

    private void HandleRestart()
    {
        foreach (ActiveQuest quest in activeQuests)
        {
            if (quest.data.resetOnRestart) quest.progress = 0;
            quest.startDistance = GameManager.Instance.GetPlayer.GetPlayerTransform.position.z;
        }
        RefreshAllUI();
        Save();
    }

    #endregion

    #region UI

    private void RefreshAllUI()
    {
        for (int i = 0; i < questSlots.Length; i++)
        {
            if (i >= activeQuests.Count) continue;
            questSlots[i].Setup(activeQuests[i]);
        }
    }

    #endregion

    #region Save / Load

    public void Save()
    {
        QuestSystemSave save = new();
        foreach (ActiveQuest quest in activeQuests)
        {
            save.quests.Add(new QuestSaveData
            {
                questId = quest.data.id,
                progress = quest.progress,
                completed = quest.completed
            }
            );
        }
        if (trackedQuest != null) save.trackedQuestId = trackedQuest.data.id;
        string json = JsonUtility.ToJson(save, true);
        File.WriteAllText(SavePath, json);
    }

    public void Load()
    {
        if (!File.Exists(SavePath)) return;
        string json = File.ReadAllText(SavePath);
        QuestSystemSave save = JsonUtility.FromJson<QuestSystemSave>(json);
        activeQuests.Clear();
        foreach (QuestSaveData saveData in save.quests)
        {
            QuestData data = questDatabase.Find(q => q.id == saveData.questId);
            if (data == null) continue;
            ActiveQuest quest = new()
            {
                data = data,
                progress = saveData.progress,
                completed = saveData.completed,
                startDistance = GameManager.Instance.GetPlayer.GetPlayerTransform.position.z
            };
            activeQuests.Add(quest);
            if (save.trackedQuestId == data.id) trackedQuest = quest;
        }
    }

    #endregion
}