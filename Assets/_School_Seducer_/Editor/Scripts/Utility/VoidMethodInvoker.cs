using System;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _School_Seducer_.Editor.Scripts.Utility
{
    public enum ParameterType
    {
        Integer,
        Float,
        String,
        Boolean,
        Object
    }
    
    [System.Serializable]
    public class CustomParameter
    {
        public ParameterType parameterType;
        public int intValue;
        public float floatValue;
        public string stringValue;
        public bool boolValue;
        public Object objectValue;
        public object currentValue;
    }
    
    public class VoidMethodInvoker : MonoBehaviour
    {
        [SerializeField] public MonoBehaviour target;
        [SerializeField] public string targetObjectName;
        [SerializeField] public string methodName;

        [Header("Parameters")]
        [SerializeField] public CustomParameter parameterFirst;
        [SerializeField] public CustomParameter parameterSecond;
        [SerializeField] public CustomParameter parameterThird;

        private string _parameterFirstName;
        private string _parameterSecondName;
        private string _parameterThirdName;

        private Component _targetComponent;

        private MethodInfo _foundedVoidMethod;

        private void OnValidate()
        {
            CheckCurrentValueParameter(parameterFirst);
            CheckCurrentValueParameter(parameterSecond);
            CheckCurrentValueParameter(parameterThird);
        }

        private void CheckCurrentValueParameter(CustomParameter parameter)
        {
            switch (parameter.parameterType)
            {
                case ParameterType.Integer: parameter.currentValue = parameter.intValue > 0 ? parameter.intValue : 0; break;
                case ParameterType.String: parameter.currentValue = parameter.stringValue; break;
                case ParameterType.Boolean: parameter.currentValue = parameter.boolValue; break;
                case ParameterType.Float: parameter.currentValue = parameter.floatValue; break;
                case ParameterType.Object: parameter.currentValue = parameter.objectValue; break;
            }
        }

        public void Invoke()
        {
            if (_foundedVoidMethod != null)
            {
                ParameterInfo[] parameters = _foundedVoidMethod.GetParameters();
                object[] parameterValues = new object[parameters.Length];
                
                // Проход по каждому параметру и установка соответствующего значения
                for (int i = 0; i < parameters.Length; i++)
                {
                    // Получаем тип параметра по его имени
                    object parameterType = GetParameterValue(i);
                    
                    // Пытаемся скастовать значение параметра к нужному типу
                    if (parameterType != null)
                    {
                        Debug.Log("<color=red>VOID INVOKER: </color> found parameter type: " + parameterType);
                        
                        parameterValues[i] = FindInnerParameterType(GetParameterTypeName(i), parameterType);
                    }
                    else
                    {
                        // Если не удалось определить тип параметра, устанавливаем значение в null
                        parameterValues[i] = null;
                        Debug.Log("<color=red>VOID INVOKER: </color> not found parameter type as: " + GetParameterTypeName(i));
                    }
                }
                
                _foundedVoidMethod.Invoke(_targetComponent, parameterValues);
                Debug.Log("<color=red>VOID INVOKER: </color> method invoked with count params: " + parameterValues.Length);
            }
        }
        
        private object CastParameterValueToType(object parameter, string typeName)
            {
                Type parameterType = Type.GetType(typeName);
                if (parameterType != null)
                {
                    try
                    {
                        // Пытаемся привести параметр к нужному типу
                        object castedValue = Convert.ChangeType(parameter, parameterType);
                        return castedValue;
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Error casting parameter to type {parameterType}: {ex.Message}");
                        return null;
                    }
                }
                else
                {
                    Debug.LogError($"Type {typeName} not found.");
                    return null;
                }
            }
        

        private string GetParameterTypeName(int index)
        {
            switch (index)
            {
                case 0:
                    return _parameterFirstName;
                case 1:
                    return _parameterSecondName;
                case 2:
                    return _parameterThirdName;
                default:
                    return null;
            }
        }

        private object GetParameterValue(int index)
        {
            switch (index)
            {
                case 0:
                    return parameterFirst.currentValue;
                case 1:
                    return parameterSecond.currentValue;
                case 2:
                    return parameterThird.currentValue;
                default:
                    return null;
            }
        }

        private void Start() => FindMethod(methodName);
        
        private Object FindInnerParameterType(string typeName, object parameter) 
        {
            if (parameter is ScriptableObject scriptableObj) return scriptableObj;
             
             Component targetComponent = null;
             GameObject parameterGameObject = parameter as GameObject;
             Component[] components = parameterGameObject.GetComponents<Component>();
             foreach (Component component in components)
             {
                 if (component != null && component.GetType().Name == typeName)
                 {
                     targetComponent = component;
                     Debug.Log("<color=green>Returned target component for parameter: </color> " + typeName);
                     return targetComponent;
                 }
             }
             Debug.Log("<color=red>Returned target component for parameter: </color> as NULL ");
             return null;
        }

        public void FindMethod(string memberName)
        {
            if (target == null)
            {
                Debug.LogWarning("Target object is not found.");
                return;
            }

            Type targetType = target.GetType();
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
            
            Component targetComponent = null;
            Component[] components = target.GetComponents<Component>();
            foreach (Component component in components)
            {
                if (component != null && component.GetType().Name == targetObjectName)
                {
                    targetComponent = component;
                    _targetComponent = targetComponent;
                    break;
                }
            }

            if (targetComponent == null)
            {
                Debug.LogWarning($"Component \"{targetObjectName}\" not found.");
                return;
            }
            
            foreach (PropertyInfo property in targetType.GetProperties(bindingFlags))
            {
                if (property.GetGetMethod() != null && property.GetGetMethod().Name == memberName)
                {
                    _foundedVoidMethod = property.GetGetMethod();
                    Debug.Log("<color=red>VOID INVOKER: </color> <color=green>found GETTER </color> " + property.GetGetMethod().Name);
                    return;
                }

                if (property.GetSetMethod() != null && property.GetSetMethod().Name == memberName)
                {
                    _foundedVoidMethod = property.GetSetMethod();
                    Debug.Log("<color=red>VOID INVOKER: </color> <color=green>found SETTER </color> " + property.GetSetMethod().Name);
                    return;                    
                }
            }
            
            foreach (MethodInfo method in targetType.GetMethods(bindingFlags))
            {
                if (method.Name == memberName)
                {
                    _foundedVoidMethod = method;
                    Debug.Log("<color=red>VOID INVOKER: </color> <color=green>found METHOD </color> " + method.Name);
                    
                    ParameterInfo[] parameters = method.GetParameters();
                    if (parameters.Length > 0)
                    {
                        for (int i = 0; i < parameters.Length; i++)
                        {
                            switch (i)
                            {
                                case 0: _parameterFirstName = parameters[i].ParameterType.Name;
                                    Debug.Log("Founded parameter type name: " + _parameterFirstName);
                                    break;
                                case 1: _parameterSecondName = parameters[i].ParameterType.Name;
                                    Debug.Log("Founded parameter type name: " + _parameterSecondName);
                                    break;
                                case 2: _parameterThirdName = parameters[i].ParameterType.Name;
                                    Debug.Log("Founded parameter type name: " + _parameterThirdName);
                                    break;
                            }
                        }
                    }
                    return;
                }
            }
            
            Debug.LogWarning($"Member \"{memberName}\" not found.");
        }
    }
}