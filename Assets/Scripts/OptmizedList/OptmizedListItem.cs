using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// this component can be used on items to fire unityEvents when item show or hide
/// </summary>
public class OptmizedListItem : MonoBehaviour
{
    public UnityEvent<int> OnShow;
    public UnityEvent<int> OnHide;
}
