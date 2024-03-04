using System;
using System.Collections;
using System.Collections.Generic;
using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using _School_Seducer_.Editor.Scripts.UI;
using _School_Seducer_.Editor.Scripts.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace _School_Seducer_.Editor.Scripts.Chat
{
    public class ChatMessageProcessorBase : MonoBehaviour, IChatMessageProcessorModule
    {
        [Inject] private EventManager _eventManager;
        [Inject] private IContentDataProvider _contentDataProvider;
        
        [SerializeField] private RectTransform paddingPrefab;
        
        [ShowInInspector] public List<IContent> PictureMessages { get; private set; } = new();
        [ShowInInspector] public List<IContent> DampedPictureMessages { get; private set; } = new();
        
        public СonversationData CurrentConversation => _chatSystem.Initializator.CurrentCharacter.CurrentConversation;
        public List<MessageData> Messages { get; private set; } = new();
        public List<MessageData> CompletedMessagesCurrentConversation { get; private set; } = new();
        public List<MessageData> CompletedMessages { get; private set; } = new();

        public event Action TapEvent;
        
        private ChatSystem _chatSystem;
        private MessageDefaultViewProxy _previousDefaultMsg;
        private IChatStateHandlerModule _stateHandlerModule;

        private List<MessageDefaultViewProxy> _defaultMessages = new();

        public void InitializeCore(ChatSystem chatSystem)
        {
            _chatSystem = chatSystem;

            _stateHandlerModule = _chatSystem.StateHandler;
        }

        public GameObject GetUtilityObject() => Instantiate(paddingPrefab.gameObject, _chatSystem.ContentMsg);

        public void StartProcessMessages(MessageData[] messages)
        {
            if (messages == null) StartCoroutine(ChatTap(() => InvokeLoadMessages(CurrentConversation.Messages)));
            else InvokeLoadMessages(messages);
        }

        private void InvokeLoadMessages(MessageData[] messages)
        {
            StartCoroutine(LoadMessages(messages));
        }

        private IEnumerator LoadMessages(MessageData[] messages = null)
        {
            messages = CheckIsBranch(messages);
            
            _chatSystem.Translator.UpdateMessages(Messages);
            _chatSystem.Translator.TranslateDefaultMessages(_defaultMessages);

            if (messages != null)
                Messages.AddRange(messages);
            
            _chatSystem.Translator.OnObservableUpdate();

            for (int i = 0; i < Messages.Count; i++)
            {
                if (_stateHandlerModule.CheckEndConversation()) StopCoroutine(ProcessMessage(i, Messages[i]));
                    
                yield return ProcessMessage(i, Messages[i]);
                
                if (DampedPictureMessages.Count <= 0)
                    this.DelayedCall(.3f, () => _contentDataProvider.LoadContentData(PictureMessages));
                else
                    this.DelayedCall(.3f, () => _contentDataProvider.LoadContentData(DampedPictureMessages));
            }

            _stateHandlerModule.TryEndConversation(CheckLastMessage(Messages.ToArray(), Messages.Count - 1, false));
            _stateHandlerModule.EndConversation();
        }

        private IEnumerator ProcessMessage(int index, MessageData messageData)
        {
            _eventManager.ChatMessageReceived(false);
            
            if (CompletedMessagesCurrentConversation.Count <= 0) StopCoroutine(ProcessMessage(index, messageData));
            
            if (CompletedMessagesCurrentConversation.Contains(Messages[index]))
            {
                yield break;
            }
            
            CheckOptionsParentIsLastSibling();
            
            yield return new WaitUntil(CheckTapBounds);
            yield return new WaitForSeconds(0.5f);

            var paddingBack = CreatePadding();
            var newMsg = _chatSystem.MessageRenderer.InstallPrefabMsg(messageData);
            var paddingForward = CreatePadding();

            if (newMsg == null) yield break;
            
            _chatSystem.MessageRenderer.RenderMessage(newMsg, messageData, _chatSystem.ContentMsg);
            
            if (newMsg is MessagePictureViewProxy msgPicture)
            {
                paddingBack.gameObject.Deactivate();
                
                if (_previousDefaultMsg.PaddingForward != null)
                    _previousDefaultMsg.PaddingForward.Deactivate();
            }
            else if (newMsg is MessageDefaultViewProxy msgDefault) 
            {
                _previousDefaultMsg = msgDefault;
                _defaultMessages.Add(msgDefault);
            }

            _chatSystem.ChatConfig.OnMessageReceived?.Invoke();
            _eventManager.ChatMessageReceived(true);
            
            CheckOptionsParentIsLastSibling();
            
            yield return new WaitForSeconds(_chatSystem.ChatConfig.DelayBtwMessage);
            
            if (MsgHasBranches(messageData, index))
            {
                Debug.Log("options installed in MsgHasBranches");

                if (newMsg is IMessageCondition messageCond)
                {
                    while (messageCond.CheckCondition())
                    {
                        messageCond.ResolveCondition();
                        _chatSystem.UIManager.SetContentByMessageData(messageData);
                    }   
                }

                _chatSystem.UIManager.SetContentByMessageData(messageData);
                
                CompletedMessagesCurrentConversation.Add(Messages[index]);
                CompletedMessages.Add(Messages[index]);
                
                yield break;
            }

            if (Messages[index] == Messages[^1])
            {
                _stateHandlerModule.EndConversation();
            }

            var lastMessage = CheckLastMessage(Messages.ToArray(), index, false);

            if (newMsg is IMessageCondition messageCondition)
            {
                while (messageCondition.CheckCondition())
                {
                    messageCondition.ResolveCondition();
                }   
            }

            if (lastMessage)
            {
                CurrentConversation.isCompleted = true;
                _eventManager.ChatMessageReceived(false);
            }
            
            CompletedMessagesCurrentConversation.Add(Messages[index]);
            CompletedMessages.Add(Messages[index]);
        }

        private IEnumerator ChatTap(Action onTapped)
        {
            yield return new WaitUntil(CheckTapBounds);
            onTapped?.Invoke();
            TapEvent?.Invoke();
        }

        private bool CheckTapBounds()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                
                BoxCollider2D collider = gameObject.GetComponent<BoxCollider2D>();
                
                RaycastHit2D[] hits = Physics2D.RaycastAll(mousePosition, Vector2.zero, Mathf.Infinity);

                foreach (var hit in hits)
                {
                    GameObject hitObject = hit.collider.gameObject;
                    if (hitObject.gameObject.layer == LayerMask.NameToLayer("Unclickable"))
                    {
                        Debug.Log("Clicked on Unclickable area!");
                        return false;
                    }

                    return true;
                }
            }

            return false;
        }
        
        private void CheckOptionsParentIsLastSibling()
        {
            Transform optionsContainer = _chatSystem.ContentMsg.Find("Options");

            if (optionsContainer != null)
            {
                optionsContainer.SetAsLastSibling();
            }
        }
        
        private MessageData[] CheckIsBranch(MessageData[] messages)
        {
            messages ??= CurrentConversation.Messages;
            Debug.Log("messages found");
            return messages;
        }
        
        private bool CheckLastMessage(MessageData[] messages, int i, bool lastMessage)
        {
            if (i == messages.Length - 1)
            {
                lastMessage = true;
            }
            
            return lastMessage;
        }
        
        private bool MsgHasBranches(MessageData messageData, int i)
        {
            if (messageData.optionalData.Branches.Length <= 0) return false;
            
            foreach (var branch in messageData.optionalData.Branches)
            {
                if (branch != null) break;
            }
            
            return true;
        }
        
        private RectTransform CreatePadding()
        {
            return Instantiate(paddingPrefab, _chatSystem.ContentMsg);
        }
    }
}