using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace _School_Seducer_.Editor.Scripts.UI
{
    [CreateAssetMenu(fileName = "GallerySlotData", menuName = "Game/Data/Gallery/Slot", order = 0)]
    public class GallerySlotData : ScriptableObject
    {
        [ShowAssetPreview(32, 32), ShowIf("CheckEmptyAnimation")] public Sprite Sprite;
        [ShowAssetPreview(32, 32), ShowIf("CheckEmptyImage")] public Sprite Animation;
        
        private bool CheckEmptyImage() => Sprite == null;
        private bool CheckEmptyAnimation() => Animation == null;
        
        [FormerlySerializedAs("Type")] public GallerySlotType Section;

        [FormerlySerializedAs("AddedToGallery")] public bool NeedInGallery = true;
        public bool ShowDebugParameters; 
        [ShowIf("ShowDebugParameters")] public bool AddedInGallery;

        public void CheckNeedInGallery()
        {
            if (NeedInGallery) AddedInGallery = true;
        }
    }
}