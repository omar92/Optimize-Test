using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class InventoryItem : MonoBehaviour
{
    public Image Background;
    public Image Icon;
    public TextMeshProUGUI Name;
    public Button Button;

    internal void SetClicked(bool isClicked)
    {
        Background.color = isClicked ? Color.red : Color.white;
    }
}
