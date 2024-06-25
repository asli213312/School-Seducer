using System;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.Utility
{
    public class ObjectMembersHighlighter : MonoBehaviour
    {
        [SerializeField] public MonoBehaviour target;
        [SerializeField] public string targetObjectName;

        private Component _targetComponent;
        
        [ShowInInspector] public bool Value 
        { 
            get 
            {
                if (_foundedMember == null) return false;

                if (_foundedMember is FieldInfo)
                {
                    return ((FieldInfo)_foundedMember).GetValue(target) as bool? ?? false;
                }
                else if (_foundedMember is PropertyInfo)
                {
                    return ((PropertyInfo)_foundedMember).GetValue(target, null) as bool? ?? false;
                }
                else if (_foundedMember is MethodInfo)
                {
                    object result = ((MethodInfo)_foundedMember).Invoke(target, null);
                    return result as bool? ?? false;
                }
                else
                {
                    return false;
                }
            }
        }

        private MemberInfo _foundedMember;
        private MethodInfo _foundedVoidMethod;

        public void ChangeTarget(MonoBehaviour newTarget) => target = newTarget;
        public void ChangeObjectName(string newName) => targetObjectName = newName;
        public void InvokeCurrentVoidMethod() => _foundedVoidMethod?.Invoke(_targetComponent, null);
        public void InvokeVoidMethod(string methodName) => InvokeVoidMethodByName(methodName);

        private void InvokeVoidMethodByName(string memberName)
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
            
            foreach (MethodInfo method in targetType.GetMethods(bindingFlags))
            {
                if (method.Name == memberName && method.ReturnType == typeof(void))
                {
                    method.Invoke(targetComponent, null);
                }
            }

            Debug.LogWarning($"Member \"{memberName}\" not found.");
        }

        public void SetCurrentMethod(string memberName)
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
            
            foreach (MethodInfo method in targetType.GetMethods(bindingFlags))
            {
                if (method.Name == memberName && method.ReturnType == typeof(void))
                {
                    _foundedVoidMethod = method;
                }
            }

            Debug.LogWarning($"Member \"{memberName}\" not found.");
        }
        
        public void SetCurrentValue(string memberName)
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
                    break;
                }
            }

            if (targetComponent == null)
            {
                Debug.LogWarning($"Component \"{targetObjectName}\" not found.");
                return;
            }

            // Поиск среди полей
            foreach (FieldInfo field in targetType.GetFields(bindingFlags))
            {
                if (field.Name == memberName && field.FieldType == typeof(bool))
                {
                    bool fieldValue = (bool)field.GetValue(targetComponent);
                    _foundedMember = field;
                    Debug.Log("<color=magenta>OBJECT HIGHLIGHTER</color> Field found: " + field.Name);
                    return;
                }
            }

            // Поиск среди свойств
            foreach (PropertyInfo property in targetType.GetProperties(bindingFlags))
            {
                if (property.Name == memberName && property.PropertyType == typeof(bool))
                {
                    bool propertyValue = (bool)property.GetValue(targetComponent, null);
                    _foundedMember = property;
                    Debug.Log("<color=magenta>OBJECT HIGHLIGHTER</color> Property found: " + property.Name);
                    return;
                }
            }

            // Поиск среди методов без параметров и возвращаемого типа bool
            foreach (MethodInfo method in targetType.GetMethods(bindingFlags))
            {
                if (method.Name == memberName && method.ReturnType == typeof(bool))
                {
                    object result = method.Invoke(targetComponent, null);
                    if (result is bool)
                    {
                        bool boolResult = (bool)result;
                        _foundedMember = method;
                        return;
                    }
                }
            }

            Debug.LogWarning($"Member \"{memberName}\" not found.");
        }
    }
}