using System;
using System.Linq;
using System.Reflection;
using _School_Seducer_.Editor.Scripts.Utility;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor
{
    [CustomEditor(typeof(ObjectMembersHighlighter))]
    public class ObjectHighlighterEditor : UnityEditor.Editor
    {
        private SerializedProperty targetProperty;
        private SerializedProperty targetObjectNameProperty;

        private void OnEnable()
        {
            targetProperty = serializedObject.FindProperty("target");
            targetObjectNameProperty = serializedObject.FindProperty("targetObjectName");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            ObjectMembersHighlighter targetObject = (ObjectMembersHighlighter)target;

            // Отобразить поле target
            EditorGUILayout.PropertyField(targetProperty);

            // Отобразить поле targetObjectName
            EditorGUILayout.PropertyField(targetObjectNameProperty);

            if (targetObject.target != null)
            {
                UnityEngine.MonoBehaviour targetObjectToDisplay = FindTargetObject(targetObject);
                if (targetObjectToDisplay != null)
                {
                    GUILayout.TextField("Field");
                    EditorGUILayout.Separator();
                    DisplayBoolFields(targetObjectToDisplay);
                    GUILayout.Space(5);
                    GUILayout.TextField("Properties");
                    EditorGUILayout.Separator();
                    DisplayBoolProperties(targetObjectToDisplay);
                    GUILayout.Space(5);
                    GUILayout.TextField("Bool Methods");
                    EditorGUILayout.Separator();
                    DisplayBoolMethods(targetObjectToDisplay);
                    
                    GUILayout.Space(10);
                    GUILayout.TextField("Void Methods");
                    DisplayVoidMethods(targetObjectToDisplay);
                }
                else
                {
                    EditorGUILayout.HelpBox("Object not found", MessageType.Warning);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private UnityEngine.MonoBehaviour FindTargetObject(ObjectMembersHighlighter targetObject)
        {
            if (targetObject.target == null)
                return null;

            // Получаем компонент по имени на объекте target
            Component[] components = targetObject.target.GetComponents<Component>();
            foreach (Component component in components)
            {
                if (component != null && component.GetType().Name == targetObject.targetObjectName)
                {
                    return component as MonoBehaviour;
                }
            }

            return null;
        }

        private void DisplayBoolFields(UnityEngine.MonoBehaviour targetObject)
        {
            Type targetType = targetObject.GetType();
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

            // Отображение полей типа bool
            foreach (FieldInfo field in targetType.GetFields(bindingFlags))
            {
                if (field.FieldType == typeof(bool))
                {
                    field.SetValue(targetObject, EditorGUILayout.Toggle(field.Name, (bool)field.GetValue(targetObject)));
                }
            }
        }

        private void DisplayBoolProperties(UnityEngine.MonoBehaviour targetObject)
        {
            Type targetType = targetObject.GetType();
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

            // Отображение свойств типа bool
            foreach (PropertyInfo property in targetType.GetProperties(bindingFlags))
            {
                if (property.PropertyType == typeof(bool))
                {
                    EditorGUILayout.Toggle(property.Name, (bool)property.GetValue(targetObject, null));
                }
            }
        }

        private void DisplayBoolMethods(UnityEngine.MonoBehaviour targetObject)
        {
            Type targetType = targetObject.GetType();
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

            // Отображение методов без параметров и возвращаемого типа void
            foreach (MethodInfo method in targetType.GetMethods(bindingFlags))
            {
                ParameterInfo[] parameters = method.GetParameters();
                if (parameters.Length == 0 && method.ReturnType == typeof(bool))
                {
                    string methodName = method.Name;
                    if (GUILayout.Button(methodName))
                    {
                        method.Invoke(targetObject, null);
                    }
                }
            }
        }
        
        private void DisplayVoidMethods(UnityEngine.MonoBehaviour targetObject)
        {
            Type targetType = targetObject.GetType();
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
    
            foreach (MethodInfo method in targetType.GetMethods(bindingFlags))
            {
                ParameterInfo[] parameters = method.GetParameters();
                if (parameters.Length > 0 && method.ReturnType == typeof(void) && !IsMonoBehaviourMethod(method))
                {
                    string methodName = method.Name;

                    // Создаем строку с параметрами метода
                    string parameterString = "";
                    foreach (ParameterInfo param in parameters)
                    {
                        parameterString += $"{param.ParameterType.Name} {param.Name}, ";
                    }
                    if (!string.IsNullOrEmpty(parameterString))
                    {
                        parameterString = parameterString.Remove(parameterString.Length - 2); // Удаляем последнюю запятую и пробел
                    }

                    // Отображаем кнопку с именем метода и его параметрами
                    if (GUILayout.Button($"{methodName}"))
                    {
                        method.Invoke(targetObject, null);
                    }
                    
                    GUILayout.Label($"{parameterString}");
                    
                    GUILayout.Space(3);
                }
            }
        }
        
        private bool IsMonoBehaviourMethod(MethodInfo method)
        {
            string[] excludedMethods =
            {
                "Awake", "Start", "Update", "FixedUpdate", "LateUpdate", "OnEnable", "OnDisable", "OnDestroy", "Reset",
                "BroadcastMessage", "Invoke", "InvokeRepeating", "CancelInvoke", "StopCoroutine", "GetComponentFastPath", 
                "GetComponentsInChildren", "GetComponents", "GetComponentsInParent", "SendMessageUpwards", 
                "SendMessage", "set_tag", "set_name", "set_hideFlags", "set_enabled", "set_runInEditMode", "set_useGUILayout"
            };
    
            return excludedMethods.Contains(method.Name);
        }
    }
}