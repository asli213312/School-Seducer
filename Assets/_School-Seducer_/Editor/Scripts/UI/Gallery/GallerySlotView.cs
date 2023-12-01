using NaughtyAttributes;
using UnityEngine;
using UnityFigmaBridge.Runtime.UI;

namespace _School_Seducer_.Editor.Scripts.UI
{
    public class GallerySlotView : MonoBehaviour
    {
        [SerializeField] private FigmaImage image;
        [SerializeField] private FigmaImage animationPreview;
        
        private GallerySlotData _data;
        
        public void Render(GallerySlotData data)
        { 
            _data = data;

            image.sprite = data.Sprite;
        }
    }
}