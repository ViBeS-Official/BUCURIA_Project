using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameInventoryUI : MonoBehaviour
{
    [Header("Currencies")]
    public TMP_Text _caramelAmountText;
    public TMP_Text _coinAmountText;

    [Header("Candies")]
    public GameObject panel;
    public Transform content;
    public GameObject cardPrefab;

    private List<GameObject> _spawnedCards = new();

    #region Currency

    public void UpdateCoins(int value)
    {
        if (_caramelAmountText) _caramelAmountText.text = value.ToString();
    }

    public void UpdateCaramels(int value)
    {
        if (_coinAmountText) _coinAmountText.text = value.ToString();
    }

    #endregion

    public void TogglePanel(bool active)
    {
        panel.SetActive(active);
        if (panel.activeSelf) Refresh();
    }

    public void Refresh()
    {
        Clear();
        if (GameInventory.Instance.candies == null || GameInventory.Instance.candies.Count == 0) return;
        foreach (CandyItem item in GameInventory.Instance.candies)
        {
            GameObject card = Instantiate(cardPrefab, content);
            CandyCardUI ui = card.GetComponent<CandyCardUI>();
            ui.Setup(item);
            _spawnedCards.Add(card);
        }
    }

    private void Clear()
    {
        foreach (GameObject obj in _spawnedCards) Destroy(obj);
        _spawnedCards.Clear();
    }
}