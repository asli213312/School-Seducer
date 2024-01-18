using System;
using System.Collections.Generic;
using System.Linq;
using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using _School_Seducer_.Editor.Scripts.UI;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _School_Seducer_.Editor.Scripts.Chat
{
    public class StoryResolver : MonoBehaviour
    {
        [SerializeField] private Slider expSlider;

        private List<ChatStatusView> _chatStatusViews = new();
        private СonversationData _lockedConversation;

        public bool StoryUnlocked { get; private set; }
        
        public event Action UpdateStatusViewsEvent;

        public void Initialize()
        {
            UpdateStatusViewsEvent += CheckConversationsAvailable;
            UpdateStatusViewsEvent += InstallSliderNextConversation;
        }

        public void InitStatusViews(List<ChatStatusView> statusViews)
        {
            _chatStatusViews = statusViews;
        }

        public void SetSliderValue(int characterExperience) => expSlider.value = characterExperience;

        public void SetRolledConversation([CanBeNull] СonversationData rolledConversation)
        {
            _lockedConversation = rolledConversation;

            StoryUnlocked = expSlider.value >= expSlider.maxValue;
            
            Debug.Log("Rolled conversation in storyResolver is: " + _lockedConversation?.name);
        }

        public void UpdateStatusViews() => UpdateStatusViewsEvent?.Invoke();

        public ChatStatusView FindFirstLockedStatusView() => _chatStatusViews.FirstOrDefault(x => x.Conversation.isUnlocked == false);

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
                    statusView.OnUpdateUnlockBar();
                else
                    statusView.HideBarUnlock();
            }
        }

        private void OnDestroy()
        {
            UpdateStatusViewsEvent -= CheckConversationsAvailable;
            UpdateStatusViewsEvent -= InstallSliderNextConversation;
        }
    }
}