using System.Collections.Generic;
using UnityEngine;

public class CandyInventoryUI : MonoBehaviour
{
    public GameObject panel;
    public Transform content;
    public GameObject cardPrefab;

    private List<GameObject> spawnedCards = new List<GameObject>();

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
            spawnedCards.Add(card);
        }
    }

    private void Clear()
    {
        foreach (GameObject obj in spawnedCards) Destroy(obj);
        spawnedCards.Clear();
    }
}