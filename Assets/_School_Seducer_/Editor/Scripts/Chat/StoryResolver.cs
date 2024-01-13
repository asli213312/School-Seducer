using System;
using System.Collections.Generic;
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
        private СonversationData _rolledConversation;

        public bool StoryUnlocked { get; private set; }
        
        private event Action UpdateStatusViewsEvent;

        public void Initialize()
        {
            UpdateStatusViewsEvent += CheckConversationsAvailable;
            UpdateStatusViewsEvent += InstallSliderNextConversation;
            //UpdateStatusViewsEvent += CheckToUnlockStory;
        }

        public void InitStatusViews(List<ChatStatusView> statusViews)
        {
            _chatStatusViews = statusViews;
        }

        public void SetSliderValue(int characterExperience) => expSlider.value = characterExperience;

        public void SetRolledConversation([CanBeNull] СonversationData rolledConversation)
        {
            _rolledConversation = rolledConversation;
            
            StoryUnlocked = expSlider.value >= expSlider.maxValue;
            
            Debug.Log("Rolled conversation in storyResolver is: " + _rolledConversation?.name);
        }

        public void UpdateStatusViews() => UpdateStatusViewsEvent?.Invoke();

        private void InstallSliderNextConversation()
        {
            if (_rolledConversation == null) return;

            expSlider.maxValue = _rolledConversation.costExp;
            
            StoryUnlocked = expSlider.value >= expSlider.maxValue;
                    
            Debug.Log("Slider will be use conversation: " + _rolledConversation.name);
        }

        private void CheckConversationsAvailable()
        {
            foreach (var statusView in _chatStatusViews)
            {
                if (statusView.Conversation.isUnlocked)
                {
                    statusView.gameObject.Activate();
                }
                else
                    statusView.gameObject.Deactivate();
            }
        }

        public void CheckToUnlockStory()
        {
            if (StoryUnlocked) _rolledConversation.isUnlocked = true;
        }

        private void OnDestroy()
        {
            UpdateStatusViewsEvent -= CheckConversationsAvailable;
            UpdateStatusViewsEvent -= InstallSliderNextConversation;
            //UpdateStatusViewsEvent -= CheckToUnlockStory;
        }
    }
}