using System.Collections.Generic;
using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.Utility.Translation
{
    public class LocalizedMonoBehaviour : MonoBehaviour
    {
        [SerializeField] private List<LocalizedScriptableObject.LocalizedField> localizedFieldsRuntime = new();
    }
}