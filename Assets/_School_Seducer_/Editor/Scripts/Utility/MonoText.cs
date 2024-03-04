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
        [SerializeField, Tooltip("ID for call using MonoController")] private string nameId;
        [SerializeField] private string necessaryTypeName;
        [SerializeField] private bool useValueByString;
        [SerializeField] private bool needEntryUpdate;
        [SerializeField, Tooltip("Const base text + divider + nameMember")] private bool needConstBaseText;
        [SerializeField, ShowIf(nameof(needConstBaseText))] private string divider;
        [SerializeField, ShowIf(nameof(needConstBaseText))] private string baseText;
        [SerializeField, ShowIf(nameof(useValueByString))] private string nameMember;

        [Header("Debug")] 
        [SerializeField] private bool showDebugParameters;

        public string NameId => nameId;
        
        private MonoBehaviour _necessaryType;
        [ShowIf(nameof(showDebugParameters)), ShowInInspector] private MemberInfo _foundedMember;
        
        private MemberInfo FoundedMember
        {
            get => _foundedMember;
            set
            {
                _foundedMember = value;
                UpdateText();
            }
        }

        public void UpdateText()
        {
            FoundedMember.IsNullReturn();

            object foundedMemberObject = null;

            if (FoundedMember is FieldInfo foundedField)
            {
                foundedMemberObject = foundedField.GetValue(_necessaryType);
            }
            else if (FoundedMember is PropertyInfo foundedProperty)
            {
                foundedMemberObject = foundedProperty.GetValue(_necessaryType, null);
            }

            if (foundedMemberObject is Object engineObject)
            {
                if (needConstBaseText)
                    textComponent.text = baseText + divider + engineObject.name;
                else
                    textComponent.text = engineObject.name;
                Debug.Log("Name founded member: " + engineObject.name);
            }
            else
            {
                if (needConstBaseText)
                    textComponent.text = baseText + divider + foundedMemberObject;
                else
                    textComponent.text = foundedMemberObject?.ToString();
                Debug.Log("Value founded member: " + foundedMemberObject);   
            }
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
                    Debug.LogWarning("Failed to find the necessary component for: " + name);
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
                        }
                        
                        FoundedMember = field;
                    }
                    else if (member is PropertyInfo property)
                    {
                        var propertyValue = property.GetValue(_necessaryType, null);

                        if (propertyValue is Object objectProperty)
                        {
                            textComponent.text = objectProperty.name;
                        }
                        
                        FoundedMember = property;
                    }
                    
                    Debug.Log($"MonoText as {name} was successfully changed on = {textComponent.text}");
                    Debug.Log("MonoText was successfully founded foundedMember! " + name);
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
            
            var fields = _necessaryType.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var field = fields.FirstOrDefault(f => f.Name == nameMember);
            
            if (field == null)
            {
                var properties = _necessaryType.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                var property = properties.FirstOrDefault(p => p.Name == nameMember);

                if (property != null)
                {
                    FoundedMember = property;
                    Debug.Log("Found property: " + property.Name);
                }
                else
                {
                    Debug.Log("Member not found.");
                }
            }
            else
            {
                FoundedMember = field;
                Debug.Log("Found field: " + field.Name);
            }
        }
    }
}