using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using ScriptableSystem;

public class InventoryItem : MonoBehaviour
{
    [Header("Item Parts")]
    [SerializeField] private Image Background;
    [SerializeField] private Image Icon;
    [SerializeField] private TextMeshProUGUI Name;
    [SerializeField] private Button Button;

    [Header("So Variables")]
    [SerializeField] private IntSO selectedItemIndex;
    [SerializeField] private InventoryItemsDataSO inventoryItemsData;

    [Header("So Events")]
    [SerializeField] private EventSO OnItemsSeleccted;

    private int index;
    private bool isClicked;

    private void Start()
    {
        Button.onClick.RemoveAllListeners();
        Button.onClick.AddListener(() =>
        {
            OnClick();
        });
    }

    /// <summary>
    /// used to set item data
    /// </summary>
    /// <param name="index">The item order.</param>
    /// <param name="inventoryItemData">the Item Data.</param>
    /// <param name="sprite">the Item sprite.</param>
    public void SetData(int index)
    {
        this.index = index;
        Icon.sprite = inventoryItemsData.Value[index].sprite;
        Name.text = inventoryItemsData.Value[index].Name;
        SetClicked(index == selectedItemIndex.Value);
    }

    /// <summary>
    /// this function supposed to be called by so event when any item is clicked 
    /// </summary>
    public void OnItemClicekd()
    {
        if (index == selectedItemIndex.Value)
        {
            if (!isClicked)
            {
                SetClicked(true);
            }
        }
        else
        {
            if (isClicked)
            {
                SetClicked(false);
            }
        }
    }

    private void SetClicked(bool isClicked)
    {
        Background.color = isClicked ? Color.red : Color.white;
        this.isClicked = isClicked;
    }
    private void OnClick()
    {
        selectedItemIndex.Value = index;
        OnItemsSeleccted.Raise();
    }
}
