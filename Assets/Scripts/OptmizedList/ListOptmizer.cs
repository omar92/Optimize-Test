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
    float ItemHeight = 0;
    float itemsSpacing = 0;
    float ViewportHeight = 0;
    int VisibleItemsNumber = 0;
    int totallItemsNumber = 0;
    float ContentHeight = 0;
    float itemHeighPercintage = 0;

    List<GameObject> visbleItems;
    #endregion


    #region MonoBehaviour

    private void Awake()
    {
    }

    void Start()
    {
        PopulateList(100, (i) =>
        {
            Debug.Log($"Populate item {i}");
        });
    }
    #endregion

    #region methods


    public void PopulateList(int itemsNumber, Action<int> OnItemPopulate)
    {
        this.totallItemsNumber = itemsNumber;
        ClearOldItems();
        CalculateMeasures();
        CreateVisibleItems();
        CreateFillers();
        startIndex = 0;
        endIndex = VisibleItemsNumber - 1;
        ResizePanels();
        scrollbarVertical.onValueChanged.AddListener(OnScrollValueChanged);
    }

    private void OnScrollValueChanged(float value)
    {
        startIndex = Mathf.FloorToInt((100 - value * 100) / itemHeighPercintage);
        endIndex = startIndex + VisibleItemsNumber - 1;
        if (endIndex > totallItemsNumber)
        {
            startIndex = totallItemsNumber - VisibleItemsNumber;
            endIndex = totallItemsNumber;
        }
        ResizePanels();
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

    #endregion
    #region functions
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
        visbleItems = new List<GameObject>();
        ItemTemplate.SetActive(true);
        for (int i = 0; i < VisibleItemsNumber; i++)
        {
            visbleItems.Add(GameObject.Instantiate(ItemTemplate, content));
        }
        ItemTemplate.SetActive(false);
    }
    #endregion

}
