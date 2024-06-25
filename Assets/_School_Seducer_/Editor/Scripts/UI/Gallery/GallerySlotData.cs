using _School_Seducer_.Editor.Scripts.UI.Gallery;
using NaughtyAttributes;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Serialization;

namespace _School_Seducer_.Editor.Scripts.UI
{
    [CreateAssetMenu(fileName = "GallerySlotData", menuName = "Game/Data/Gallery/Slot", order = 0)]
    public class GallerySlotData : GallerySlotDataBase
    {
        [HideIf(nameof(Section), GallerySlotType.Photo)] public SkeletonDataAsset animation;
        
        private bool CheckEmptyImage() => Sprite == null;
        private bool CheckEmptyAnimation() => animation == null;

        public string GetAnimationName() => animation.GetSkeletonData(false).Animations.Items[0].Name;
    }
}