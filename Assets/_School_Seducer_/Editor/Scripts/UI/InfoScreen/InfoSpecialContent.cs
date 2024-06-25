using System;
using System.Linq;
using _School_Seducer_.Editor.Scripts.Chat;
using _School_Seducer_.Editor.Scripts.UI.Gallery;
using _School_Seducer_.Editor.Scripts.UI;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _School_Seducer_.Editor.Scripts
{
    public class InfoSpecialContent : MonoBehaviour, IModule<InfoScreenSystem>
    {
        [SerializeField] private SpecialContentPrefab prefab;
        [SerializeField] private ScrollRect scroller;
        [SerializeField] private UnityEvent onClickAvailableContent;
        [SerializeField] private UnityEvent onClickLockedContent;

        private GalleryCharacterData _currentGallery;

        private InfoScreenSystem _system;

        public void InitializeCore(InfoScreenSystem system)
        {
            _system = system;
        }

        public void Initialize() 
        {
            _system.Previewer.CharacterSelectedEvent += OnCharacterSelected;
        }

        private void OnDestroy()
        {
            _system.Previewer.CharacterSelectedEvent -= OnCharacterSelected;
        }

        public void OnCharacterSelected(Character character)
        {
            _currentGallery = character.Data.gallery;
            ResetContent();

            foreach (var content in _currentGallery.AllSlots) 
            {
                if (content.Section != GallerySlotType.Special) return;

                SpecialContentPrefab view = Instantiate(prefab, scroller.content);
                view.Render(content);
                view.OnClick = OnClickContent;
            }
        }

        private void OnClickContent(SpecialContentPrefab selectedContent) 
        {
            if (selectedContent.Data.AddedInGallery)
                onClickAvailableContent?.Invoke();
            else
                onClickLockedContent?.Invoke();
        }

        private void ResetContent() 
        {
            if (scroller.content.childCount > 0) 
            {
                for (var i = 0; i < scroller.content.childCount; i++) 
                {
                    GameObject contentObj = scroller.content.GetChild(i).gameObject;
                    Destroy(contentObj);
                }
            }
        }
    }
}