using System.Collections.Generic;
using System.Linq;
using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using _School_Seducer_.Editor.Scripts.Chat;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _School_Seducer_.Editor.Scripts.UI
{
    public class ContentScreen : MonoBehaviour
    {
        [Header("Data")] 
        [SerializeField] private Chat.Chat chat;
        [SerializeField] private GalleryData galleryData;
        [SerializeField] private GalleryScreen galleryScreen;
        
        [Header("Components")]
        [SerializeField] private Image contentWide;
        [SerializeField] private Image contentSquare;
        [SerializeField] private Button leftImageButton;
        [SerializeField] private Button rightImageButton;

        public bool showDebugParameters;
        [ShowInInspector, ShowIf("showDebugParameters")] public static OpenContent CurrentData;
        public GameObject Container => _container;
        
        private const int NEXT = 1;
        private const int PREVIOUS = -1;

        [ShowInInspector] private List<IContent> _currentSlots = new();

        private Button _contentWideButton;
        private Button _contentSquareButton;
        
        private GameObject _container;
        private Image _currentContent;
        
        private int _currentIndexContent;
        private bool _isSelected;

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

        private void InstallSlots()
        {
            if (galleryScreen.gameObject.activeSelf)
            {
                if (_currentSlots.Count > 0) _currentSlots.Clear();
                _currentSlots = galleryScreen.GetTotalSlotsInContent();
            }
            else if (chat.gameObject.activeSelf)
            {
                if (_currentSlots.Count > 0) _currentSlots.Clear();
                
                if (chat.PictureMessages.Count != 0)
                {
                    //_currentSlots.AddRange(chat.DampedPictureMessages);
                    _currentSlots.AddRange(chat.PictureMessages);
                }
                else
                {
                    _currentSlots = chat.DampedPictureMessages;
                }
            }
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

            InstallSlots();

            SetContent(CurrentData.Content.sprite);

            _currentContent = CurrentData.Content;

            _currentIndexContent = GetIndexForCurrentContent();

            _container.Activate();
        }

        private void PreviousContent()
        {
            SwitchContent(PREVIOUS);
        }

        private void NextContent()
        {
            SwitchContent(NEXT);
        }

        private void SwitchContent(int indexOffset)
        {
            int newIndex = _currentIndexContent + indexOffset;

            if (newIndex >= 0 && newIndex < _currentSlots.Count)
            {
                IContent nextSlot = _currentSlots[newIndex];

                if (nextSlot is GallerySlotView)
                {
                    GallerySlotView slotGallery = nextSlot as GallerySlotView;
                    SetContent(slotGallery.Data.Sprite);
                    _currentContent = slotGallery.GetComponent<Image>();
                    _currentIndexContent = newIndex;    
                }
                else if (nextSlot is MessagePictureView)
                {
                    MessagePictureView slotChat = nextSlot as MessagePictureView;
                    SetContent(slotChat.CurrentImage.sprite);
                    _currentContent = slotChat.CurrentImage;
                    _currentIndexContent = newIndex;
                }
            }
        }

        private void SetContent(Sprite spriteInContent)
        {
            if (spriteInContent.IsWideSprite())
            {
                contentWide.sprite = spriteInContent;
                contentWide.gameObject.Activate();
                contentSquare.gameObject.Deactivate();
                Debug.Log("Content installed like WIDE");
            }
            else
            {
                contentSquare.sprite = spriteInContent;
                contentSquare.gameObject.Activate();
                contentWide.gameObject.Deactivate();
                Debug.Log("Content installed like SQUARE");
            }
        }

        private int GetIndexForCurrentContent()
        {
            Debug.Log("Current count slots of content: " + _currentSlots.Count);

            foreach (var slot in _currentSlots)
            {
                MonoBehaviour monoSlot = slot as MonoBehaviour;
                Debug.Log(monoSlot.name);
            }
            
            if (_currentSlots == null) return 0;

            for (int i = 0; i < _currentSlots.Count; i++)
            {
                MonoBehaviour slotGO = _currentSlots[i] as MonoBehaviour;
                Image slotView = slotGO.GetComponent<Image>();

                if (_currentSlots[i] is MessagePictureView)
                {
                    Debug.Log("Founded messagePictureView in content screen");
                    
                    MessagePictureView slotChat = _currentSlots[i] as MessagePictureView;
                    Image pictureChat = slotChat.CurrentImage;
                    
                    Debug.Log("Sprite in pictureMessage: " + pictureChat.sprite.name);
                    
                    if (_currentContent.sprite == pictureChat.sprite)
                        return i;
                }

                if (slotView != null && _currentContent.sprite == slotView.sprite)
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