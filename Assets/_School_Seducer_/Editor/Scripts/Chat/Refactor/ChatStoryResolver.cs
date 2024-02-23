using System;
using System.Collections.Generic;
using System.Linq;
using _School_Seducer_.Editor.Scripts.UI;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace _School_Seducer_.Editor.Scripts.Chat.Refactor
{
    public class ChatStoryResolver : MonoBehaviour, IChatStoryResolverModule
    {
        [SerializeField] private Transform contentStatusViews;
        [SerializeField] private Slider expSlider;
        [SerializeField] private ChatStatusView statusViewPrefab;
        
        public bool StoryUnlocked { get; private set; }

        public event Action UpdateStatusViewsEvent;
        
        private readonly List<ChatStatusView> _chatStatusViews = new();
        private ChatStatusView _currentStatusView;
        private СonversationData _lockedConversation;

        private ChatSystem _chatSystem;

        public void InitializeCore(ChatSystem chatSystem)
        {
            _chatSystem = chatSystem;
            
            UpdateStatusViewsEvent += CheckConversationsAvailable;
            UpdateStatusViewsEvent += InstallSliderNextConversation;
        }

        public void Initialize()
        {
            if (_chatStatusViews.Count > 0)
            {
                _chatStatusViews.Clear();
                for (int i = 0; i < contentStatusViews.childCount; i++)
                {
                    Destroy(contentStatusViews.GetChild(i).gameObject);
                }
            }
            
            foreach (var conversation in _chatSystem.Initializator.CurrentCharacter.Data.allConversations)
            {
                ChatStatusView chatStatus = Instantiate(statusViewPrefab, contentStatusViews);
                chatStatus.Render(conversation, _chatSystem.ChatConfig.ChatUncompletedSprite);
                chatStatus.SelectEvent += OnSelectStatusView;
                _chatStatusViews.Add(chatStatus);
            }

            UpdateView();
            
            CheckConversationsAvailable();
        }

        public void SetSliderValue(int characterExperience) => expSlider.value = characterExperience;

        public void SetRolledConversation([CanBeNull] СonversationData rolledConversation)
        {
            _lockedConversation = rolledConversation;

            StoryUnlocked = expSlider.value >= expSlider.maxValue;
            
            Debug.Log("Rolled conversation in storyResolver is: " + _lockedConversation?.name);
        }

        public void UpdateView() => UpdateStatusViewsEvent?.Invoke();

        private void OnSelectStatusView(ChatStatusView statusView)
        {
            if (_chatSystem.ContentMsg.childCount > 1)
            {
                Debug.LogWarning("Selected current conversation");
                return;
            }
            
            _chatSystem.StateHandler.LoadMessages(statusView.Conversation.Messages);
            
            Debug.Log("Chat Status View is selected!");
        }

        private ChatStatusView FindFirstLockedStatusView() => _chatStatusViews.FirstOrDefault(x => x.Conversation.isUnlocked == false);

        private void InstallSliderNextConversation()
        {
            if (_lockedConversation == null) return;

            expSlider.maxValue = _lockedConversation.costExp;
            
            StoryUnlocked = expSlider.value >= expSlider.maxValue;
                    
            Debug.Log("Slider will be use conversation: " + _lockedConversation.name);
        }

        private void CheckConversationsAvailable()
        {
            foreach (var statusView in _chatStatusViews)
            {
                if (statusView == FindFirstLockedStatusView())
                    statusView.OnUpdateUnlockBar(_chatSystem.Initializator.CurrentCharacter.Data.experience);
                else
                    statusView.HideBarUnlock();
            }
        }

        private void OnDestroy()
        {
            foreach (var statusView in _chatStatusViews)
            {
                statusView.SelectEvent += OnSelectStatusView;
            }
            
            UpdateStatusViewsEvent -= CheckConversationsAvailable;
            UpdateStatusViewsEvent -= InstallSliderNextConversation;
        }
    }
}