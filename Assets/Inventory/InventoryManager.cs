using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
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

    private List<InventoryItem> Items;

    void Start()
    {
        ItemDatas = GenerateItemDatas(ItemJson, ItemGenerateScale);

        // Instantiate items in the Scroll View.
        InvintoryList.PopulateList(ItemDatas.Length, (i, item) =>
        {
            var inventoryItem = item.GetComponent<InventoryItem>();
            inventoryItem.Icon.sprite = Icons[ItemDatas[i].IconIndex];
            inventoryItem.Name.text = ItemDatas[i].Name;
            inventoryItem.Button.onClick.AddListener(() => { InventoryItemOnClick(inventoryItem, ItemDatas[i]); });
        });

        // Select the first item.
        InventoryItemOnClick(InvintoryList.activeItems[0].item.GetComponent<InventoryItem>(), ItemDatas[0]);
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

    private void InventoryItemOnClick(InventoryItem itemClicked, InventoryItemData itemData)
    {   //just to complete cycle for now 
        foreach (var item in InvintoryList.activeItems)
        {
            var inventoryItem = item.item.GetComponent<InventoryItem>();
            inventoryItem.Background.color = Color.white;
        }
        itemClicked.Background.color = Color.red;
    }
}
