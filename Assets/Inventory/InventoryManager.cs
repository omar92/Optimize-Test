using System;
using UnityEngine;
using ScriptableSystem;

public class InventoryManager : MonoBehaviour
{

    [Header("data settings")]
    [Tooltip(tooltip: "Loads the list using this format.")]
    [Multiline]
    [SerializeField] private string ItemJson;

    [Tooltip(tooltip: "This is used in generating the items list. The number of additional copies to concat the list parsed from ItemJson.")]
    [SerializeField] private int ItemGenerateScale = 10;

    [Tooltip(tooltip: "Icons referenced by ItemData.IconIndex when instantiating new items.")]
    [SerializeField] private Sprite[] Icons;

    [Header("So variables")]
    [Tooltip(tooltip: "SO to contain the parsed data.")]
    [SerializeField] private InventoryItemsDataSO ItemDatas;
    [Tooltip(tooltip: "SO Indicate current selected item.")]
    [SerializeField] private IntSO selectedItemIndex;


    [Header("So Events")]
    [Tooltip(tooltip: "Event to indicate data is ready.")]
    [SerializeField] private EventSO OnDataIsReady;

    [Serializable]
    private class InventoryItemDatas
    {
        public InventoryItemData[] ItemDatas;
    }


    void Start()
    {
        InitData();
    }

    private void InitData()
    {
        ItemDatas.Value = GenerateItemDatas(ItemJson, ItemGenerateScale);
        selectedItemIndex.Value = 0; 
        OnDataIsReady.Raise();
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
            itemDatas[i].sprite = Icons[itemDatas[i].IconIndex];
            for (var j = 0; j < scale; j++)
            {
                finalItemDatas[i + j * itemDatas.Length] = itemDatas[i];
            }
        }

        return finalItemDatas;
    }

}
