using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using NaughtyAttributes;
using UnityEngine;
using UnityFigmaBridge.Runtime.UI;

namespace _School_Seducer_.Editor.Scripts.UI
{
    public class GallerySlotView : MonoBehaviour
    {
        [SerializeField] private FigmaImage image;
        [SerializeField] private FigmaImage animationPreview;
        
        public GallerySlotData Data { get; private set; }
        
        public void Render(GallerySlotData data)
        { 
            Data = data;

            image.sprite = data.Sprite;
        }

        public void CheckCropWidePicture()
        {
            if (Data.Sprite.IsWideSprite())
                image.ScaleMode = FigmaImage.ImageScaleMode.Fill;
        }
    }
}