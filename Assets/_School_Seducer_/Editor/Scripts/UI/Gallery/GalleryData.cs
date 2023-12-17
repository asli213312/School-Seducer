using _School_Seducer_.Editor.Scripts.UI.Gallery;
using UnityEngine;
using UnityEngine.Serialization;

namespace _School_Seducer_.Editor.Scripts.UI
{
    public class GalleryData : ScriptableObject
    {
        [SerializeField, Sirenix.OdinInspector.ShowIf("showDebugParameters")] public GalleryCharacterData dampedData;
        public bool showDebugParameters;
        //[ShowIf("showDebugParameters")] public 
    }
}