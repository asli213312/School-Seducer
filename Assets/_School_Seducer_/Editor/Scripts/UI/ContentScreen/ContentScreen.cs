using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using _School_Seducer_.Editor.Scripts.Chat;
using _School_Seducer_.Editor.Scripts.Utility;
using Sirenix.OdinInspector;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace _School_Seducer_.Editor.Scripts.UI
{
    public class ContentScreen : MonoBehaviour
    {
        [Header("Data")] 
        [SerializeField] private Chat.Chat chat;
        [SerializeField] private GalleryScreen galleryScreen;
        
        [Header("Components")]
        [SerializeField] private Image contentWide;
        [SerializeField] private Image contentSquare;
        [SerializeField] private SkeletonAnimation contentAnimation;
        [SerializeField] private Button leftImageButton;
        [SerializeField] private Button rightImageButton;

        [Header("Options")] 
        [SerializeField] private float durationAnim;

        public bool showDebugParameters;
        [ShowInInspector, ShowIf("showDebugParameters")] public static OpenContentBase CurrentData;
        public GameObject Container => _container;
        
        private const int NEXT = 1;
        private const int PREVIOUS = -1;

        [ShowInInspector] private List<IContent> _currentSlots = new();

        private Button _contentWideButton;
        private Button _contentSquareButton;
        private Button _contentAnimationButton;
        private Button _animateButton;
        
        private GameObject _container;
        private Image _currentContentImage;
        private SkeletonAnimation _currentContentAnimation;
        
        private int _currentIndexContent;
        private bool _isSelected;

        private void OnValidate()
        {
            _container = gameObject.transform.GetChild(0).gameObject;
            _contentWideButton = contentWide.GetComponent<Button>();
            _contentSquareButton = contentSquare.GetComponent<Button>();
            _contentAnimationButton = contentAnimation.GetComponent<Button>();
            _animateButton = contentAnimation.transform.GetChild(0).GetComponent<Button>();
        }

        private void Awake()
        {
            RegisterCloseContent();
            RegisterIterateContent();
            RegisterAnimateButton();
        }

        private void OnDestroy()
        {
            UnregisterCloseContent();
            UnregisterIterateContent();
            UnregisterAnimateButton();
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

            if (CurrentData is OpenContentSprite openContentSprite)
            {
                SetContentImage(openContentSprite.Content.sprite);
                _currentContentImage = openContentSprite.Content;
            }
            else if (CurrentData is OpenContentAnimation openContentAnimation)
            {
                SetContentAnimation(openContentAnimation.Animation.skeletonDataAsset, openContentAnimation.AnimationName);
                _currentContentAnimation = openContentAnimation.Animation;
            }

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

                if (nextSlot is GallerySlotView slotGallery)
                {
                    if (slotGallery.Data.AddedInGallery == false) return;

                    InstallContent(slotGallery.Data);
                }
                else if (nextSlot is MessagePictureView slotChat)
                {
                    SetContentImage(slotChat.CurrentImage.sprite);
                    _currentContentImage = slotChat.CurrentImage;
                    _currentIndexContent = newIndex;

                    if (slotChat.Data.optionalData.GallerySlot == null) return;
                    
                    switch (slotChat.Data.optionalData.GallerySlot.Section)
                    {
                        case GallerySlotType.Video:
                            SetContentAnimation(slotChat.Data.optionalData.GallerySlot.animation, slotChat.Data.optionalData.GallerySlot.GetAnimationName());
                            _currentContentAnimation = contentAnimation;
                            _currentIndexContent = newIndex;
                            break;
                    }
                }

                void InstallContent(GallerySlotData data)
                {
                    switch (data.Section)
                    {
                        case GallerySlotType.Video:
                            SetContentAnimation(data.animation, data.GetAnimationName());
                            _currentContentAnimation = contentAnimation;
                            _currentIndexContent = newIndex;
                            break;
                        case GallerySlotType.Photo:
                            SetContentImage(slotGallery.Data.Sprite);
                            _currentContentImage = slotGallery.GetComponent<Image>();
                            _currentIndexContent = newIndex;
                            break;
                    }
                }
            }
        }

        private void SetContentAnimation(SkeletonDataAsset animationData, string animationName)
        {
            contentWide.gameObject.Deactivate();
            contentSquare.gameObject.Deactivate();
            _currentContentImage = null;

            //contentAnimation.timeScale = 0;
            
            contentAnimation.skeletonDataAsset = animationData;
            contentAnimation.AnimationName = animationName;
            contentAnimation.skeletonDataAsset.Clear();
            contentAnimation.Initialize(true);
            contentAnimation.Initialize(true);
            contentAnimation.timeScale = 0;
            
            contentAnimation.gameObject.Activate();
        }

        private void SetContentImage(Sprite spriteInContent)
        {
            contentAnimation.gameObject.Deactivate();
            _currentContentAnimation = null;

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

                    if (slotChat.Data.optionalData.GallerySlot.animation != null)
                    {
                        if (_currentContentAnimation != null) 
                            if (_currentContentAnimation.skeletonDataAsset != slotChat.Data.optionalData.GallerySlot.animation) 
                                continue;
                        
                        if (_currentContentAnimation != null)
                            if (_currentContentAnimation.skeletonDataAsset ==
                                slotChat.Data.optionalData.GallerySlot.animation)
                                return i;
                    }

                    if (_currentContentImage.sprite != pictureChat.sprite) continue;
                    
                    if (_currentContentImage.sprite == pictureChat.sprite)
                        return i;
                }
                
                if (slotGO is GallerySlotView slotGallery) 
                    if (_currentContentAnimation != null && slotGallery.Data.animation != null)
                        if (_currentContentAnimation.skeletonDataAsset == slotView.GetComponent<OpenContentAnimation>()
                            .Animation.skeletonDataAsset) 
                            return i;

                if (_currentContentImage != null && _currentContentImage.sprite == slotView.sprite)
                {
                    return i;
                }
            }
            
            Debug.LogWarning("Index not found for current content");
            return 0;
        }

        private void RegisterAnimateButton()
        {
            _animateButton.onClick.AddListener(AnimateContent);
        }

        private void UnregisterAnimateButton()
        {
            _animateButton.onClick.RemoveListener(AnimateContent);
        }

        private void AnimateContent()
        {
            contentAnimation.timeScale = 1;
            contentAnimation.Initialize(true);
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
            _contentAnimationButton.RemoveListener(ResetContent);
        }

        private void RegisterCloseContent()
        {
            _contentSquareButton.AddListener(ResetContent);
            _contentWideButton.AddListener(ResetContent);
            _contentAnimationButton.AddListener(ResetContent);
        }

        private void ResetContent()
        {
            _container.gameObject.Deactivate();
            _isSelected = false;
            CurrentData = null;
        } 
    }
}