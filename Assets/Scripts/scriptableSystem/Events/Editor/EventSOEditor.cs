using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ScriptableSystem
{
    [CustomEditor(typeof(EventSO))]
    public class EventSOEditor : Editor
    {
        EventSO script;
        void OnEnable()
        {
            script = (EventSO)target;
        }

        public override void OnInspectorGUI()
        {

            base.OnInspectorGUI();
            EditorGUI.BeginDisabledGroup(!Application.isPlaying);
            if (GUILayout.Button("Raise"))
            {
                script.Raise();
            }
            EditorGUI.EndDisabledGroup();
        }
    }
}