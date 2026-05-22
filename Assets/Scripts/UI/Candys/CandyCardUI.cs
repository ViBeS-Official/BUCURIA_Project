using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CandyCardUI : MonoBehaviour
{
    public Image icon;
    public TMP_Text nameText;
    public TMP_Text rarityText;
    public TMP_Text coinCostText;
    public TMP_Text amountText;

    public void Setup(CandyItem item)
    {
        icon.sprite = item.icon;
        nameText.text = item.name;
        rarityText.text = item.rarity.ToString();
        coinCostText.text = "Sell: " + item.coinValue;
        amountText.text = "x" + item.amount;
    }
}