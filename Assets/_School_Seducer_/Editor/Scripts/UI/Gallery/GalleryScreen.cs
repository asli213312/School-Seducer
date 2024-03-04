using System.Collections.Generic;
using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using _School_Seducer_.Editor.Scripts.Chat;
using _School_Seducer_.Editor.Scripts.UI.Gallery;
using _School_Seducer_.Editor.Scripts.Utility;
using TMPro;
using UnityEngine;
using Zenject;

namespace _School_Seducer_.Editor.Scripts.UI
{
    public class GalleryScreen : ScreenViewBase
    {
        [Inject] private IContentDataProvider _contentDataProvider;
        
        [Header("Data")]
        [SerializeField] private Chat.Chat chat;
        [SerializeField] private ContentScreen contentScreen;
        [SerializeField] private Transform galleryContent;
        [SerializeField] private TextMeshProUGUI characterName;
        [SerializeField] private GalleryData data;
        [SerializeField] private GallerySlotView slotPhotoPrefab;
        [SerializeField] private GallerySlotView slotVideoPrefab;
        [SerializeField] private GallerySectionButton[] sectionButtons;

        [Header("Counters")]
        [SerializeField] private TextMeshProUGUI photosCountText;
        [SerializeField] private TextMeshProUGUI gamesCountText;
        [SerializeField] private TextMeshProUGUI videosCountText;

        public ContentScreen ContentScreen => contentScreen;
        
        private GallerySlotType _currentType;
        private GalleryCharacterData _currentGalleryData;
        private GallerySlotData _currentSlotData;
        private List<GallerySlotData> _foundedSlotsCurrentCharacter = new();
        private GallerySectionButton _currentActiveSectionButton;

        public void SetCurrentData(GalleryCharacterData currentData)
        {
            _currentGalleryData = currentData;
            data.dampedData = _currentGalleryData;
        }

        private void Awake()
        {
            RegisterSections();
        }

        private void OnDestroy()
        {
            UnregisterSections();
        }

        private void Start()
        {
            InstallDefaultSection();
        }

        private void OnEnable()
        {
            foreach (var section in sectionButtons)
            {
                SetCounterSlots(section.TypeSection);
            }

            characterName.text = chat.CurrentCharacterData.name;
        }

        private void OnDisable()
        {
            if (_foundedSlotsCurrentCharacter.Count > 0) _foundedSlotsCurrentCharacter.Clear();
            if (galleryContent.childCount > 0)
            {
                for (int i = 0; i < galleryContent.childCount; i++)
                {
                    Destroy(galleryContent.GetChild(i).gameObject);
                }
            }
            
            _contentDataProvider.ResetContentList();
        }

        private void InstallDefaultSection()
        {
            SetSlotsByConversation(GallerySlotType.Photo);
            
            this.DelayedCall(.3f,() => _contentDataProvider.LoadContentData(GetTotalSlotsInContent()));
        }

        private void OnSectionSelected(GallerySectionButton sectionButton)
        {
            _currentActiveSectionButton = sectionButton;

            ResetContent();

            _currentType = sectionButton.TypeSection;
            SetSlotsByConversation(sectionButton.TypeSection);
            //SetSlotsByData(sectionButton.TypeSection);
            this.DelayedCall(.3f,() => _contentDataProvider.LoadContentData(GetTotalSlotsInContent()));
        }

        private void SetSlotsByData(GallerySlotType section)
        {
            if (_currentGalleryData.AllSlots.Count == 0) return;
            
            for (int j = 0; j < _currentGalleryData.AllSlots.Count; j++)
            {
                GallerySlotData slotData = _currentGalleryData.AllSlots[j];

                if (slotData.Section == section)
                {
                    GallerySlotView slotView = Instantiate(slotPhotoPrefab, galleryContent);
                    slotView.Render(slotData, data.lockedSlot, data.frameSlot);

                    //SetCounterSlots(section);

                    if (slotData.AddedInGallery)
                    {
                        slotView.gameObject.Activate();
                    }
                }
            }
        }

        private void SetSlotsByConversation(GallerySlotType section)
        {
            for (int j = 0; j < _foundedSlotsCurrentCharacter.Count; j++)
            {
                GallerySlotData slotData = _foundedSlotsCurrentCharacter[j];

                GallerySlotView selectedView = null;

                if (slotData.Section == section)
                {
                    switch (slotData.Section)
                    {
                        case GallerySlotType.Photo: selectedView = slotPhotoPrefab; break;
                        case GallerySlotType.Video: selectedView = slotVideoPrefab; break;
                        case GallerySlotType.Game: selectedView = slotPhotoPrefab; break;
                    }

                    GallerySlotView slotView = Instantiate(selectedView, galleryContent);
                    slotView.Render(slotData, data.lockedSlot, data.frameSlot);
                
                    slotView.GetComponent<OpenContentBase>().SetCondition(new (slotView.Data.AddedInGallery));

                    if (slotData.AddedInGallery && _currentGalleryData.IsOriginalData(slotData))
                    {
                        _currentGalleryData.AddSlotData(slotData);
                    }   
                }
            }
        }

        private void SetCounterSlots(GallerySlotType section)
        {
            switch (section)
            {
                case GallerySlotType.Photo:
                    int totalPhotosCountConversation = GetTotalCountSlotsByConversation(GallerySlotType.Photo);
                    int totalPhotosCount = GetTotalAddedSlotsByType(GallerySlotType.Photo);

                    photosCountText.text = $"{totalPhotosCount}/{totalPhotosCountConversation}";
                    break;

                case GallerySlotType.Game:
                    int totalGamesCountConversation = GetTotalCountSlotsByConversation(GallerySlotType.Game);
                    int totalGamesCountData = GetTotalAddedSlotsByType(GallerySlotType.Game);

                    Debug.Log("totalGamesCountConversation: " + totalGamesCountConversation);
                    Debug.Log("totalGamesCountData: " + totalGamesCountData);

                    gamesCountText.text = $"{totalGamesCountData}/{totalGamesCountConversation}";
                    break;

                case GallerySlotType.Video:
                    //slotView.Render(slotData, );
                    int totalVideosCountConversation = GetTotalCountSlotsByConversation(GallerySlotType.Video);
                    int totalVideosCountData = GetTotalAddedSlotsByType(GallerySlotType.Video);

                    videosCountText.text = $"{totalVideosCountData}/{totalVideosCountConversation}";
                    break;
            }
        }

        private void CheckBranchesForSlots(MessageData branchMessage, GallerySlotType section, ref int countedSlotsByType, HashSet<MessageData> checkedMessages, HashSet<string> checkedBranches)
        {
            if (checkedMessages.Contains(branchMessage))
            {
                return;
            }

            checkedMessages.Add(branchMessage);

            for (int branchIndex = 0; branchIndex < branchMessage.optionalData.Branches.Length; branchIndex++)
            {
                var branch = branchMessage.optionalData.Branches[branchIndex];
                
                if (checkedBranches.Contains(branch.BranchName))
                {
                    Debug.Log($"Skipping branch {branch.BranchName} as it has already been checked.");
                    continue;
                }
                
                Debug.Log($"Checking branch {branch.BranchName}");
                checkedBranches.Add(branch.BranchName);

                for (int innerBranchMessageIndex = 0; innerBranchMessageIndex < branch.Messages.Length; innerBranchMessageIndex++)
                {
                    var innerBranchMessage = branch.Messages[innerBranchMessageIndex];
                    Debug.Log($"Checking branch message {innerBranchMessage.Msg}");

                    if (innerBranchMessage.optionalData.GallerySlot != null)
                    {
                        GallerySlotData branchSlotData = innerBranchMessage.optionalData.GallerySlot;

                        if (branchSlotData.Section == section && branchSlotData.NeedInGallery)
                        {
                            countedSlotsByType++;
                            _foundedSlotsCurrentCharacter.Add(branchSlotData);
                            Debug.Log($"Found a slot of type {section} in branch {branch.BranchName} for message {innerBranchMessage.Msg}.");
                        }
                        else
                        {
                            Debug.Log($"Slot type {branchSlotData.Section} in branch {branch.BranchName} for message {innerBranchMessage.Msg} does not match the desired type {section}.");
                        }
                    }
                    else
                    {
                        Debug.Log($"No slot found in branch {branch.BranchName} for message {innerBranchMessage.Msg}.");
                    }
                    
                    CheckBranchesForSlots(innerBranchMessage, section, ref countedSlotsByType, checkedMessages, checkedBranches);
                }
            }
        }

        private int GetTotalCountSlotsByConversation(GallerySlotType slotType)
        {
            int countedSlotsByType = 0;
            HashSet<MessageData> checkedMessages = new HashSet<MessageData>();
            HashSet<string> checkedBranches = new HashSet<string>();

            foreach (var conversation in chat.CurrentCharacterData.allConversations)
            {
                for (int i = 0; i < conversation.Messages.Length; i++)
                {
                    Debug.Log($"Checking main message {i}");

                    if (conversation.Messages[i].optionalData.GallerySlot != null)
                    {
                        GallerySlotData slotData = conversation.Messages[i].optionalData.GallerySlot;

                        if (slotData.Section == slotType && slotData.NeedInGallery)
                        {
                            countedSlotsByType++;
                            _foundedSlotsCurrentCharacter.Add(slotData);
                            Debug.Log($"Found a slot of type {slotType} in main {i} message.");
                        }
                        else
                        {
                            Debug.Log($"Slot type {slotData.Section} in main {i} message does not match the desired type {slotType}.");
                        }
                    }
                    else
                    {
                        Debug.Log($"No slot found in main {i} message.");
                    }
                
                    if (conversation.Messages[i].optionalData.Branches.Length > 0)
                    {
                        Debug.Log($"Main message {i} has branches.");
                        foreach (var branch in conversation.Messages[i].optionalData.Branches)
                        {
                            Debug.Log($"Checking branch {branch.BranchName}");
                            foreach (var branchMessage in branch.Messages)
                            {
                                Debug.Log($"Checking message {branchMessage.Msg} in {branch.BranchName}");
                                CheckBranchesForSlots(branchMessage, slotType, ref countedSlotsByType, checkedMessages, checkedBranches);
                            }
                        }
                    
                        CheckBranchesForSlots(conversation.Messages[i], slotType, ref countedSlotsByType, checkedMessages, checkedBranches);
                    }
                }
            }

            return countedSlotsByType;
        }

        private int GetCountSlotsAddedByConversation(GallerySlotType typeSlot)
        {
            int countedSlotsByType = 0;
            for (int i = 0; i < chat.CompletedMessagesCurrentConversation.Count; i++)
            {
                if (chat.CompletedMessagesCurrentConversation[i].optionalData.GallerySlot == null) continue;
                
                GallerySlotData slotData = chat.CompletedMessagesCurrentConversation[i].optionalData.GallerySlot;

                if (slotData.Section == typeSlot && slotData.AddedInGallery)
                {
                    countedSlotsByType++;
                }
            }

            return countedSlotsByType;
        }

        public List<IContent> GetTotalSlotsInContent()
        {
            List<IContent> selectedSlots = new List<IContent>();

            for (int i = 0; i < galleryContent.childCount; i++)
            {
                GallerySlotView slot = galleryContent.GetChild(i).GetComponent<GallerySlotView>();
                selectedSlots.Add(slot);
                Debug.Log($"Slot -- {slot.name} -- was added in current list data in gallery");
            }

            return selectedSlots;
        }

        private int GetTotalAddedSlotsByType(GallerySlotType typeSlot)
        {
            int countedSlotsByType = 0;

            foreach (var foundedSlot in _foundedSlotsCurrentCharacter)
            {
                if (foundedSlot.Section != typeSlot) continue;

                if (foundedSlot.AddedInGallery) countedSlotsByType++;
            }

            return countedSlotsByType;
        }

        private int GetTotalCountSlotsInDataByType(GallerySlotType typeSlot)
        {
            int countedSlotsByType = 0;
            
            for (int i = 0; i < _currentGalleryData.AllSlots.Count; i++)
            {
                GallerySlotData slotData = _currentGalleryData.AllSlots[i];
                
                if (slotData.Section == typeSlot)
                {
                    if (slotData.Section == typeSlot)
                        countedSlotsByType++;   
                }
            }

            return countedSlotsByType;
        }

        private void ResetContent()
        {
            for (int i = 0; i < galleryContent.childCount; i++)
            {
                Destroy(galleryContent.GetChild(i).gameObject);
            }
        }

        private void RegisterSections()
        {
            foreach (var sectionButton in sectionButtons)
            {
                sectionButton.SectionSelected += OnSectionSelected;
            }
        }
        
        private void UnregisterSections()
        {
            foreach (var sectionButton in sectionButtons)
            {
                sectionButton.SectionSelected -= OnSectionSelected;
            }
        }
    }
}