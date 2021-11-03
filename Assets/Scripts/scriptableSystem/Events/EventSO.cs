using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace ScriptableSystem
{
    [CreateAssetMenu(fileName = "EventSO", menuName = "SO/Event")]
    public class EventSO : ScriptableObject
    {
        Dictionary<SOEventListener, UnityEvent<object>> callbacks = new Dictionary<SOEventListener, UnityEvent<object>>();

        public void Raise()
        {
            Raise(null);
        }
        public async void Raise(object value)
        {
            await Task.Delay(1); //this will cause event to act is if they were in a queue (they will be stacked after frame)
            RaiseSync(value);
        }

        public void RaiseSync()
        {
            RaiseSync(null);
        }
        public void RaiseSync(object value)
        {
            foreach (var kvp in callbacks.ToArray())
            {
                // Debug.Log($"<color=green>SoEvent</color> {name} <color=green>invoked</color> {kvp.Key.name}");
                kvp.Value.Invoke(value);
            }
        }

        public void Subscribe(SOEventListener listener, UnityEvent<object> calback)
        {
            if (!callbacks.ContainsKey(listener))
            {
                callbacks.Add(listener, calback);
            }
            else
            {
                callbacks[listener] = calback;
            }
        }

        public void UnSubscribe(SOEventListener listener)
        {
            if (callbacks.ContainsKey(listener))
            {
                callbacks.Remove(listener);
            }
        }

        void OnAfterDeserialize()
        {
            callbacks.Clear();
        }
        void OnBeforeSerialize()
        {
            callbacks.Clear();
        }

    }
}