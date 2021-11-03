using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
namespace ScriptableSystem
{
    public class SOEventListener : MonoBehaviour
    {
        [System.Serializable]
        public struct SOEventCallbacksPair
        {
            public EventSO soEvent;
            public UnityEvent<object> onRaise;
        }

        public SOEventCallbacksPair[] soEventCallbacks = new SOEventCallbacksPair[0];

        private void OnEnable()
        {
            for (int i = 0; i < soEventCallbacks.Length; i++)
            {
                soEventCallbacks[i].soEvent.Subscribe(this,soEventCallbacks[i].onRaise);
            }
        }

        private void OnDisable()
        {
            for (int i = 0; i < soEventCallbacks.Length; i++)
            {
                soEventCallbacks[i].soEvent.UnSubscribe(this);
            }
        }
    }
}
