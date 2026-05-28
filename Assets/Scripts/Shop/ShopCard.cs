using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopCard : MonoBehaviour
{
    [Header("Data")]
    public string itemId;

    [Header("UI")]
    public TMP_Text titleText;
    public Image icon;
    public TMP_Text priceText;
    public TMP_Text amountText;
    public TMP_Text buttonText;
    public Button buyButton;
    public Image buttonImage;
    public Image shadowImage;

    [Header("Colors")]
    public Color buyColor = Color.green;
    public Color ownedColor = Color.gray;

    public void Initialize(ShopItemData item)
    {
        itemId = item.id;
        titleText.text = item.title;
        icon.sprite = item.sprite;
        UpdateCard(item);
    }

    public void Buy()
    {
        ShopManager.Instance.Buy(itemId);
    }

    public void UpdateCard(ShopItemData item)
    {
        if (item.amount > 0)
        {
            buttonText.text = "OWNED";
            buyButton.interactable = false;
            buttonImage.color = ownedColor;
            shadowImage.color = ownedColor;
        }
        else
        {
            buttonText.text = "BUY";
            buyButton.interactable = true;
            buttonImage.color = buyColor;
            shadowImage.color = buyColor;
        }
        priceText.text = item.price.ToString();
        amountText.text = $"X{item.amount}";
    }
}