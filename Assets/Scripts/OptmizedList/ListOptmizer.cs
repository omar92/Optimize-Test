using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(ScrollRect))]
public class ListOptmizer : MonoBehaviour
{
    #region parameters
    [Header("Item Template")]
    [Tooltip(tooltip: "reference to the GameObject that will be used to fill the list.")]
    [SerializeField] private GameObject ItemTemplate;
    [Header("Scrollbar")]
    private ScrollRect scrollRect;
    private Scrollbar scrollbarVertical;
    private Scrollbar scrollbarHorizintal;
    private RectTransform content;
    #endregion

    #region variables
    private bool isHorizintal = true;
    private RectTransform rearFiller;
    private RectTransform frontFiller;
    private int startIndex = 0;
    private int endIndex = 0;
    private int oldStartIndex = -1;
    private float itemsSpacing = 0;

    private float ItemLength = 0;
    private float ContentLength = 0;
    private float itemLengthPercintage = 0;
    private float ViewportLength = 0;

    private int VisibleItemsNumber = 0;
    private int totallItemsNumber = 0;
    private Action<int, GameObject> OnItemShow;
    private Action<int, GameObject> OnItemHide;
    private List<ItemIndexPair> activeItems;
    private bool isPopulated;
    #endregion

    /// <summary>
    /// Used to link the item with its index
    /// </summary>
    class ItemIndexPair
    {
        public ItemIndexPair(int index, GameObject gameobject)
        {
            this.index = index;
            this.item = gameobject;
            optmizedItem = item.GetComponent<OptmizedListItem>();
        }
        public int index;
        public GameObject item;
        public OptmizedListItem optmizedItem;
    }

    #region MonoBehaviour
    private void Start()
    {
        if (scrollRect == null)
        {
            scrollRect = GetComponent<ScrollRect>();
        }
        if (scrollbarVertical == null)
        {
            scrollbarVertical = scrollRect.verticalScrollbar;
        }
        if (scrollbarHorizintal == null)
        {
            scrollbarHorizintal = scrollRect.horizontalScrollbar;
        }
        if (content == null)
        {
            content = scrollRect.content;
        }
        if (ItemTemplate == null)
        {
            Debug.LogError("ItemTemplate is null");
            return;
        }
        if (scrollbarHorizintal != null)
            scrollbarHorizintal.onValueChanged.AddListener(OnScrollValueChanged);
        if (scrollbarVertical != null)
            scrollbarVertical.onValueChanged.AddListener(OnScrollValueChanged);
    }

    /// <summary>
    /// unity event to detect UI size changes
    /// </summary>
    public void OnRectTransformDimensionsChange()
    {
        if (isPopulated)
        {
            OnSizeChange();
        }
    }
    #endregion
    #region methods

    private void Awake()
    {
        ItemTemplate.gameObject.SetActive(false);
    }
    /// <summary>
    /// Used to build the list items
    /// </summary>
    /// <param name="itemsNumber"> the list length</param>
    /// <param name="OnItemShow">callback that is invoked when item show up</param>
    /// <param name="OnItemHide">callback that is invoked when item disappear</param>
    public void PopulateList(int itemsNumber, Action<int, GameObject> OnItemShow = null, Action<int, GameObject> OnItemHide = null)
    {
        ItemTemplate.gameObject.SetActive(true);
        Z.InvokeEndOfFrame(() =>
        {
            isHorizintal = scrollRect.horizontal;
            if (isHorizintal && scrollRect.vertical)
            {
                Debug.LogError("You can't have both horizontal and vertical enabled\n vertical will be disabled");
                scrollRect.vertical = false;
            }

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
            ResizeFillers();
            ExcuteOnItemShowAction();
            isPopulated = true;
            ItemTemplate.gameObject.SetActive(false);
        });
    }

    /// <summary>
    /// used to iterate over current visible items
    /// </summary>
    /// <param name="onItem">callback that is invoked bet item</param>
    public void ForEachVisible(Action<int, GameObject> onItem)
    {
        foreach (var item in activeItems)
        {
            onItem(item.index, item.item);
        }
    }
    #endregion

    #region functions

    /// <summary>
    /// invoke OnItemShow callback for all the visible items
    /// </summary>
    private void ExcuteOnItemShowAction()
    {
        foreach (var pair in activeItems)
        {
            InvokeItemShowCallback(pair);
        }
    }



    /// <summary>
    /// is fired when the scrollbar value changes
    /// </summary>
    /// <param name="value">current scroll bar value</param>
    private void OnScrollValueChanged(float value)
    {
        if (isPopulated)
        {
            UpdateStartAndEnd(value);
            ResizeFillers();
            ReSortItems();
            oldStartIndex = startIndex;
        }
    }

    /// <summary>
    /// this is called when the rectTransfrom size changes 
    /// it is used to update the list to handle the new size change
    /// </summary>
    private void OnSizeChange() //TODO: enhance
    {
        ClearOldItems();
        CalculateMeasures();
        CreateVisibleItems();
        CreateFillers();
        oldStartIndex = -1;
        endIndex = startIndex + VisibleItemsNumber - 1;
        ExcuteOnItemShowAction();
    }

    /// <summary>
    /// resort the list items so the items that is no longer supposed to be visible will be moved to the other end of the list and filled with the new data
    /// </summary>
    private void ReSortItems()
    {
        if (startIndex != oldStartIndex)//make sure visible items not the same
        {
            if (startIndex > oldStartIndex)//check if lest went down 
            {
                for (int i = 0; i < activeItems.Count; i++)
                {
                    if (activeItems[i].index < startIndex) //loop while the items are not is visible in the new position
                    {
                        InvokeItemHideCallback(i);
                        activeItems[i].index = endIndex - i;
                        MoveToEndOfItems(activeItems[i].item, i);
                        InvokeItemShowCallback(i);
                    }
                    else
                    {
                        break;//when reach already available items stop the iteration 
                    }
                }
            }
            else //check if lest went down 
            {
                for (int i = activeItems.Count - 1; i >= 0; i--)//loop while the items are not is visible in the new position
                {
                    if (activeItems[i].index > endIndex)
                    {
                        InvokeItemHideCallback(i);
                        activeItems[i].index = startIndex + (activeItems.Count - 1 - i);
                        MoveToStartOfItems(activeItems[i].item, activeItems.Count - 1 - i);
                        InvokeItemShowCallback(i);
                    }
                    else
                    {
                        break;//when reach already available items stop the iteration 
                    }
                }
            }

            activeItems.Sort((item1, item2) => { return item1.index - item2.index; });//sort the items data in the list
        }
    }


    private void InvokeItemShowCallback(int i)
    {
        InvokeItemShowCallback(activeItems[i]);
    }
    private void InvokeItemShowCallback(ItemIndexPair pair)
    {
        if (pair.index >= totallItemsNumber || pair.index < 0)
        {
            pair.item.gameObject.SetActive(false);
            return;
        }

        pair.item.gameObject.SetActive(true);
        OnItemShow?.Invoke(pair.index, pair.item);
        if (pair.optmizedItem) pair.optmizedItem.OnShow.Invoke(pair.index); //if the item containt component OptmizedListItem Invoke OnShow
    }

    private void InvokeItemHideCallback(int i)
    {
        InvokeItemHideCallback(activeItems[i]);
    }
    private void InvokeItemHideCallback(ItemIndexPair pair)
    {
        OnItemHide?.Invoke(pair.index, pair.item);
        if (pair.optmizedItem) pair.optmizedItem.OnHide.Invoke(pair.index);//if the item containt component OptmizedListItem Invoke OnHide
    }

    /// <summary>
    /// move the item to the end of the list after the filler 
    /// </summary>
    /// <param name="item"></param>
    /// <param name="shift">how far from the end of the list</param>
    private void MoveToEndOfItems(GameObject item, int shift)
    {
        item.transform.SetSiblingIndex((frontFiller.GetSiblingIndex() - 1) - shift);
    }
    /// <summary>
    /// move the item to the start of the list before the filler 
    /// </summary>
    /// <param name="item"></param>
    /// <param name="shift">how far from the start of the list</param>
    private void MoveToStartOfItems(GameObject item, int shift)
    {
        item.transform.SetSiblingIndex((rearFiller.GetSiblingIndex() + 1) + shift);
    }

    /// <summary>
    /// calculate the new start and end index values based on the new slider value
    /// </summary>
    /// <param name="value">current slider value</param>
    private void UpdateStartAndEnd(float value)
    {
        startIndex = Mathf.FloorToInt((100 - value * 100) / itemLengthPercintage);
        endIndex = startIndex + VisibleItemsNumber - 1;
        if (startIndex < 0)
        {
            startIndex = 0;
            endIndex = VisibleItemsNumber - 1;
        }
        if (endIndex >= totallItemsNumber)
        {
            startIndex = totallItemsNumber - VisibleItemsNumber;
            endIndex = totallItemsNumber - 1;
        }
    }

    /// <summary>
    /// resize the fillers so the items stay in the visible area
    /// </summary>
    private void ResizeFillers()
    {
        var length = startIndex * (ItemLength + itemsSpacing) - itemsSpacing; //the space before the visble items
        if (isHorizintal)
        {
            rearFiller.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, length);
        }
        else
        {
            rearFiller.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, length);
        }

        length = (totallItemsNumber - endIndex - 1) * (ItemLength + itemsSpacing) - itemsSpacing; //the space after the visble items

        if (isHorizintal)
        {
            frontFiller.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, length);
        }
        else
        {
            frontFiller.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, length);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());
    }

    /// <summary>
    /// create an empty rect before and after the items to act as the space of the hidden items
    /// </summary>
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

    /// <summary>
    /// delete any old items that are not the template
    /// </summary>
    private void ClearOldItems()
    {
        for (int i = 0; i < content.childCount; i++)
        {
            if (ItemTemplate != content.GetChild(i).gameObject)//if the item is the template used for Instantiation dont destroy it
            {
                Destroy(content.GetChild(i).gameObject);
            }
        }
    }

    /// <summary>
    /// calculate the values required to adjust and build the view
    /// </summary>
    private void CalculateMeasures()
    {
        if (isHorizintal)
        {
            ItemLength = ItemTemplate.GetComponent<RectTransform>().rect.width;//the width of the single item

        }
        else
        {
            ItemLength = ItemTemplate.GetComponent<RectTransform>().rect.height;//the height of the single item
        }

        VerticalLayoutGroup vlg = content.GetComponent<VerticalLayoutGroup>();
        if (vlg)
        {
            itemsSpacing = vlg.padding.vertical;//the space bettween the items if is used
        }

        if (isHorizintal)
        {
            ViewportLength = this.GetComponent<RectTransform>().rect.width;//the visible area width
        }
        else
        {
            ViewportLength = this.GetComponent<RectTransform>().rect.height;//the visible area height
        }

        VisibleItemsNumber = (int)(ViewportLength / (ItemLength + itemsSpacing));//the number of the items that can be displayed in the visible area

        ContentLength = totallItemsNumber * (ItemLength + itemsSpacing) - itemsSpacing;//the full height of the content

        itemLengthPercintage = (100 * ItemLength) / (ContentLength - (ItemLength * VisibleItemsNumber));//the single item height percintage to the totall items height

        VisibleItemsNumber += 2;//add extra 2 items to make the list look better
    }

    /// <summary>
    /// create the items that will be used to preview the data
    /// </summary>
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
