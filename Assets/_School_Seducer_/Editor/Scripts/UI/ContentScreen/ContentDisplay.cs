using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using _School_Seducer_.Editor.Scripts.Chat;
using Sirenix.OdinInspector;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace _School_Seducer_.Editor.Scripts.UI
{
    public class ContentDisplay : MonoBehaviour, IContentDisplay, IContentAnimation
    {
        [Header("Components")]
        [SerializeField] private Image contentWide;
        [SerializeField] private Image contentSquare;
        [SerializeField] private SkeletonAnimation contentAnimation;
        [SerializeField] private Transform container;

        public OpenContentBase CurrentContent {get => ContentScreenProxy.CurrentContent; set => ContentScreenProxy.CurrentContent = value;}
        public bool IsSelected { get; private set; }
        
        private int _currentIndexContent;

        private const int NEXT = 1;
        private const int PREVIOUS = -1;

        private Image _currentContentImage;
        private SkeletonAnimation _currentContentAnimation;
        private Button _contentSquareButton;
        private Button _contentWideButton;
        private Button _contentAnimateButton;
        
        private IContentDataProvider _dataProvider;

        public void Initialize(IContentDataProvider dataProvider)
        {
            _dataProvider = dataProvider;

            _contentWideButton = contentWide.GetComponent<Button>();
            _contentSquareButton = contentSquare.GetComponent<Button>();
            _contentAnimateButton = contentAnimation.GetComponent<Button>();
            
            RegisterCloseContent();
        }

        private void OnDestroy()
        {
            UnregisterCloseContent();
        }

        private void Update()
        {
            if (CurrentContent != null && IsSelected == false)
            {
                ShowContent(CurrentContent);
                IsSelected = true;
            }
        }

        public void ShowContent(OpenContentBase content)
        {
            switch (content)
            {
                case OpenContentSprite openContentSprite:
                    SetContentImage(openContentSprite.Content.sprite);
                    _currentContentImage = openContentSprite.Content;
                    break;
                case OpenContentAnimation openContentAnimation:
                    SetContentAnimation(openContentAnimation.Animation.skeletonDataAsset, openContentAnimation.AnimationName);
                    _currentContentAnimation = openContentAnimation.Animation;
                    break;
            }
            
            _currentIndexContent = GetIndexForContent();
            
            container.gameObject.Activate();
        }

        public void SwitchToNextContent()
        {
            ShowContentAtIndex(NEXT);
        }

        public void SwitchToPreviousContent()
        {
            ShowContentAtIndex(PREVIOUS);
        }

        public void EnableAnimation()
        {
            contentAnimation.timeScale = 1;
            contentAnimation.Initialize(true);
        }

        private void ShowContentAtIndex(int indexOffset)
        {
            int newIndex = _currentIndexContent + indexOffset;
            
            if (newIndex >= 0 && newIndex < _dataProvider.ContentList.Count)
            {
                IContent nextSlot = _dataProvider.ContentList[newIndex];

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

        private int GetIndexForContent()
        {
            Debug.Log("Current count slots of content: " + _dataProvider.ContentList.Count);

            foreach (var slot in _dataProvider.ContentList)
            {
                MonoBehaviour monoSlot = slot as MonoBehaviour;
                Debug.Log(monoSlot.name);
            }
            
            if (_dataProvider.ContentList == null) return 0;

            for (int i = 0; i < _dataProvider.ContentList.Count; i++)
            {
                MonoBehaviour slotGO = _dataProvider.ContentList[i] as MonoBehaviour;
                Image slotView = slotGO.GetComponent<Image>();

                if (_dataProvider.ContentList[i] is MessagePictureView)
                {
                    Debug.Log("Founded messagePictureView in content screen");
                    
                    MessagePictureView slotChat = _dataProvider.ContentList[i] as MessagePictureView;
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

        private void SetContentAnimation(SkeletonDataAsset animationData, string animationName)
        {
            contentWide.gameObject.SetActive(false);
            contentSquare.gameObject.SetActive(false);

            contentAnimation.skeletonDataAsset = animationData;
            contentAnimation.AnimationName = animationName;
            contentAnimation.skeletonDataAsset.Clear();
            contentAnimation.Initialize(true);
            contentAnimation.Initialize(true);
            contentAnimation.timeScale = 0;

            contentAnimation.gameObject.SetActive(true);
        }

        private void SetContentImage(Sprite spriteInContent)
        {
            contentAnimation.gameObject.SetActive(false);
            _currentContentAnimation = null;

            if (spriteInContent.IsWideSprite())
            {
                contentWide.sprite = spriteInContent;
                contentWide.gameObject.SetActive(true);
                contentSquare.gameObject.SetActive(false);
            }
            else
            {
                contentSquare.sprite = spriteInContent;
                contentSquare.gameObject.SetActive(true);
                contentWide.gameObject.SetActive(false);
            }
        }

        private void RegisterCloseContent()
        {
            _contentSquareButton.AddListener(ResetContent);
            _contentWideButton.AddListener(ResetContent);
            _contentAnimateButton.AddListener(ResetContent);
        }

        private void UnregisterCloseContent()
        {
            _contentSquareButton.RemoveListener(ResetContent);
            _contentWideButton.RemoveListener(ResetContent);
            _contentAnimateButton.RemoveListener(ResetContent);
        }

        private void ResetContent()
        {
            container.gameObject.Deactivate();
            IsSelected = false;
            CurrentContent = null;
        }
    }
}