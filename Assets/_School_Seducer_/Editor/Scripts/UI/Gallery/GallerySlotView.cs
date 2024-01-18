using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using _School_Seducer_.Editor.Scripts.Chat;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;
using UnityFigmaBridge.Runtime.UI;

namespace _School_Seducer_.Editor.Scripts.UI
{
    public class GallerySlotView : MonoBehaviour, IContent
    {
        [SerializeField] private FigmaImage image;
        [SerializeField] private FigmaImage frame;
        [SerializeField] private FigmaImage animationPreview;
        
        public GallerySlotData Data { get; private set; }

        public void Render(GallerySlotData data, Sprite lockedSlot, Sprite emptySlot)
        {
            Data = data;

            if (Data.AddedInGallery)
            {
                image.sprite = data.Sprite;
                frame.sprite = emptySlot;
            }
            else
            {
                image.sprite = data.Sprite;
                frame.sprite = lockedSlot;
            }
        }
    }
}