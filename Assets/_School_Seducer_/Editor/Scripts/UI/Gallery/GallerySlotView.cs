using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using _School_Seducer_.Editor.Scripts.Chat;
using NaughtyAttributes;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;
using UnityFigmaBridge.Runtime.UI;

namespace _School_Seducer_.Editor.Scripts.UI
{
    public class GallerySlotView : MonoBehaviour, IContent
    {
        [SerializeField] private FigmaImage image;
        [SerializeField] private FigmaImage frame;

        public GallerySlotDataBase Data { get; private set; }

        public void Render(GallerySlotDataBase data, Sprite lockedSlot, Sprite emptySlot)
        {
            Data = data;

            if (data.AddedInGallery)
            {
                image.sprite = data.Sprite;
                frame.sprite = emptySlot;
                
                if (data is GallerySlotData defaultData)
                    if (defaultData.animation != null) GetComponentInChildren<SkeletonAnimation>().skeletonDataAsset = defaultData.animation;
            }
            else
            {
                image.sprite = data.Sprite;
                frame.sprite = lockedSlot;
            }
        }
    }
}