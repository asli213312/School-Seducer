using System;
using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using _School_Seducer_.Editor.Scripts.UI.Gallery;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _School_Seducer_.Editor.Scripts.UI
{
    public class ContentScreen : MonoBehaviour
    {
        [SerializeField] private Image contentWide;
        [SerializeField] private Image contentSquare;
        [SerializeField] private Button leftImageButton;
        [SerializeField] private Button rightImageButton;

        public Image ContentWide => contentWide;
        public Image ContentSquare => contentSquare;

        private GallerySlotData[] _currentSlotsBySection;
        
        private GalleryCharacterData _currentData;
        private GalleryScreen _galleryScreen;

        public void Initialize(GalleryCharacterData currentData, GalleryScreen galleryScreen)
        {
            _currentData = currentData;
            _galleryScreen = galleryScreen;
        }

        public void InstallImagesBySection()
        {
            _currentSlotsBySection = _galleryScreen.GetTotalSlotsInCurrentData();
        }

        public Image GetContentFormat(GallerySlotData clickedSlotData)
        {
            if (clickedSlotData.Sprite.IsWideSprite())
            {
                //contentWide.sprite = clickedSlotData.Sprite;
                //contentWide.gameObject.Activate();
                return contentWide;
            }
            else
            {
                //contentSquare.sprite = clickedSlotData.Sprite;
                //contentSquare.gameObject.Deactivate();
                return contentSquare;
            }
        }
    }
}