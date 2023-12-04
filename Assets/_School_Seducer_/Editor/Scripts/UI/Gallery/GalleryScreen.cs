using System.Collections.Generic;
using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using _School_Seducer_.Editor.Scripts.Chat;
using _School_Seducer_.Editor.Scripts.UI.Gallery;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace _School_Seducer_.Editor.Scripts.UI
{
    public class GalleryScreen : ScreenViewBase
    {
        [Header("Data")]
        [SerializeField] private Chat.Chat chat;
        [SerializeField] private Transform galleryContent;
        [SerializeField] private GallerySlotView slotPrefab;
        [FormerlySerializedAs("data")] [SerializeField] private GalleryCharacterData characterData;
        [SerializeField] private GallerySectionButton[] sectionButtons;
        [Header("Counters")]
        [SerializeField] private TextMeshProUGUI photosCountText;
        [SerializeField] private TextMeshProUGUI gamesCountText;
        [SerializeField] private TextMeshProUGUI videosCountText;

        private GallerySlotData _currentSlotData;
        private List<GallerySlotData> _foundedSlotsInConversation = new();
        private GallerySectionButton _currentActiveSectionButton;

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

        private void InstallDefaultSection()
        {
            SetSlotsByConversation(GallerySlotType.Photo);
        }

        private void OnSectionSelected(GallerySectionButton sectionButton)
        {
            _currentActiveSectionButton = sectionButton;

            ResetContent();

            SetSlotsByConversation(sectionButton.TypeSection);
            SetSlotsByData(sectionButton.TypeSection);
        }

        private void SetSlotsByData(GallerySlotType typeSlot)
        {
            for (int j = 0; j < characterData.AllSlots.Count; j++)
            {
                GallerySlotData slotData = characterData.AllSlots[j];

                if (slotData.Type == typeSlot)
                {
                    GallerySlotView slotView = Instantiate(slotPrefab, galleryContent);
                    slotView.Render(slotData);
                    
                    switch (typeSlot)
                    {
                        case GallerySlotType.Photo:
                            int totalPhotosCountConversation = GetTotalCountSlotsByConversation(GallerySlotType.Photo);
                            int totalPhotosCount = GetTotalCountSlotsInDataByType(GallerySlotType.Photo);
                            
                            photosCountText.text = $"{totalPhotosCount}/{totalPhotosCountConversation}";
                            break;
                        
                        case GallerySlotType.Game:
                            int currentGamesCountInConversationAdded = GetCountSlotsAddedByConversation(GallerySlotType.Game);
                            int totalGamesCountConversation = GetTotalCountSlotsByConversation(GallerySlotType.Game);
                            int totalGamesCountData = GetTotalCountSlotsInDataByType(GallerySlotType.Game);
                            
                            Debug.Log("totalGamesCountConversation: " + totalGamesCountConversation);
                            Debug.Log("totalGamesCountData: " + totalGamesCountData);
                                
                            gamesCountText.text = $"{totalGamesCountData}/{totalGamesCountConversation}";
                            break;
                        
                        case GallerySlotType.Video:
                            slotView.Render(slotData);
                            int totalVideosCountConversation = GetTotalCountSlotsByConversation(GallerySlotType.Video);
                            int totalVideosCountData = GetTotalCountSlotsInDataByType(GallerySlotType.Video);
                            
                            videosCountText.text = $"{totalVideosCountData}/{totalVideosCountConversation}";
                            break;
                    }

                    if (slotData.AddedInGallery)
                    {
                        slotView.gameObject.Activate();
                    }
                    else
                        slotView.gameObject.Destroy();
                }
            }
        }

        private void SetSlotsByConversation(GallerySlotType typeSlot)
        {
            for (int j = 0; j < chat.CompletedMessages.Count; j++)
            {
                if (chat.CompletedMessages[j].optionalData.GallerySlot != null)
                {
                    GallerySlotData slotData = chat.CompletedMessages[j].optionalData.GallerySlot;

                    if (slotData.Type == typeSlot)
                    {
                        GallerySlotView slotView = Instantiate(slotPrefab, galleryContent);
                        slotView.Render(slotData);
                        
                        switch (typeSlot)
                        {
                            case GallerySlotType.Photo:
                                int totalPhotosCountConversation = GetTotalCountSlotsByConversation(GallerySlotType.Photo);
                                int totalPhotosCount = GetTotalCountSlotsInDataByType(GallerySlotType.Photo);

                                photosCountText.text = $"{totalPhotosCount}/{totalPhotosCountConversation}";
                                break;
                        
                            case GallerySlotType.Game:
                                int totalGamesCountConversation = GetTotalCountSlotsByConversation(GallerySlotType.Game);
                                int totalGamesCountData = GetTotalCountSlotsInDataByType(GallerySlotType.Game);
                            
                                Debug.Log("totalGamesCountConversation: " + totalGamesCountConversation);
                                Debug.Log("totalGamesCountData: " + totalGamesCountData);
                                
                                gamesCountText.text = $"{totalGamesCountData}/{totalGamesCountConversation}";
                                break;
                        
                            case GallerySlotType.Video:
                                int totalVideosCountConversation = GetTotalCountSlotsByConversation(GallerySlotType.Video);
                                int totalVideosCountData = GetTotalCountSlotsInDataByType(GallerySlotType.Video);
                            
                                videosCountText.text = $"{totalVideosCountData}/{totalVideosCountConversation}";
                                break;
                        }
                        
                        if (slotData.AddedInGallery && characterData.IsOriginalData(slotData))
                        {
                            slotView.gameObject.Destroy();
                            characterData.AddSlotData(slotData);
                        }
                        else
                            slotView.gameObject.Destroy();
                    }
                }
            }
        }

        private void CheckBranchesForSlots(MessageData branchMessage, GallerySlotType slotType, ref int countedSlotsByType, HashSet<MessageData> checkedMessages, HashSet<string> checkedBranches)
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

                        if (branchSlotData.Type == slotType && branchSlotData.NeedInGallery)
                        {
                            countedSlotsByType++;
                            _foundedSlotsInConversation.Add(branchSlotData);
                            Debug.Log($"Found a slot of type {slotType} in branch {branch.BranchName} for message {innerBranchMessage.Msg}.");
                        }
                        else
                        {
                            Debug.Log($"Slot type {branchSlotData.Type} in branch {branch.BranchName} for message {innerBranchMessage.Msg} does not match the desired type {slotType}.");
                        }
                    }
                    else
                    {
                        Debug.Log($"No slot found in branch {branch.BranchName} for message {innerBranchMessage.Msg}.");
                    }
                    
                    CheckBranchesForSlots(innerBranchMessage, slotType, ref countedSlotsByType, checkedMessages, checkedBranches);
                }
            }
        }

        private int GetTotalCountSlotsByConversation(GallerySlotType slotType)
        {
            int countedSlotsByType = 0;
            HashSet<MessageData> checkedMessages = new HashSet<MessageData>();
            HashSet<string> checkedBranches = new HashSet<string>();

            for (int i = 0; i < chat.CurrentConversation.Messages.Length; i++)
            {
                Debug.Log($"Checking main message {i}");

                if (chat.CurrentConversation.Messages[i].optionalData.GallerySlot != null)
                {
                    GallerySlotData slotData = chat.CurrentConversation.Messages[i].optionalData.GallerySlot;

                    if (slotData.Type == slotType && slotData.NeedInGallery)
                    {
                        countedSlotsByType++;
                        _foundedSlotsInConversation.Add(slotData);
                        Debug.Log($"Found a slot of type {slotType} in main {i} message.");
                    }
                    else
                    {
                        Debug.Log($"Slot type {slotData.Type} in main {i} message does not match the desired type {slotType}.");
                    }
                }
                else
                {
                    Debug.Log($"No slot found in main {i} message.");
                }
                
                if (chat.CurrentConversation.Messages[i].optionalData.Branches.Length > 0)
                {
                    Debug.Log($"Main message {i} has branches.");
                    foreach (var branch in chat.CurrentConversation.Messages[i].optionalData.Branches)
                    {
                        Debug.Log($"Checking branch {branch.BranchName}");
                        foreach (var branchMessage in branch.Messages)
                        {
                            Debug.Log($"Checking message {branchMessage.Msg} in {branch.BranchName}");
                            CheckBranchesForSlots(branchMessage, slotType, ref countedSlotsByType, checkedMessages, checkedBranches);
                        }
                    }
                    
                    CheckBranchesForSlots(chat.CurrentConversation.Messages[i], slotType, ref countedSlotsByType, checkedMessages, checkedBranches);
                }
            }

            return countedSlotsByType;
        }

        private int GetCountSlotsAddedByConversation(GallerySlotType typeSlot)
        {
            int countedSlotsByType = 0;
            for (int i = 0; i < chat.CompletedMessages.Count; i++)
            {
                if (chat.CompletedMessages[i].optionalData.GallerySlot == null) continue;
                
                GallerySlotData slotData = chat.CompletedMessages[i].optionalData.GallerySlot;

                if (slotData.Type == typeSlot && slotData.AddedInGallery)
                {
                    countedSlotsByType++;
                }
            }

            return countedSlotsByType;
        }

        private int GetTotalCountSlotsInDataByType(GallerySlotType typeSlot)
        {
            int countedSlotsByType = 0;
            
            for (int i = 0; i < characterData.AllSlots.Count; i++)
            {
                GallerySlotData slotData = characterData.AllSlots[i];
                
                if (slotData.Type == typeSlot)
                {
                    if (slotData.Type == typeSlot)
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