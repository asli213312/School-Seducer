using System;
using System.Linq;
using System.Reflection;
using _School_Seducer_.Editor.Scripts.Extensions;
using _School_Seducer_.Editor.Scripts.Utility.Attributes;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace _School_Seducer_.Editor.Scripts.Utility
{
    public abstract class MonoBase : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour monoObject;
        [SerializeField, Tooltip("ID for call using MonoController")] private string nameId;
        [SerializeField] private string necessaryTypeName;
        [SerializeField] private bool useValueByString;
        [SerializeField] private bool needEntryUpdate;
        [SerializeField, ShowIf(nameof(useValueByString))] private string nameMember;

        [Header("Debug")] 
        [SerializeField] private bool showDebugParameters;

        public string NameId => nameId;

        protected object FoundedMemberObject;
        protected Object FoundedEngineObject;
        
        private MonoBehaviour _necessaryType;
        private Type _foundedTypeOfMember;
        [ShowIf(nameof(showDebugParameters)), ShowInInspector] private MemberInfo _foundedMember;
        
        private MemberInfo FoundedMember
        {
            get => _foundedMember;
            set
            {
                _foundedMember = value;
                UpdateMember();
            }
        }

        private void OnEnable()
        {
            if (needEntryUpdate)
            {
                Invoke(nameof(UpdateMember), .1f);
                needEntryUpdate = false;
            }
        }

        private void Awake()
        {
            if (monoObject != null)
            {
                Component[] components = monoObject.GetComponents(typeof(Component));

                Component foundedComponent = components.FirstOrDefault(x => x.GetType().Name == necessaryTypeName);

                if (foundedComponent != null)
                {
                    _necessaryType = (MonoBehaviour)foundedComponent;
                    
                    if (useValueByString == false) FindMonoTextByAttribute();
                    else FindMonoTextByString();
                }
                else
                {
                    Debug.LogWarning("Failed to find the necessary component for: " + name);
                }
            }
        }

        public void UpdateMember()
        {
            FoundedMember.IsNullReturn();

            object foundedMemberObject = null;

            if (FoundedMember is FieldInfo foundedField)
            {
                foundedMemberObject = foundedField.GetValue(_necessaryType);
                
                //Debug.Log("FoundedMemberObject: "+ foundedMemberObject.GetType());
            }
            else if (FoundedMember is PropertyInfo foundedProperty)
            {
	            foundedMemberObject = foundedProperty.GetValue(_necessaryType);

	            //Debug.Log("FoundedMemberObject: "+ foundedMemberObject.GetType());
            }

            FoundedMemberObject = foundedMemberObject;

            if (foundedMemberObject is Object engineObject)
            {
                FoundedEngineObject = engineObject;
                
                UpdateEngineObject();
                Debug.Log("Name founded member: " + engineObject.name);
            }
            else
            {
                UpdateObject();
                Debug.Log("Value founded member: " + foundedMemberObject);   
            }
        }

        protected abstract void UpdateEngineObject();
        protected abstract void UpdateObject();

        private void FindMonoTextByAttribute()
        {
            if (_necessaryType == null)
            {
                Debug.LogError("Needed type for MonoText: " + name + " not founded");
                return;
            }
            
            var members = _necessaryType.GetType().GetMembers();

            foreach (var member in members)
            {
                var monoTextAttribute = (MonoTextAttribute)member.GetCustomAttribute(typeof(MonoTextAttribute), false);

                if (monoTextAttribute != null)
                {
                    if (member is FieldInfo field)
                    {
                        var fieldValue = field.GetValue(_necessaryType);

                        if (fieldValue is Object objectField)
                        {
                            //textComponent.text = objectField.name;
                        }
                        
                        FoundedMember = field;
                    }
                    else if (member is PropertyInfo property)
                    {
                        var propertyValue = property.GetValue(_necessaryType, null);

                        if (propertyValue is Object objectProperty)
                        {
                            //textComponent.text = objectProperty.name;
                        }
                        
                        FoundedMember = property;
                    }
                    
                    //Debug.Log($"MonoText as {name} was successfully changed on = {textComponent.text}");
                    Debug.Log("MonoText was successfully founded foundedMember! " + name);
                }
            }
        }

        private void FindMonoTextByString()
        {
	        if (_necessaryType == null)
	        {
		        Debug.LogError($"Needed type for {GetType().Name}: "  + name + " not found");
		        return;
	        }
    
	        string[] memberNames = nameMember.Split('.');
    
	        MemberInfo foundMember = null;
	        Type currentType = _necessaryType.GetType();
	        object currentObject = _necessaryType; // Сохраняем объект, от которого начался поиск
	        object foundObject = null;

	        foreach (string memberName in memberNames)
	        {
		        // Находим поле или свойство в текущем объекте
		        FieldInfo field = currentObject.GetType().GetField(memberName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
		        PropertyInfo property = currentObject.GetType().GetProperty(memberName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

		        if (field != null)
		        {
			        foundMember = field;
			        currentType = field.DeclaringType;
			        //currentObject = field.GetValue(currentType);
			        //Debug.Log($"Field '{memberName}' found in type '{property.DeclaringType.Name}'");
		        }
		        else if (property != null)
		        {
			        foundMember = property;
			        currentType = property.DeclaringType;
			        //currentObject = property.GetValue(currentType);
			        //Debug.Log($"Property '{memberName}' found in type '{property.DeclaringType.Name}'");
		        }
		        else
		        {
			        Debug.LogError("Member '" + memberName + "' not found.");
			        return;
		        }
	        }

	        if (foundMember != null)
	        {
		        //_foundedTypeOfMember = currentType; // Тип найденного объекта
		        FoundedMember = foundMember;
		        Debug.Log("Found member: " + foundMember.Name);
		        //Debug.Log("<color=green>Founded type of member:</color> " + _foundedTypeOfMember.GetType().Name);
	        }

	        return;
        }
    }
}