using System.Collections.Generic;
using System.Linq;
using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _School_Seducer_.Editor.Scripts.UI
{
    public class ContentScreen : MonoBehaviour
    {
        [Header("Data")] 
        [SerializeField] private GalleryData galleryData;
        [SerializeField] private GalleryScreen galleryScreen;
        
        [Header("Components")]
        [SerializeField] private Image contentWide;
        [SerializeField] private Image contentSquare;
        [SerializeField] private Button leftImageButton;
        [SerializeField] private Button rightImageButton;

        public bool showDebugParameters;
        [ShowInInspector, ShowIf("showDebugParameters")] public static OpenContent CurrentData;

        public Image ContentWide => contentWide;
        public Image ContentSquare => contentSquare;
        private List<GallerySlotView> _currentSlotsBySection;
        public GameObject Container => _container;
        
        private Button _contentWideButton;
        private Button _contentSquareButton;
        
        private GameObject _container;
        private Image _currentContent;

        private int _currentIndexInGallery;
        private bool _isSelected;

        public void Initialize(GalleryScreen gallery)
        {
            galleryScreen = gallery;
        }

        private void OnValidate()
        {
            _container = gameObject.transform.GetChild(0).gameObject;
            _contentWideButton = contentWide.GetComponent<Button>();
            _contentSquareButton = contentSquare.GetComponent<Button>();
        }

        private void Awake()
        {
            RegisterCloseContent();
            RegisterIterateContent();
        }

        private void OnDestroy()
        {
            UnregisterCloseContent();
            UnregisterIterateContent();
        }

        private void Update()
        {
            if (CurrentData != null && _isSelected == false)
            {
                ShowContent();
                _isSelected = true;
            }
        }

        private void InstallSlotsBySection()
        {
            _currentSlotsBySection = galleryScreen.GetTotalSlotsInContent();
        }

        private void ShowContent()
        {
            _isSelected = true;
            Debug.Log("Showing content...");
            
            if (CurrentData == null)
            {
                Debug.Log("CurrentContent is null");
                return;
            }

            if (galleryScreen.gameObject.activeSelf)
                InstallSlotsBySection();
            else
                _currentSlotsBySection = new();

            if (CurrentData.Content.sprite.IsWideSprite())
            {
                contentWide.sprite = CurrentData.Content.sprite;
                contentWide.gameObject.Activate();
                contentSquare.gameObject.Deactivate();
                Debug.Log("Content installed like WIDE");
            }
            else 
            {
                contentSquare.sprite = CurrentData.Content.sprite;
                contentSquare.gameObject.Activate();
                contentWide.gameObject.Deactivate();
                Debug.Log("Content installed like SQUARE");
            }

            _currentContent = CurrentData.Content;
            _currentIndexInGallery = GetIndexForCurrentContent();

            _container.Activate();
        }
        
        private void PreviousContent() 
        {
           if (_currentIndexInGallery > 0)
           {
               GallerySlotView nextSlot = _currentSlotsBySection[_currentIndexInGallery - 1];

               if (nextSlot.Data.Sprite.IsWideSprite())
               {
                   contentWide.sprite = nextSlot.Data.Sprite;
                   contentWide.gameObject.Activate();
                   contentSquare.gameObject.Deactivate();
               }
               else
               {
                   contentSquare.sprite = nextSlot.Data.Sprite;
                   contentSquare.gameObject.Activate();
                   contentWide.gameObject.Deactivate();
               }

               _currentContent = nextSlot.GetComponent<Image>();

               _currentIndexInGallery--;
           }
        }
        
        private void NextContent()
        {
            if (_currentIndexInGallery < _currentSlotsBySection.Count - 1)
            {
                GallerySlotView nextSlot = _currentSlotsBySection[_currentIndexInGallery + 1];

                if (nextSlot.Data.Sprite.IsWideSprite())
                {
                    contentWide.sprite = nextSlot.Data.Sprite;
                    contentWide.gameObject.Activate();
                    contentSquare.gameObject.Deactivate();
                }
                else
                {
                    contentSquare.sprite = nextSlot.Data.Sprite;
                    contentSquare.gameObject.Activate();
                    contentWide.gameObject.Deactivate();
                }

                _currentContent = nextSlot.GetComponent<Image>();

                _currentIndexInGallery++;
            }
        }

        private int GetIndexForCurrentContent()
        {
            if (_currentSlotsBySection == null) return 0;
            
            for (int i = 0; i < _currentSlotsBySection.Count; i++)
            {
                Image slotView = _currentSlotsBySection[i].GetComponent<Image>();
                if (_currentContent.sprite == slotView.sprite)
                {
                    return i;
                }
            }
            
            Debug.LogWarning("Index not found for current content");
            return 0;
        }

        private void RegisterIterateContent()
        {
            rightImageButton.AddListener(NextContent);
            leftImageButton.AddListener(PreviousContent);
        }

        private void UnregisterIterateContent()
        {
            rightImageButton.RemoveListener(NextContent);
            leftImageButton.RemoveListener(PreviousContent);
        }

        private void UnregisterCloseContent()
        {
            _contentSquareButton.RemoveListener(ResetContent);
            _contentWideButton.RemoveListener(ResetContent);
        }

        private void RegisterCloseContent()
        {
            _contentSquareButton.AddListener(ResetContent);
            _contentWideButton.AddListener(ResetContent);
        }

        private void ResetContent()
        {
            _container.gameObject.Deactivate();
            _isSelected = false;
            CurrentData = null;
        } 
    }
}