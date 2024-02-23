using System;
using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using _School_Seducer_.Editor.Scripts.Utility;
using TMPro;
using UnityEngine;
using Zenject;

namespace _School_Seducer_.Editor.Scripts.Chat.Refactor
{
    public class ChatUIManager : MonoBehaviour, IChatUIManagerModule
    {
        [Inject] private EventManager _eventManager;
        
        [SerializeField] private OptionButton[] options;

        private ChatSystem _chatSystem;
        private ChatConfig _chatConfig;
        
        public void InitializeCore(ChatSystem chatSystem)
        {
            if (chatSystem == null) return;
            
            _chatSystem = chatSystem;
            _chatConfig = _chatSystem.ChatConfig;

            RegisterOptions();
        }

        public void SetContent()
        {
            //throw new System.NotImplementedException();
        }

        public void SetContentByMessageData(MessageData messageData)
        {
            if (_chatSystem == null) return;
            
            SetOptions(messageData);  
            InstallOptions(messageData);
        }

        private void InstallOptions(MessageData msgData)
        {
            for (int i = 0; i < options.Length; i++)
            {
                float mainHeight = _chatConfig.MainHeight;

                if (msgData.optionalData.Branches.Length == 1)
                {
                    Debug.Log("Only one option: " + msgData.optionalData.Branches.Length);
                    Vector2 newSize = new Vector2(_chatConfig.OneOptionWidth, mainHeight);
                        
                    for (int j = 0; j < options.Length; j++)
                    {
                        RectTransform transformOption = options[0].GetComponent<RectTransform>();
                        transformOption.sizeDelta = newSize;
                        options[0].gameObject.Activate();
                    }
                }
                else if (msgData.optionalData.Branches.Length == 2)
                {
                    Vector2 newSize = new Vector2(_chatConfig.TwoOptionsWidth, mainHeight);
                    
                    for (int j = 0; j < Mathf.Min(2, options.Length); j++)
                    {
                        RectTransform transformOption = options[j].GetComponent<RectTransform>();
                        transformOption.sizeDelta = newSize;
                        options[j].gameObject.Activate();
                    }
                }
                else if (msgData.optionalData.Branches.Length == 3)
                {
                    Vector2 newSize = new Vector2(_chatConfig.ThreeOptionsWidth, mainHeight);
                    
                    for (int j = 0; j < Mathf.Min(3, options.Length); j++)
                    {
                        RectTransform transformOption = options[j].GetComponent<RectTransform>();
                        transformOption.sizeDelta = newSize;
                        options[j].gameObject.Activate();
                    }
                }
                
                options[i].transform.parent.gameObject.Activate();
            }

            //TranslateOptions(optionButtons, _localizer.GlobalLanguageCodeRuntime);
            
            this.DelayedBoolCall(1.5f, boolParameter => _eventManager.ChatMessageReceived(boolParameter),  false);
        }
        
        protected void SetOptions(MessageData data)
        {
            for (int i = 0; i < options.Length && i < data.optionalData.Branches.Length; i++)
            {
                if (data.optionalData.Branches[i] != null)
                {
                    options[i].BranchData = data.optionalData.Branches[i];
                    TextMeshProUGUI textChildren = options[i].GetComponentInChildren<TextMeshProUGUI>();
                    textChildren.text = options[i].BranchData.BranchName;
                }
            }
        }

        private void RegisterOptions()
        {
            foreach (var option in options)
            {
                option.LoadMessagesEvent += _chatSystem.MessageProcessor.StartProcessMessages;
            }
        }

        private void OnDestroy()
        {
            UnregisterOptions();
        }

        private void UnregisterOptions()
        {
            foreach (var option in options)
            {
                option.LoadMessagesEvent -= _chatSystem.MessageProcessor.StartProcessMessages;
            }
        }
    }
}