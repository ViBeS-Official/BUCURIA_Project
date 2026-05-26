using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestUI : MonoBehaviour
{
    public TMP_Text titleText;
    public TMP_Text progressText;
    public TMP_Text rewardText;

    [Header("Tracking")]
    public Image background;

    public Color normalColor = Color.black;
    public Color trackedColor = Color.black;

    private ActiveQuest _quest;
    public ActiveQuest GetQuest => _quest;  

    public void Setup(ActiveQuest quest)
    {
        _quest = quest;
        Refresh();
    }

    public void Refresh()
    {
        if (_quest == null) return;
        titleText.text = _quest.data.GetDescription();
        progressText.text = $"{_quest.progress}/{_quest.data.targetAmount}";
        rewardText.text = $"{_quest.data.rewardCoins}";
        UpdateTrackingVisual();
    }

    public void TrackQuest()
    {
        QuestSystem.Instance.SetTrackedQuest(_quest);
    }

    public void UpdateTrackingVisual()
    {
        if (!background) return;
        bool tracked = QuestSystem.Instance.trackedQuest == _quest;
        background.color = tracked ? trackedColor : normalColor;
    }
}