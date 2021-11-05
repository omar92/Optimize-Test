using ScriptableSystem;
using TMPro;
using UnityEngine;

public class InventoryInfoPanel : MonoBehaviour
{
    [Header("Info Panel components")]
    [SerializeField] private TextMeshProUGUI Name;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private TextMeshProUGUI stats;

    [Header("So variables")]
    [Tooltip(tooltip: "SO to contain the parsed data.")]
    [SerializeField] private InventoryItemsDataSO ItemDatas;
    [Tooltip(tooltip: "SO Indicate current selected item.")]
    [SerializeField] private IntSO selectedItemIndex;

    public void OnDataReady()
    {
        UpdateData();
    }
    public void OnItemSelected()
    {
        UpdateData();
    }

    private void UpdateData()
    {
        var InventoryItemsData = ItemDatas.Value[selectedItemIndex.Value];
        Name.text = InventoryItemsData.Name;
        description.text = InventoryItemsData.Description;
        stats.text = InventoryItemsData.Stat.ToString();
    }
}
