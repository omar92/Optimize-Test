using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(ScrollRect))]
public class ListOptmizer : MonoBehaviour
{
    #region parameters
    [Header("Item Template")]
    [Tooltip(tooltip: "refrence to the GameObject that will be used to fill the list.")]
    [SerializeField] private GameObject ItemTemplate;
    [Tooltip(tooltip: "refrence to the GameObject that will be used to fill the list.")]
    [SerializeField] private RectTransform content;
    [SerializeField] private Scrollbar scrollbarVertical;
    [Tooltip(tooltip: "Maximum number of inistances at the same time.")]

    private RectTransform rearFiller;
    private RectTransform frontFiller;
    private int startIndex = 0;
    private int endIndex = 0;
    private int oldStartIndex = -1;
    float ItemHeight = 0;
    float itemsSpacing = 0;
    float ViewportHeight = 0;
    int VisibleItemsNumber = 0;
    int totallItemsNumber = 0;
    float ContentHeight = 0;
    float itemHeighPercintage = 0;
    Action<int, GameObject> OnItemShow;
    Action<int, GameObject> OnItemHide;
    List<ItemIndexPair> activeItems;
    private bool isPopulated;
    class ItemIndexPair
    {
        public ItemIndexPair(int index, GameObject gameobject)
        {
            this.index = index;
            this.item = gameobject;
        }
        public int index;
        public GameObject item;
    }
    #endregion


    //#region MonoBehaviour
    private void Start()
    {
        scrollbarVertical.onValueChanged.AddListener(OnScrollValueChanged);
    }
    public void OnRectTransformDimensionsChange()
    {
        if (isPopulated)
        {
            OnSizeChange();
        }
    }

    //#endregion

    #region methods
    public void PopulateList(int itemsNumber, Action<int, GameObject> OnItemShow = null, Action<int, GameObject> OnItemHide = null)
    {
        this.totallItemsNumber = itemsNumber;
        this.OnItemShow = OnItemShow;
        this.OnItemHide = OnItemHide;
        ClearOldItems();
        CalculateMeasures();
        CreateVisibleItems();
        CreateFillers();
        startIndex = 0;
        oldStartIndex = -1;
        endIndex = VisibleItemsNumber - 1;
        ResizePanels();
        ExcutePoulateAction();
        isPopulated = true;
    }

    public void ForEachVisible(Action<int, GameObject> onItem)
    {
        foreach (var item in activeItems)
        {
            onItem(item.index, item.item);
        }
    }
    #endregion

    #region functions
    private void ExcutePoulateAction()
    {
        foreach (var pair in activeItems)
        {
            OnItemShow?.Invoke(pair.index, pair.item);
        }
    }

    private void OnScrollValueChanged(float value)
    {
        if (isPopulated)
        {
            UpdateStartAndEnd(value);
            ResizePanels();
            ReSortItems();
            oldStartIndex = startIndex;
        }
    }
    private void OnSizeChange() //TODO: inhance
    {
        ClearOldItems();
        CalculateMeasures();
        CreateVisibleItems();
        CreateFillers();
        oldStartIndex = -1;
        endIndex = startIndex + VisibleItemsNumber - 1;
        ExcutePoulateAction();
        //MoveScrollbarToSelected(); //if not used the scroll will return to start when size change 
    }

    private void MoveScrollbarToSelected()//TODO: fix as it go to wrong position
    {
        var startPercintage = itemHeighPercintage/100 * startIndex;
        scrollbarVertical.value = 1- startPercintage;
    }

    private void ReSortItems()//TODO: inhance
    {
        if (startIndex != oldStartIndex)//need to be optmized
        {
            for (int i = 0; i < activeItems.Count; i++)
            {
                this.OnItemHide?.Invoke(activeItems[i].index, activeItems[i].item);
                activeItems[i].index = startIndex + i;
                this.OnItemShow?.Invoke(activeItems[i].index, activeItems[i].item);
            }
        }
    }
    private void UpdateStartAndEnd(float value)
    {
        startIndex = Mathf.FloorToInt((100 - value * 100) / itemHeighPercintage);
        endIndex = startIndex + VisibleItemsNumber - 1;
        if (startIndex < 0)
        {
            startIndex = 0;
            endIndex = VisibleItemsNumber;
        }
        if (endIndex >= totallItemsNumber)
        {
            startIndex = totallItemsNumber - VisibleItemsNumber;
            endIndex = totallItemsNumber;
        }
    }
    private void ResizePanels()
    {
        var rect = rearFiller.rect;
        var height = startIndex * (ItemHeight + itemsSpacing) - itemsSpacing; //the space before the visble items
        rearFiller.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height); //.sert rect.Set(rect.x, rect.y, space, space);

        rect = frontFiller.rect;
        height = (totallItemsNumber - endIndex) * (ItemHeight + itemsSpacing) - itemsSpacing; //the space after the visble items
        frontFiller.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);

        LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());
    }


    private void CreateFillers()
    {
        var newGameObject = new GameObject();
        rearFiller = newGameObject.AddComponent<RectTransform>();
        rearFiller.name = "rearFiller";
        rearFiller.SetParent(content);
        rearFiller.SetAsFirstSibling();

        newGameObject = new GameObject();
        frontFiller = newGameObject.AddComponent<RectTransform>();
        frontFiller.name = "frontFiller";
        frontFiller.SetParent(content);
        frontFiller.SetAsLastSibling();
    }

    private void ClearOldItems()
    {
        for (int i = 0; i < content.childCount; i++)
        {
            if (ItemTemplate != content.GetChild(i).gameObject)
            {
                Destroy(content.GetChild(i).gameObject);
            }
        }
    }

    private void CalculateMeasures()
    {
        ItemHeight = ItemTemplate.GetComponent<RectTransform>().rect.height;

        VerticalLayoutGroup vlg = content.GetComponent<VerticalLayoutGroup>();
        if (vlg)
        {
            itemsSpacing = vlg.padding.vertical;
        }

        ViewportHeight = this.GetComponent<RectTransform>().rect.height;

        VisibleItemsNumber = (int)(ViewportHeight / (ItemHeight + itemsSpacing));

        ContentHeight = totallItemsNumber * (ItemHeight + itemsSpacing) - itemsSpacing;

        itemHeighPercintage = (100 * ItemHeight) / (ContentHeight - (ItemHeight * VisibleItemsNumber));

        VisibleItemsNumber += 2;
    }

    private void CreateVisibleItems()
    {
        activeItems = new List<ItemIndexPair>();
        ItemTemplate.SetActive(true);
        for (int i = 0; i < VisibleItemsNumber; i++)
        {
            activeItems.Add(new ItemIndexPair(i, GameObject.Instantiate(ItemTemplate, content)));
        }
        ItemTemplate.SetActive(false);
    }
    #endregion

}
