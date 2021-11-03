using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [Tooltip(tooltip: "Refrence to the list optmizer tool.")]
    public ListOptmizer InvintoryList;

    [Tooltip(tooltip: "Loads the list using this format.")]
    [Multiline]
    public string ItemJson;

    [Tooltip(tooltip: "This is used in generating the items list. The number of additional copies to concat the list parsed from ItemJson.")]
    public int ItemGenerateScale = 10;

    [Tooltip(tooltip: "Icons referenced by ItemData.IconIndex when instantiating new items.")]
    public Sprite[] Icons;

    [Serializable]
    private class InventoryItemDatas
    {
        public InventoryItemData[] ItemDatas;
    }
    private InventoryItemData[] ItemDatas;

    private int selectedItemIndex = 0;

    void Start()
    {
        ItemDatas = GenerateItemDatas(ItemJson, ItemGenerateScale);

        // populate items in the Scroll View with correct data.
        InvintoryList.PopulateList(ItemDatas.Length, (i, item) =>
        {
            var inventoryItem = item.GetComponent<InventoryItem>();
            inventoryItem.Icon.sprite = Icons[ItemDatas[i].IconIndex];
            inventoryItem.Name.text = ItemDatas[i].Name;
            inventoryItem.Button.onClick.AddListener(() => { SelectInventoryItem(i); });

            if (i == selectedItemIndex)//set selected if it was the last selected item
            {
                inventoryItem.SetClicked(true);
            }

        }, (i, item) =>//reset the item to defult state so it can be used again for another item
        {
            var inventoryItem = item.GetComponent<InventoryItem>();
            inventoryItem.Button.onClick.RemoveAllListeners();
            inventoryItem.SetClicked(false);
        });
    }

    /// <summary>
    /// Generates an item list.
    /// </summary>
    /// <param name="json">JSON to generate items from. JSON must be an array of InventoryItemData.</param>
    /// <param name="scale">Concats additional copies of the array parsed from json.</param>
    /// <returns>An array of InventoryItemData</returns>
    private InventoryItemData[] GenerateItemDatas(string json, int scale)
    {
        var itemDatas = JsonUtility.FromJson<InventoryItemDatas>(json).ItemDatas;
        var finalItemDatas = new InventoryItemData[itemDatas.Length * scale];
        for (var i = 0; i < itemDatas.Length; i++)
        {
            for (var j = 0; j < scale; j++)
            {
                finalItemDatas[i + j * itemDatas.Length] = itemDatas[i];
            }
        }

        return finalItemDatas;
    }


    /// <summary>
    /// select item from inventory acording to index.
    /// </summary>
    /// <param name="index">the item index from the data.</param>
    private void SelectInventoryItem(int index)
    {
        InvintoryList.ForEachVisible((i, item) =>
        {
            if (i == selectedItemIndex)
            {
                var inventoryItem = item.GetComponent<InventoryItem>();//used GetComponent inside if to reduce the search operations
                inventoryItem.SetClicked(false);

            }
            else if (i == index)
            {
                var inventoryItem = item.GetComponent<InventoryItem>();//used GetComponent inside if to reduce the search operations
                inventoryItem.SetClicked(true);
            }
        });
        selectedItemIndex = index;
    }


}
