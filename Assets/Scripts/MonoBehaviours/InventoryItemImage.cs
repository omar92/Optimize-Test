using ScriptableSystem;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class InventoryItemImage : MonoBehaviour
{
    [Header("So variables")]
    [Tooltip(tooltip: "SO to contain the parsed data.")]
    [SerializeField] private InventoryItemsDataSO ItemDatas;
    [Tooltip(tooltip: "SO Indicate current selected item.")]
    [SerializeField] private IntSO selectedItemIndex;
    private Image image;

    public void OnDataReady()
    {
        UpdateImage();
    }
    public void OnItemSelected()
    {
        UpdateImage();
    }

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    private void UpdateImage()
    {
        image.sprite = ItemDatas.Value[selectedItemIndex.Value].sprite;
    }
}
