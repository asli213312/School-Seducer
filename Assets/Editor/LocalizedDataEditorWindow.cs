using System.Collections.Generic;
using System.Reflection;
using _School_Seducer_.Editor.Scripts.Utility;
using _School_Seducer_.Editor.Scripts.Utility.Translation;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Editor
{
    public class LocalizedDataEditorWindow : EditorWindow
    {
        //private SerializedObject serializedObject;
        private SerializedProperty _localizedDataProperty;
        private LocalizedScriptableObject _localizedData;
        private SerializedObject _serializedObject;
        private SerializedProperty _localizedFieldsProperty;

        public static void Open(LocalizedScriptableObject localizedData)
        {
            LocalizedDataEditorWindow window = CreateInstance<LocalizedDataEditorWindow>();
            window._localizedData = localizedData;
            window.ShowUtility();
        }

        private void OnGUI()
        {
            if (_localizedData != null)
            {
                EditorGUILayout.LabelField("Localized Data Editor");

                _serializedObject = new SerializedObject(_localizedData);

                _serializedObject.Update();

                _localizedFieldsProperty = _serializedObject.FindProperty("localizedFields");

                EditorGUILayout.PropertyField(_localizedFieldsProperty, true);

                _serializedObject.ApplyModifiedProperties();
                
                if (GUI.changed)
                {
                    EditorUtility.SetDirty(_localizedData);
                }
            }
        }

        private void DrawLocalizedStructList()
        {
            if (_localizedDataProperty != null)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Localized Struct List");

                EditorGUI.indentLevel++;

                SerializedProperty languagesProperty = _localizedDataProperty.FindPropertyRelative("languages");
                int listSize = languagesProperty.arraySize;

                for (int i = 0; i < listSize; i++)
                {
                    SerializedProperty languageElement = languagesProperty.GetArrayElementAtIndex(i);
                    SerializedProperty languageCode = languageElement.FindPropertyRelative("languageCode");
                    SerializedProperty key = languageElement.FindPropertyRelative("key");

                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.PropertyField(languageCode, GUIContent.none);
                    EditorGUILayout.PropertyField(key, GUIContent.none);

                    if (GUILayout.Button("X", GUILayout.Width(20)))
                    {
                        languagesProperty.DeleteArrayElementAtIndex(i);
                        i--;
                        listSize--;
                    }

                    EditorGUILayout.EndHorizontal();
                }

                EditorGUI.indentLevel--;

                if (GUILayout.Button("Add Language"))
                {
                    languagesProperty.arraySize++;
                }
            }
        }
    }
}