using NaughtyAttributes;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace _School_Seducer_.Editor.Scripts.UI
{
    public abstract class GallerySlotDataBase : ScriptableObject
    {
        [ShowAssetPreview(32, 32)] public Sprite Sprite;
        [SerializeField] public GallerySlotType Section;
        [SerializeField, SerializeReference, Sirenix.OdinInspector.ShowIf(nameof(ShowSpecialPrefab))] public GallerySlotView prefab;
        [FormerlySerializedAs("AddedToGallery")] public bool NeedInGallery = true;
        
        [Header("Debug")]
        [SerializeField] public bool showDebugParameters; 
        [Sirenix.OdinInspector.ShowIf("showDebugParameters")] public bool AddedInGallery;

        [Header("Sub content")] 
        [SerializeField, HideLabel] private bool blank;

        private bool ShowSpecialPrefab() => Section == GallerySlotType.Special;
        
        public void CheckNeedInGallery()
        {
            if (NeedInGallery) AddedInGallery = true;
        }
    }
}