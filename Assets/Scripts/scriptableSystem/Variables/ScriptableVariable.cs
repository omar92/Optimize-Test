using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScriptableSystem
{
    public class ScriptableVariable<T> : ScriptableObject
    {
        public T Value;
    }
}