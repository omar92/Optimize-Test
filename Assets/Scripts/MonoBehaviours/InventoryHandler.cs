
using ScriptableSystem;
using UnityEngine;

public class InventoryHandler : MonoBehaviour
{
    [Tooltip(tooltip: "Refrence to the list optmizer tool.")]
    [SerializeField] private ListOptmizer InvintoryList;

    [Header("So variables")]
    [Tooltip(tooltip: "SO to contain the parsed data.")]
    [SerializeField] private InventoryItemsDataSO ItemDatas;
    private void FillTheList()
    {
        // populate items in the Scroll View with correct data.
        InvintoryList.PopulateList(ItemDatas.Value.Length, (i, item) =>
        {
            item.name = $"{i}:{ItemDatas.Value[i].Name}";
            var inventoryItem = item.GetComponent<InventoryItem>();
            inventoryItem.SetData(i);
        });
    }


    public void OnDataIsReady()
    {
        FillTheList();
    }

}
