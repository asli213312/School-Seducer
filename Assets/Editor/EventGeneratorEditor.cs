using _School_Seducer_.Editor.Scripts.Utility;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(EventGenerator))]
    public class EventGeneratorEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
        }
    }
}