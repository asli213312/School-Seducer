using System;
using System.Reflection;
using _School_Seducer_.Editor.Scripts;
using _School_Seducer_.Editor.Scripts.Chat;
using _School_Seducer_.Editor.Scripts.Utility;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(LocalizedScriptableObject), true)]
    public class LocalizedDataEditor : UnityEditor.Editor
    {
        private SerializedObject _serializedObject;
        
        [MenuItem("Tools/Edit Localized Data")]
        private static void EditLocalizedData()
        {
            foreach (var obj in Selection.objects)
            {
                if (obj is LocalizedScriptableObject scriptableObject)
                {
                    Debug.Log("Selected object can open localized data editor window");
                    LocalizedDataEditorWindow.Open(scriptableObject);
                }
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawDefaultInspector();

            EditorGUILayout.Space();

            if (GUILayout.Button("Edit Localized Data"))
            {
                foreach (var targetObject in targets)
                {
                    if (targetObject is LocalizedScriptableObject scriptableObject)
                    {
                        LocalizedDataEditorWindow.Open(scriptableObject);
                    }
                }
            }
            
            _serializedObject = new SerializedObject(target);
            
            serializedObject.ApplyModifiedProperties();
        }
        
        private void EmptyMethodCallback()
        {
            // Пустой метод
            Debug.Log("Empty Method Called");
        }
        
        private void ShowContextMenuForStringField(object target, FieldInfo field)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent($"Edit {field.Name}"), false, () => EditStringField(target, field));
            menu.ShowAsContext();
        }

        private void EditStringField(object target, FieldInfo field)
        {
            // Добавьте здесь вашу логику редактирования поля
            Debug.Log($"Editing {field.Name}");
        }

        private void AddContextMenuForStringFields(object target)
        {
            FieldInfo[] fields = target.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (FieldInfo field in fields)
            {
                if (field.FieldType == typeof(string) && field.DeclaringType == target.GetType())
                {
                    // Проверяем, находится ли указатель мыши над строковым полем
                    Rect fieldRect = EditorGUILayout.GetControlRect(GUILayout.ExpandWidth(true), GUILayout.Height(EditorGUIUtility.singleLineHeight));

                    if (fieldRect.Contains(Event.current.mousePosition))
                    {
                        ShowContextMenuForStringField(target, field);
                    }

                    // Отображаем строковое поле
                    EditorGUI.TextField(fieldRect, field.Name, field.GetValue(target) as string);
                }
            }
        }
    }
}