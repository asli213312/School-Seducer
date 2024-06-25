using System.Reflection;
using _School_Seducer_.Editor.Scripts.Extensions;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _School_Seducer_.Editor.Scripts.Utility
{
    public class MonoText : MonoBase
    {
        [Header("Component options")] 
        [SerializeField] private TextMeshProUGUI textComponent;
        [SerializeField, Tooltip("Const base text + divider + nameMember")] private bool needConstBaseText;
        [SerializeField, ShowIf(nameof(needConstBaseText))] private string divider;
        [SerializeField, ShowIf(nameof(needConstBaseText))] private string baseText;

        protected override void UpdateEngineObject()
        {
            if (needConstBaseText)
                textComponent.text = baseText + divider + FoundedEngineObject.name;
            else
                textComponent.text = FoundedEngineObject.name;
        }

        protected override void UpdateObject()
        {
            if (needConstBaseText)
                textComponent.text = baseText + divider + FoundedMemberObject;
            else
                textComponent.text = FoundedMemberObject?.ToString();
        }
    }
}