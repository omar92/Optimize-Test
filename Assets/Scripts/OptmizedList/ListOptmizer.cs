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
    [SerializeField] private int maxInistances;

    [SerializeField] private int startIndex = 0;
    [SerializeField] private int endIndex = 0;
    private RectTransform rearFiller;
    private RectTransform frontFiller;

    [SerializeField] float ItemHeight = 0;
    [SerializeField] float itemsSpacing = 0;
    [SerializeField] float ViewportHeight = 0;
    [SerializeField] int VisibleItemsNumber = 0;
    [SerializeField] float ContentHeight = 0;

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
        ClearOldItems();
        CalculateMeasures(itemsNumber);
        CreateVisibleItems();
        CreateFillers();
        startIndex = 0;
        endIndex = VisibleItemsNumber - 1;
        ResizePanels(itemsNumber);

    }

    private void ResizePanels(int totallItemsNumber)
    {
        var rect = rearFiller.rect;
        var height = startIndex * (ItemHeight + itemsSpacing) - itemsSpacing; //the space before the visble items
        rearFiller.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height); //.sert rect.Set(rect.x, rect.y, space, space);

        rect = frontFiller.rect;
        height = (totallItemsNumber - endIndex) * (ItemHeight + itemsSpacing) - itemsSpacing; //the space after the visble items
        frontFiller.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
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

    private void CalculateMeasures(int totallItemsNumber)
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
    }

    private void CreateVisibleItems()
    {
        visbleItems = new List<GameObject>();
        ItemTemplate.SetActive(true);
        for (int i = 0; i < VisibleItemsNumber; i++)
        {
            visbleItems.Add(GameObject.Instantiate(ItemTemplate,content));
        }
        ItemTemplate.SetActive(false);
    }
    #endregion

}
