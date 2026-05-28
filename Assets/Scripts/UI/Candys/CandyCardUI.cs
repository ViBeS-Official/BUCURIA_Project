using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CandyCardUI : MonoBehaviour
{
    private CandyItem _item;
    public Image shadow;
    public Image icon;
    public TMP_Text nameText;
    public TMP_Text rarityText;
    public TMP_Text coinCostText;
    public TMP_Text amountText;

    public void Setup(CandyItem item)
    {
        _item = item;
        icon.sprite = _item.icon;
        nameText.text = _item.name;
        rarityText.text = _item.rarity.ToString();
        coinCostText.text = _item.coinValue.ToString();;
        amountText.text = "x" + _item.amount;
        switch (_item.rarity)
        {
            case CandyRarity.Common:
                rarityText.color = Color.gray;
                shadow.color = Color.gray;
                break;
            case CandyRarity.Rare:
                rarityText.color = Color.blue;
                shadow.color = Color.blue;
                break;
            case CandyRarity.Epic:
                rarityText.color = Color.magenta;
                shadow.color = Color.magenta;
                break;
            case CandyRarity.Legendary:
                rarityText.color = Color.red;
                shadow.color = Color.red;
                break;
        }
    }

    public void Sell()
    {
        if (_item.amount <= 0)
        {
            PopupManager.Instance.Show("You don't have any candies to sell.");
            return;
        }
        GameInventory.Instance.AddCandy(_item.name, -1);
        GameInventory.Instance.ChangeCoins(_item.coinValue);
        GameInventory.Instance.Refresh();
    }
}