using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScriptableSystem
{
    /// <summary>
    /// vase class for ScriptableVariable
    /// </summary>
    public class ScriptableVariable<T> : ScriptableObject
    {
        public T Value;
    }
}