using System;
using TMPro;
using UnityEngine;

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance;

    [Header("UI")]
    public GameObject panel;
    public TMP_Text messageText;

    private Action _onClose;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    public void Show(string message, Action onClose = null)
    {
        _onClose = onClose;
        messageText.text = message;
        panel.SetActive(true);
    }

    public void Close()
    {
        panel.SetActive(false);
        _onClose?.Invoke();
        _onClose = null;
    }
}