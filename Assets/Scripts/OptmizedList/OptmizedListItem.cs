using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OptmizedListItem : MonoBehaviour
{

    public UnityEvent<object> OnData;


    public void SetData(object data)
    {
        OnData.Invoke(data);
    }

}
