using System;
using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace _School_Seducer_.Editor.Scripts.UI
{
    [RequireComponent(typeof(Button))]
    public class GallerySectionButton : MonoBehaviour
    {
        public event Action<GallerySectionButton> SectionSelected;
        
        public GallerySlotType TypeSection;

        private Button _button; 

        private void OnValidate()
        {
            _button = GetComponent<Button>();
        }

        private void Awake()
        {
            _button.AddListener(SetSection);
        }

        private void OnDestroy()
        {
            _button.RemoveListener(SetSection);
        }

        private void SetSection()
        {
            SectionSelected?.Invoke(this);
        }
    }
}