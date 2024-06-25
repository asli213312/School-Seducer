using System;
using System.Reflection;
using _School_Seducer_.Editor.Scripts;
using _School_Seducer_.Editor.Scripts.Utility;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor
{
    [CustomEditor(typeof(VoidMethodInvoker))]
    public class VoidMethodInvokerEditor : UnityEditor.Editor
    {
        private SerializedProperty targetObject;
        private SerializedProperty targetMethodName;
        private SerializedProperty targetObjectName;
        private SerializedProperty parameterFirstProperty;
        private SerializedProperty parameterSecondProperty;
        private SerializedProperty parameterThirdProperty;

        private void OnEnable()
        {
            targetObject = serializedObject.FindProperty("target");
            targetObjectName = serializedObject.FindProperty("targetObjectName");
            targetMethodName = serializedObject.FindProperty("methodName");
            parameterFirstProperty = serializedObject.FindProperty("parameterFirst");
            parameterSecondProperty = serializedObject.FindProperty("parameterSecond");
            parameterThirdProperty = serializedObject.FindProperty("parameterThird");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            VoidMethodInvoker targetObject = (VoidMethodInvoker)target;

            EditorGUILayout.PropertyField(this.targetObject);
            EditorGUILayout.PropertyField(this.targetObjectName);
            EditorGUILayout.PropertyField(this.targetMethodName);

            if (targetObject.target != null)
            {
                UnityEngine.MonoBehaviour targetObjectToDisplay = FindTargetObject(targetObject);
                if (targetObjectToDisplay != null)
                {
                    GUILayout.Space(15);
                    GUILayout.TextField("Method info");
                    DisplayMethodInfo(targetObjectToDisplay);
                }
                else
                {
                    EditorGUILayout.HelpBox("Object not found", MessageType.Warning);
                }
            }
            
            GUILayout.Space(20);
            if (GUILayout.Button("Find Method"))
            {
                targetObject.FindMethod(targetObject.methodName);
            }

            serializedObject.ApplyModifiedProperties();
        }
        
        private void DisplayParameterFields(SerializedProperty parameterProperty)
        {
            SerializedProperty parameterTypeProperty = parameterProperty.FindPropertyRelative("parameterType");
            ParameterType parameterType = (ParameterType)parameterTypeProperty.enumValueIndex;

            EditorGUILayout.PropertyField(parameterTypeProperty);

            switch (parameterType)
            {
                case ParameterType.Integer:
                    EditorGUILayout.PropertyField(parameterProperty.FindPropertyRelative("intValue"));
                    break;
                case ParameterType.Float:
                    EditorGUILayout.PropertyField(parameterProperty.FindPropertyRelative("floatValue"));
                    break;
                case ParameterType.String:
                    EditorGUILayout.PropertyField(parameterProperty.FindPropertyRelative("stringValue"));
                    break;
                case ParameterType.Boolean:
                    EditorGUILayout.PropertyField(parameterProperty.FindPropertyRelative("boolValue"));
                    break;
                case ParameterType.Object:
                    EditorGUILayout.PropertyField(parameterProperty.FindPropertyRelative("objectValue"));
                    break;
            }
        }

        private UnityEngine.MonoBehaviour FindTargetObject(VoidMethodInvoker targetObject)
        {
            if (targetObject.target == null)
                return null;
            
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

        private void DisplayMethodInfo(UnityEngine.MonoBehaviour targetObject)
        {
            Type targetType = targetObject.GetType();
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
    
            foreach (MethodInfo method in targetType.GetMethods(bindingFlags))
            {
                if (method.Name != targetMethodName.stringValue) continue;
                
                ParameterInfo[] parameters = method.GetParameters();
                if (parameters.Length > 0 && method.ReturnType == typeof(void))
                {
                    string methodName = method.Name;
                    
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
                    
                    GUILayout.Space(10);

                    EditorGUILayout.LabelField("Parameters", EditorStyles.boldLabel);
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        switch (i)
                        {
                            case 0: DisplayParameterFields(parameterFirstProperty); break;
                            case 1: DisplayParameterFields(parameterSecondProperty); break;
                            case 2: DisplayParameterFields(parameterThirdProperty); break;
                            default: EditorGUILayout.HelpBox("Only 3 parameters are supported.", MessageType.Warning); 
                                break;
                        }
                    }
                }
            }
        }
    }
}