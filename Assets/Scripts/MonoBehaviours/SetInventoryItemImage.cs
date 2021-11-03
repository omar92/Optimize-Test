using ScriptableSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class SetInventoryItemImage : MonoBehaviour
{
    [Header("So variables")]
    [Tooltip(tooltip: "SO to contain the parsed data.")]
    [SerializeField] private InventoryItemsDataSO ItemDatas;
    [Tooltip(tooltip: "SO Indicate current selected item.")]
    [SerializeField] private IntSO selectedItemIndex;

    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void SetImage()
    {
        image.sprite = ItemDatas.Value[selectedItemIndex.Value].sprite;
    }

}
