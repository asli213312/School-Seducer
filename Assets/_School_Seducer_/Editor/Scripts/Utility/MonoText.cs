using System.Linq;
using System.Reflection;
using _School_Seducer_.Editor.Scripts.Extensions;
using _School_Seducer_.Editor.Scripts.Utility.Attributes;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.Utility
{
    public class MonoText : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour monoObject;
        [SerializeField] private TextMeshProUGUI textComponent;
        [SerializeField] private string necessaryTypeName;
        [SerializeField] private bool useValueByString;
        [SerializeField] private bool needEntryUpdate;
        [SerializeField, ShowIf(nameof(useValueByString))] private string nameMember;

        private MonoBehaviour _necessaryType;
        private MemberInfo _foundedMember;

        public void UpdateText()
        {
            _foundedMember.IsNullReturn();

            object foundedMemberObject = null;

            if (_foundedMember is FieldInfo foundedField)
            {
                foundedMemberObject = foundedField.GetValue(_necessaryType);
            }
            else if (_foundedMember is PropertyInfo foundedProperty)
            {
                foundedMemberObject = foundedProperty.GetValue(_necessaryType, null);
            }

            if (foundedMemberObject is not Object engineObject) return;
            
            textComponent.text = engineObject.name;
            //Debug.Log("Name founded member: " + engineObject.name);
        }

        private void OnEnable()
        {
            if (needEntryUpdate)
            {
                Invoke(nameof(UpdateText), .1f);
                needEntryUpdate = false;
            }
        }

        private void Awake()
        {
            if (monoObject != null && textComponent != null)
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
                    Debug.LogWarning("Failed to find the component using InstanceID for: " + name);
                }
            }
        }
        
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
                            textComponent.text = objectField.name;
                            _foundedMember = field;
                        }
                    }
                    else if (member is PropertyInfo property)
                    {
                        var propertyValue = property.GetValue(_necessaryType, null);

                        if (propertyValue is Object objectProperty)
                        {
                            textComponent.text = objectProperty.name;
                            _foundedMember = property;
                        }
                    }
                    
                    Debug.Log($"MonoText as {name} was successfully changed on = {textComponent.text}");
                }
            }
        }

        private void FindMonoTextByString()
        {
            if (_necessaryType == null)
            {
                Debug.LogError("Needed type for MonoText: " + name + " not founded");
                return;
            }
            
            var members = _necessaryType.GetType().GetMembers();

            _foundedMember = members.FirstOrDefault(x => x.Name == nameMember);
        }
    }
}