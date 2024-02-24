using NaughtyAttributes;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Serialization;

namespace _School_Seducer_.Editor.Scripts.UI
{
    [CreateAssetMenu(fileName = "GallerySlotData", menuName = "Game/Data/Gallery/Slot", order = 0)]
    public class GallerySlotData : ScriptableObject
    {
        [ShowAssetPreview(32, 32)] public Sprite Sprite;
        [HideIf(nameof(Section), GallerySlotType.Photo)] public SkeletonDataAsset animation;
        
        private bool CheckEmptyImage() => Sprite == null;
        private bool CheckEmptyAnimation() => animation == null;
        
        [FormerlySerializedAs("Type")] public GallerySlotType Section;

        [FormerlySerializedAs("AddedToGallery")] public bool NeedInGallery = true;
        [SerializeField] public bool showDebugParameters; 
        [Sirenix.OdinInspector.ShowIf("showDebugParameters")] public bool AddedInGallery;

        public string GetAnimationName() => animation.GetSkeletonData(false).Animations.Items[0].Name;

        public void CheckNeedInGallery()
        {
            if (NeedInGallery) AddedInGallery = true;
        }
    }
}