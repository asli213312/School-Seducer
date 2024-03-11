using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using _School_Seducer_.Editor.Scripts.UI;
using _School_Seducer_.Editor.Scripts.Utility;
using _School_Seducer_.Editor.Scripts.Utility.Attributes;
using _School_Seducer_.Editor.Scripts.Utility.Translation;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;

namespace _School_Seducer_.Editor.Scripts.Chat
{
    public class Chat : MonoBehaviour, IObservableCustom<MonoBehaviour>, ICharacterSelected
    {
        [Serializable]
        private class MessagesContainer
        {
            [SerializeField] public СonversationData conversation;
            [SerializeField] public List<MessageData> messages = new();
            [SerializeField] public List<MessageViewBase> messagesView = new();
        }

        [Serializable]
        private class CharacterChat
        {
            [SerializeField] public CharacterData characterData;
            [SerializeField] public MessagesContainer chatData;
        }
        
        [Inject] private LocalizedGlobalMonoBehaviour _localizer;
        [Inject] private SoundHandler _soundHandler;
        [Inject] private EventManager _eventManager;
        [Inject] private MonoController _monoController;
        [Inject] private IContentDataProvider _contentDataProvider;

        [Header("UI")] 
        [SerializeField] private Transform chatLocker;
        [SerializeField] private Transform contentMsgs;
        [SerializeField] private Transform contentChats;
        [SerializeField] private GalleryScreen gallery;
        [SerializeField] private Previewer previewer;
        [SerializeField] private StoryResolver storyResolver;
        [SerializeField] private OptionButton[] optionButtons;

        [Header("Data")] 
        [SerializeField] private ChatConfig config;
        [SerializeField] private ChatStatusView chatStatusView;
        [SerializeField] private MessageDefaultView msgDefaultPrefab;
        [SerializeField] private MessagePictureView msgPicturePrefab;
        [SerializeField] private RectTransform paddingPrefab;

        [Header("Events")] 
        [SerializeField] private UnityEvent chatOpenedEvent;
        [SerializeField] private UnityEvent storySelectedEvent;
        [ShowInInspector] public List<IContent> PictureMessages { get; private set; } = new();
        [ShowInInspector] public List<IContent> DampedPictureMessages { get; private set; } = new();

        [MonoText] public CharacterData CurrentCharacterData { get; private set; }
        public Transform ContentMsgs => contentMsgs;
        public BranchData CurrentBranchData { get; private set; }
        [MonoText] public СonversationData CurrentConversationData { get; private set; }
        public bool IsMessagesEnded { get; private set; }
        public ChatConfig Config => config;
        public List<MessageData> CompletedMessagesCurrentConversation { get; private set; } = new();
        public List<MessageData> CompletedMessages { get; private set; } = new();
        public List<MessageData> Messages { get; private set; } = new();

        private MessagesContainer _currentRenderedChat;
        private List<MessageViewBase> _allMessages = new();
        private List<MessagesContainer> _renderedChats = new();
        private List<ChatStatusView> _chatStatusViews = new();
        private List<MessageDefaultView> _defaultMessages = new();
        private ChatStatusView _currentStatusView;
        private ConversationView _conversationView;
        private MessageDefaultView _currentDefaultMsg;
        private MessageDefaultView _previousDefaultMsg;
        private MessagePictureView _previousPictureMsg;
        private MessagePictureView _currentPictureMsg;
        private GameObject _paddingForTranslate;
        private GameObject _previousPaddingBack;
        
        private bool _isStartConversation;
        private int _iteratedMessages;

        [ContextMenu("Translate Messages")]
        private void TranslateRenderedMessages()
        {
            if (_defaultMessages.Count < 0) return;

            foreach (var msg in _defaultMessages)
            {
                msg.TranslateText(_localizer.GlobalLanguageCodeRuntime);
                msg.TranslateAudio(_localizer.GlobalLanguageCodeRuntime);
            }
        }
        
        private void StorySelected() => storySelectedEvent?.Invoke();

        private void Awake()
        {
            config.OnConversationEnd.AddListener(previewer.AddDiamondOnConversationEnd);
            config.OnMessageReceived.AddListener(previewer.ReduceMoneyPlayer);

            RegisterOptions();

            _conversationView = new ConversationView();
            _localizer.AddObserver(this);

            _eventManager.UpdateExperienceTextEvent += storyResolver.UpdateStatusViews;
            previewer.CharacterSelectedEvent += OnCharacterSelected;
            //_conversationView.Initialize(config.ChatCompletedSprite, config.ChatUncompletedSprite);
        }

        private void OnDestroy()
        {
            config.OnConversationEnd.RemoveListener(previewer.AddDiamondOnConversationEnd);
            config.OnMessageReceived.RemoveListener(previewer.ReduceMoneyPlayer);

            UnRegisterOptions();

            _localizer.RemoveObserver(this);
            
            _eventManager.UpdateExperienceTextEvent -= storyResolver.UpdateStatusViews;
            previewer.CharacterSelectedEvent -= OnCharacterSelected;
        }

        private void OnDisable()
        {
            UnRegisterStatusViews();
        }

        public void OnCharacterSelected(Character character)
        {
            if (previewer.CurrentCharacter != character)
            {
                //DeleteContentObjects();
                ResetRenderedContent();
                //ResetContent();
            }

            if (_renderedChats.Count > 0) _renderedChats.Clear();
            
            //ResetContent();
            //DeleteContentObjects();
            ResetRenderedContent();

            gameObject.SafeActivate(.1f);
            Debug.Log("Chat activated!");
            
            DeactivateStatusViews();
            ResetStatusViews();
            InstallCharacterData(character.Data);
            
            StartCoroutine(LoadStartupConversationsCurrentCharacter());
            chatLocker.gameObject.Activate();

            chatOpenedEvent?.Invoke();
        }
        
        public void OnObservableUpdate()
        {
            if (Messages.Count < 0) return;
            
            TranslateRenderedMessages();

            StartCoroutine(WaitUntilTranslated());

            IEnumerator WaitUntilTranslated()
            {
                yield return new WaitUntil(() =>
                {
                    bool completed = false;
                    
                    foreach (var msg in Messages)
                    {
                        msg.Msg = msg.TranslateTextMsg(_localizer.GlobalLanguageCodeRuntime);
                        var translatedMsg = msg.TranslateAudioMsg(_localizer.GlobalLanguageCodeRuntime);
                        msg.AudioMsg = translatedMsg.translatedAudio;

                        if (msg == Messages[^1]) completed = true;
                    }

                    return completed;
                });
            }
            
            //var paddingBack = CreatePadding();
            //paddingBack.gameObject.Deactivate(0.1f);
            //paddingBack.name = "PaddingTranslate";

            //_paddingForTranslate = paddingBack.gameObject;
        }

        public void LoadBranch(BranchData branchData)
        {
            CurrentBranchData = branchData;
            StartCoroutine(LoadMessages(branchData.Messages));
        }

        public void InstallCurrentStatusView(ChatStatusView statusView)
        {
            if (_currentStatusView != null) _currentStatusView.ResetSelected();
            
            _currentStatusView = statusView;
            _currentStatusView.ActivateSelected();
        }

        private IEnumerator LoadStartupConversationsCurrentCharacter()
        {
            if (CurrentCharacterData == null)
            {
                Debug.LogError("Current character is null in chat to load conversations!");
                yield break;
            }
            
            DeleteContentObjects();

            yield return new WaitForSeconds(1f);

            List<СonversationData> conversations = CurrentCharacterData.allConversations;

            if (_currentRenderedChat != null)
            {
                bool needEnd = false;
                
                foreach (var conversation in conversations)
                {
                    foreach (var existRenderedChat in _renderedChats)
                    {
                        if (conversation != existRenderedChat.conversation)
                        {
                            needEnd = true;
                            break;
                        }
                    }       
                }

                if (needEnd)
                {
                    Debug.Log("<color=red>Founded new conversation. End loading startup conversations!</color>");
                    yield break;
                }
            }
            
            _soundHandler.Mute();

            yield return new WaitUntil(() =>
            {
                bool loadingComplete = false;

                foreach (var conversation in conversations)
                {
                    CurrentConversationData = conversation;
                    
                    MessagesContainer chatContainer = new MessagesContainer();
                    
                    List<MessageData> renderedMessagesData = new();
                    
                    for (int i = 0; i < conversation.Messages.Length; i++)
                    {
                        if (conversation.Messages[i].completed)
                        {
                            StartCoroutine(ProcessMessage(conversation.Messages[i], conversation.Messages, false));

                            if (renderedMessagesData.Contains(conversation.Messages[i]) == false)
                            {
                                renderedMessagesData.Add(conversation.Messages[i]);
                            }
                        }
                        
                        if (conversation == conversations[^1] && i == conversation.Messages.Length - 1) 
                            loadingComplete = true;
                    }

                    chatContainer.conversation = conversation;
                    chatContainer.messages = renderedMessagesData;
                    
                    Debug.Log("Conversation rendered: " + conversation.name);
                    Debug.Log("Rendered messages for conversation: " + conversation.name + " messages count: " + renderedMessagesData.Count);

                    if (_renderedChats.Contains(chatContainer) == false)
                        _renderedChats.Add(chatContainer);
                }

                chatLocker.gameObject.Deactivate(0.5f);
                return loadingComplete;
            });

            foreach (var renderedChat in _renderedChats)
            {
                for (int i = 0; i < renderedChat.messages.Count; i++)
                {
                    MessageViewBase foundedRenderedMsgView = _allMessages.FirstOrDefault(x => x.Data == renderedChat.messages[i]);
                    
                    if (foundedRenderedMsgView is null) continue;
                    
                    if (renderedChat.messagesView.Contains(foundedRenderedMsgView))
                    {
                        Debug.Log("Message already rendered: " + foundedRenderedMsgView.name);
                        continue;
                    }
                    
                    renderedChat.messagesView.Add(foundedRenderedMsgView);
                    Debug.Log("Message was added to rendered view messages: " + foundedRenderedMsgView.name, foundedRenderedMsgView.gameObject);

                    renderedChat.messagesView[i].PaddingForwardChat = foundedRenderedMsgView.PaddingForwardChat;
                        
                    if (renderedChat.messagesView[i] is MessageDefaultView defaultMsg)
                    {
                        if (defaultMsg.MainParent != null) defaultMsg.MainParent.gameObject.Deactivate();
                        else defaultMsg.gameObject.Deactivate();
                    }
                    else if (renderedChat.messagesView[i] is MessagePictureView)
                    {
                        renderedChat.messagesView[i].gameObject.Deactivate();
                    }
                        
                    renderedChat.messagesView[i].PaddingForwardChat?.gameObject.Deactivate();
                        
                    Debug.Log("Rendered message view: " + foundedRenderedMsgView.name +" + added to chat: " + renderedChat.conversation.name, foundedRenderedMsgView.gameObject);
                }
            }
        }

        private void UpdateRenderedMsg(MessageViewBase msgView, MessageData msgData)
        {
            if (_currentRenderedChat == null)
            {
                Debug.LogWarning("Rendered chat is null to update rendered msg!");
                return;
            }

            if (_currentRenderedChat.messagesView.Contains(msgView) == false)
                _currentRenderedChat.messagesView.Add(msgView);
            
            if (_currentRenderedChat.messages.Contains(msgData) == false)
                _currentRenderedChat.messages.Add(msgData);
        }

        private void InstallCharacterData(CharacterData character)
        {
            DeactivateStatusViews();
            CurrentCharacterData = character;

            if (_chatStatusViews.Count <= 0)
            {
                ResetStatusViews();
                InitializeChats();
            }

            storyResolver.UpdateStatusViews();

            Debug.Log("Character Data is installed");
        }

        private void StartDialogue()
        {
            if (_currentStatusView != null && CurrentConversationData == _currentStatusView.Conversation && contentMsgs.childCount > 1)
            {
                Debug.LogWarning("Selected current conversation");
                return;
            }
            
            InstallCurrentConversation(_currentStatusView.Conversation);

            ResetRenderedContent();
            //ResetContent();

            foreach (var renderedChat in _renderedChats)
            {
                if (renderedChat.conversation.name != _currentStatusView.Conversation.name) continue;

                _currentRenderedChat = renderedChat;
                
                _eventManager.ChatMessageReceived(false);
                _soundHandler.StopClip();

                if (_currentStatusView.Conversation.Messages.Any(x => x.completed))
                {
                    InvokeStartConversation(_currentStatusView.Conversation.Messages);
                }
                else
                    StartCoroutine(ChatTap(() => InvokeStartConversation(_currentStatusView.Conversation.Messages)));
                
                break;

                /*if (_currentStatusView.Conversation.Messages.Any(x => x.completed))
                {
                    InvokeStartConversation(_currentStatusView.Conversation.Messages);
                
                }
                else
                    StartCoroutine(ChatTap(() => InvokeStartConversation(_currentStatusView.Conversation.Messages)));*/   
            }
        }

        private void InvokeStartConversation(MessageData[] messages)
        {
            StopCoroutine(LoadMessages(_currentStatusView.Conversation.Messages));
            StartCoroutine(LoadMessages(messages));
        }

        private void InstallCurrentConversation(СonversationData data) 
        {
            CurrentConversationData = data;
            CurrentCharacterData.currentConversation = CurrentConversationData;
            
            _monoController.UpdateMonoByName("ChatConversation");
        }

        private void InitializeChats()
        {
            for (int i = 0; i < CurrentCharacterData.allConversations.Count; i++)
            {
                ChatStatusView chatStatus = Instantiate(chatStatusView, contentChats);
                chatStatus.Initialize(this);
                chatStatus.Render(CurrentCharacterData.allConversations[i], config.ChatUncompletedSprite);
                chatStatus.OnClick += StartDialogue;
                chatStatus.OnClick += StorySelected;
                _chatStatusViews.Add(chatStatus);
            }
            
            storyResolver.InitStatusViews(_chatStatusViews);
            storyResolver.UpdateStatusViews();
            gallery.SetCurrentData(previewer.GetCurrentCharacterData().gallery);
        }

        private IEnumerator LoadMessages(MessageData[] messages = null)
	    {
            messages = CheckIsBranch(messages);

            MessagesStarted();
            StartConversation();
            
            Messages.AddRange(messages);
            
            OnObservableUpdate();
            
            ResetMsgPictures();
            
            for (int i = 0; i < Messages.Count; i++)
            {
                if (IsMessagesEnded) StopCoroutine(ProcessMessage(i));

                MessageViewBase renderedMsgView = null;
                
                foreach (var msgView in _currentRenderedChat.messagesView)
                {
                    if (msgView is null) continue;
                    if (messages[i] != msgView.Data) continue;
                    
                    if (msgView is MessageDefaultView defaultMsg)
                    {
                        if (defaultMsg.MainParent != null) defaultMsg.MainParent.gameObject.Activate();
                        else if (defaultMsg.MessageSender == MessageSender.ActorLeft)
                        {
                            defaultMsg.gameObject.Activate();
                        }
                        
                        if (defaultMsg.PreviousDefaultMsg != null) 
                        {
                            if (defaultMsg.PreviousDefaultMsg.PaddingForward != null && defaultMsg.MessageSender != MessageSender.ActorLeft)
                                defaultMsg.PreviousDefaultMsg.PaddingForward.gameObject.Deactivate();
                            //else if (defaultMsg.PreviousDefaultMsg.PaddingForward != null)
                                //defaultMsg.PreviousDefaultMsg.PaddingForward.gameObject.Activate();
                        }

                        if (defaultMsg.MessageSender == MessageSender.ActorLeft)
                        {
                            if (defaultMsg.PreviousDefaultMsg != null && defaultMsg.PreviousDefaultMsg.MessageSender != MessageSender.ActorLeft)
                            {
                                defaultMsg.PreviousDefaultMsg.PaddingForward.gameObject.Activate();
                                Debug.Log("Deactivated padForward is <color=green>founded</color> for prev default msg of LEFT msg", defaultMsg.gameObject);
                            }
                            else
                            {
                                defaultMsg?.PreviousDefaultMsg?.PaddingForward?.gameObject.Deactivate();
                                Debug.Log("Prev default msg && his padForward is <color=red>null</color> for LEFT msg to deactivate padForward", defaultMsg.gameObject);
                            }

                            if (defaultMsg.PreviousDefaultMsg != null && defaultMsg.PreviousDefaultMsg.MessageSender == MessageSender.ActorRight)
                            	defaultMsg.PaddingForwardChat.gameObject.Deactivate();

                        	if (_previousPictureMsg.MessageSender == MessageSender.ActorLeft) 
                        	{
	                        	_previousPictureMsg.PaddingForwardChat.gameObject.Activate();
                        	}

                        }
                        else if (defaultMsg.MessageSender == MessageSender.ActorRight)
                        {
                            if (defaultMsg.PreviousDefaultMsg != null && defaultMsg.PreviousDefaultMsg.MessageSender == MessageSender.ActorLeft)
                            {
                                defaultMsg.PreviousDefaultMsg.PaddingForwardChat.gameObject.Deactivate();
                            }
                            
                            if (defaultMsg.PreviousDefaultMsg != null && defaultMsg.PreviousDefaultMsg.MessageSender == MessageSender.StoryTeller)
                                defaultMsg.PreviousDefaultMsg.PaddingForwardChat.gameObject.Deactivate();

                            if (_previousPictureMsg.MessageSender == MessageSender.ActorLeft)
                            	_previousPictureMsg.PaddingForwardChat.gameObject.Activate();
                        }
                        
                        _previousDefaultMsg = defaultMsg;
                    }
                    else if (msgView is MessagePictureView pictureMsg)
                    {
                        msgView.gameObject.Activate();
                        
                        PictureMessages.Add(pictureMsg);
                        DampedPictureMessages.Add(pictureMsg);

                        if (_previousDefaultMsg != null)  
                        {
							if (_previousDefaultMsg.MessageSender == MessageSender.ActorRight) 
                        	{
                        		//_previousDefaultMsg.PaddingForward.gameObject.Activate();
                        	}
                        	else if (_previousDefaultMsg.MessageSender == MessageSender.ActorLeft) 
                        	{
                        		_previousDefaultMsg.PaddingForwardChat.gameObject.Deactivate();
                        	}
                        }

                        if (_previousPictureMsg.MessageSender == MessageSender.ActorLeft)
                        	pictureMsg.PaddingForwardChat.gameObject.Deactivate();
                        	

                        _previousPictureMsg = pictureMsg;
                    }
                    
                    msgView.PaddingForwardChat?.gameObject.Activate();
                        
                    renderedMsgView = msgView;
                    _soundHandler.StopClip();
                    break;
                }
                
                foreach (var otherRenderedChat in _renderedChats)
                {
                    if (otherRenderedChat == _currentRenderedChat) continue;
                            
                    foreach (var otherChatMsgView in otherRenderedChat.messagesView)
                    {
                        otherChatMsgView.PaddingForwardChat.gameObject.Deactivate();

                        if (otherChatMsgView is MessageDefaultView otherDefaultMsg)
                        {
                            if (otherDefaultMsg.PreviousDefaultMsg == null) continue;
                            if (otherDefaultMsg.PreviousDefaultMsg.PaddingForward == null) continue;
                            
                            otherDefaultMsg.PreviousDefaultMsg.PaddingForward.gameObject.Deactivate();
                            
                            if (otherDefaultMsg.PaddingForward == null) continue;
                            
                            otherDefaultMsg.PaddingForward.gameObject.Deactivate();
                        }
                    }
                }

                if (renderedMsgView != null && renderedMsgView.Data == messages[i])
                {
                    Debug.Log("Skipped rendered msg: " + i, renderedMsgView.gameObject);
                    continue;
                }

                //yield return new WaitUntil( () => _soundHandler.IsClipPlaying() == false);
                _soundHandler.Unmute();
                
                OnObservableUpdate();
                
                if (messages[i].completed == false)
                {
                    yield return ProcessMessage(i);
                }

                if (DampedPictureMessages.Count <= 0)
                    this.DelayedCall(.3f, () => _contentDataProvider.LoadContentData(PictureMessages));
                else
                    this.DelayedCall(.3f, () => _contentDataProvider.LoadContentData(DampedPictureMessages));
            }

            /*for (int i = 0; i < Messages.Count; i++)
            {
                if (IsMessagesEnded) StopCoroutine(ProcessMessage(i));
                
                if (messages[0].completed) chatLocker.gameObject.Activate();

                if (messages[i].completed == false)
                {
                    chatLocker.gameObject.Deactivate();
                    yield return ProcessMessage(i);
                }
                else
                {
                    yield return ProcessMessage(i, false);
                    
                    if (messages[^1].completed) chatLocker.gameObject.Deactivate();
                }

                if (DampedPictureMessages.Count <= 0)
                    this.DelayedCall(.3f, () => _contentDataProvider.LoadContentData(PictureMessages));
                else
                    this.DelayedCall(.3f, () => _contentDataProvider.LoadContentData(DampedPictureMessages));
            }*/

            EndConversation(CheckLastMessage(Messages.ToArray(), Messages.Count - 1, false));
            MessagesEnded();
        }
        
        private IEnumerator ProcessMessage(MessageData messageData, MessageData[] messages, bool needTapToNext = true)
        {
            _eventManager.ChatMessageReceived(false);
            
            //if (CompletedMessagesCurrentConversation.Count <= 0) StopCoroutine(ProcessMessage(index));
            
            //if (CompletedMessagesCurrentConversation.Contains(Messages[index]))
            //{
                //yield break;
            //}

            CheckOptionsParentIsLastSibling();

            if (needTapToNext)
            {
                yield return new WaitUntil(CheckTapBounds);
                yield return new WaitForSeconds(0.5f);
            }

            var paddingBack = CreatePadding();
            paddingBack.gameObject.name = "PaddBackChat";
            paddingBack.gameObject.Deactivate();
            var newMsg = InstallPrefabMsg(messageData, out var pictureMsgProxy);
            var paddingForward = CreatePadding();
            paddingForward.name = "PaddForwardChat";

            newMsg.Initialize(optionButtons);
            RenderMsgData(messageData, newMsg);

            if (newMsg is MessagePictureView msgPicture)
            {
                paddingForward.gameObject.Deactivate();

                if (_previousDefaultMsg.PaddingForward != null) 
                {
                    _previousDefaultMsg.PaddingForward.Deactivate();
                }
                
                msgPicture.PaddingForwardChat = paddingForward;
                msgPicture.PaddingBackChat = paddingBack;
                msgPicture.gameObject.Deactivate();

                _previousPictureMsg = msgPicture;
            }
            else if (newMsg is MessageDefaultView msgDefault)
            {
                msgDefault.PaddingForwardChat = paddingForward;
                msgDefault.PaddingBackChat = paddingBack;

                if (msgDefault.MessageSender == MessageSender.ActorLeft)
                {
                    //msgDefault.transform.SetParent(contentMsgs.parent);
                    //msgDefault.transform.position = new Vector3(0, 999, 0);
                    //yield return new WaitForSeconds(0.1f);
                    //msgDefault.transform.SetParent(contentMsgs);
                }

                if (_previousDefaultMsg != null)
                {
                    if (_previousDefaultMsg.PaddingForward != null && msgDefault.MessageSender != MessageSender.ActorLeft)
                        _previousDefaultMsg.PaddingForward.gameObject.Deactivate();
                    
                    msgDefault.PreviousDefaultMsg = _previousDefaultMsg;
                    msgDefault.NeedPreviousDefaultMsg = msgDefault.PreviousDefaultMsg.PaddingForward != null &&
                                                        msgDefault.MessageSender != MessageSender.ActorLeft;

                    // if (msgDefault.MessageSender != MessageSender.ActorLeft && _previousDefaultMsg.MessageSender == MessageSender.ActorLeft)
                    //     paddingBack.gameObject.Deactivate();
                    // else if (msgDefault.MessageSender == MessageSender.ActorLeft && _previousDefaultMsg.MessageSender != MessageSender.ActorLeft)
                    //     paddingBack.gameObject.Deactivate();
                    // else if (msgDefault.MessageSender == MessageSender.ActorRight && _previousDefaultMsg.MessageSender == MessageSender.ActorRight)
                    //     paddingBack.gameObject.Deactivate();
                    // else if (msgDefault.MessageSender == MessageSender.ActorRight && _previousDefaultMsg.MessageSender == MessageSender.ActorLeft)
                    //     paddingBack.gameObject.Deactivate();
                    
                    if (msgDefault.MessageSender == MessageSender.ActorRight && _previousDefaultMsg.MessageSender == MessageSender.ActorLeft)
                    {
                        msgDefault.PaddingForward.gameObject.Deactivate();

                        msgDefault.NeedPadding = msgDefault.MessageSender == MessageSender.ActorRight && msgDefault.PreviousDefaultMsg.MessageSender == MessageSender.ActorLeft;
                        //paddingBack.gameObject.Activate();
                    }
                }
                _previousDefaultMsg = msgDefault;
                _previousPaddingBack = paddingBack.gameObject;

                //_defaultMessages.Add(msgDefault);
            }

            Debug.Log("Render completed");

            config.OnMessageReceived?.Invoke();
            _eventManager.ChatMessageReceived(true);

            CheckOptionsParentIsLastSibling();

            if (needTapToNext) yield return new WaitForSeconds(config.DelayBtwMessage);

            if (MsgHasBranches(messageData))
            {
                Debug.Log("options installed in MsgHasBranches");

                if (pictureMsgProxy != null)
                {
                    yield return new WaitUntil(() => pictureMsgProxy.PictureInstalled);
                    InstallOptions(messageData);
                }

                InstallOptions(messageData);

                _iteratedMessages++;

                messageData.completed = true;
                CompletedMessagesCurrentConversation.Add(messageData);
                CompletedMessages.Add(messageData);
                if (needTapToNext == false) _eventManager.UpdateScrollChat();

                Debug.Log("iterated messages: " + _iteratedMessages);
                yield break;
            }

            if (messageData == messages[^1])
            {
                MessagesEnded();
            }

            var lastMessage = CheckLastMessage(messageData, messages, false);

            while (pictureMsgProxy != null && !pictureMsgProxy.PictureInstalled)
            {
                yield return new WaitForSeconds(1f);
            }

            if (lastMessage)
            {
                CurrentConversationData.isCompleted = true;
                _eventManager.ChatMessageReceived(false);
            }

            _iteratedMessages++;
            CompletedMessagesCurrentConversation.Add(messageData);
            CompletedMessages.Add(messageData);
            messageData.completed = true;
            if (needTapToNext == false) _eventManager.UpdateScrollChat();

            Debug.Log("iterated messages: " + _iteratedMessages);
        }
        
        private IEnumerator ProcessMessage(int index, bool needTapToNext = true)
        {
            _eventManager.ChatMessageReceived(false);
            
            //if (CompletedMessagesCurrentConversation.Count <= 0) StopCoroutine(ProcessMessage(index));
            
            //if (CompletedMessagesCurrentConversation.Contains(Messages[index]))
            //{
                //yield break;
            //}

            CheckOptionsParentIsLastSibling();

            if (needTapToNext)
            {
                yield return new WaitUntil(CheckTapBounds);
                yield return new WaitForSeconds(0.5f);
            }

            var paddingBack = CreatePadding();
            paddingBack.gameObject.name = "PaddBackChat";
            paddingBack.gameObject.Deactivate();
            var newMsg = InstallPrefabMsg(Messages[index], out var pictureMsgProxy);
            var paddingForward = CreatePadding();
            paddingForward.name = "PaddForwardChat";

            newMsg.Initialize(optionButtons);
            RenderMsgData(Messages[index], newMsg);
            
            UpdateRenderedMsg(newMsg as MessageViewBase, Messages[index]);

            if (newMsg is MessagePictureView msgPicture)
            {
                paddingForward.gameObject.Deactivate();
                
                if (_previousDefaultMsg.PaddingForward != null)
                    _previousDefaultMsg.PaddingForward.Deactivate();
                
                msgPicture.PaddingForwardChat = paddingForward;
            }
            else if (newMsg is MessageDefaultView msgDefault)
            {
                msgDefault.PaddingForwardChat = paddingForward;
                
                if (msgDefault.MessageSender == MessageSender.ActorLeft)
                {
                    msgDefault.transform.SetParent(contentMsgs.parent);
                    msgDefault.transform.position = new Vector3(0, 999, 0);
                    yield return new WaitForSeconds(0.1f);
                    msgDefault.transform.SetParent(contentMsgs);
                }

                if (_previousDefaultMsg != null)
                {
                    if (_previousDefaultMsg.PaddingForward != null && msgDefault.MessageSender != MessageSender.ActorLeft)
                        _previousDefaultMsg.PaddingForward.gameObject.Deactivate();
                    
                    msgDefault.PreviousDefaultMsg = _previousDefaultMsg;
                    msgDefault.NeedPreviousDefaultMsg = msgDefault.PreviousDefaultMsg.PaddingForward != null &&
                                                        msgDefault.MessageSender != MessageSender.ActorLeft;

                    // if (msgDefault.MessageSender != MessageSender.ActorLeft && _previousDefaultMsg.MessageSender == MessageSender.ActorLeft)
                    //     paddingBack.gameObject.Deactivate();
                    // else if (msgDefault.MessageSender == MessageSender.ActorLeft && _previousDefaultMsg.MessageSender != MessageSender.ActorLeft)
                    //     paddingBack.gameObject.Deactivate();
                    // else if (msgDefault.MessageSender == MessageSender.ActorRight && _previousDefaultMsg.MessageSender == MessageSender.ActorRight)
                    //     paddingBack.gameObject.Deactivate();
                    // else if (msgDefault.MessageSender == MessageSender.ActorRight && _previousDefaultMsg.MessageSender == MessageSender.ActorLeft)
                    //     paddingBack.gameObject.Deactivate();

                    if (msgDefault.MessageSender == MessageSender.ActorRight && _previousDefaultMsg.MessageSender == MessageSender.ActorLeft)
                    {
                        msgDefault.PaddingForward.gameObject.Deactivate();
                        //paddingBack.gameObject.Activate();
                        
                        msgDefault.NeedPadding = msgDefault.MessageSender == MessageSender.ActorRight && _previousDefaultMsg.MessageSender == MessageSender.ActorLeft;
                    }

                    if (msgDefault.MessageSender == MessageSender.ActorLeft && _previousDefaultMsg.MessageSender == MessageSender.ActorRight)
                    	_previousDefaultMsg.PaddingForward.gameObject.Deactivate();

                }
                _previousDefaultMsg = msgDefault;
                _previousPaddingBack = paddingBack.gameObject;

                //_defaultMessages.Add(msgDefault);
            }

            Debug.Log("Render completed");

            config.OnMessageReceived?.Invoke();
            _eventManager.ChatMessageReceived(true);

            CheckOptionsParentIsLastSibling();

            if (needTapToNext) yield return new WaitForSeconds(config.DelayBtwMessage);

            if (MsgHasBranches(Messages[index]))
            {
                Debug.Log("options installed in MsgHasBranches");

                if (pictureMsgProxy != null)
                {
                    yield return new WaitUntil(() => pictureMsgProxy.PictureInstalled);
                    InstallOptions(Messages[index]);
                }

                InstallOptions(Messages[index]);

                _iteratedMessages++;

                Messages[index].completed = true;
                CompletedMessagesCurrentConversation.Add(Messages[index]);
                CompletedMessages.Add(Messages[index]);
                if (needTapToNext == false) _eventManager.UpdateScrollChat();

                Debug.Log("iterated messages: " + _iteratedMessages);
                yield break;
            }

            if (Messages[index] == Messages[^1])
            {
                MessagesEnded();
            }

            var lastMessage = CheckLastMessage(Messages.ToArray(), index, false);

            while (pictureMsgProxy != null && !pictureMsgProxy.PictureInstalled)
            {
                yield return new WaitForSeconds(1f);
            }

            if (lastMessage)
            {
                CurrentConversationData.isCompleted = true;
                _eventManager.ChatMessageReceived(false);
            }

            _iteratedMessages++;
            CompletedMessagesCurrentConversation.Add(Messages[index]);
            CompletedMessages.Add(Messages[index]);
            Messages[index].completed = true;
            if (needTapToNext == false) _eventManager.UpdateScrollChat();

            Debug.Log("iterated messages: " + _iteratedMessages);
        }

        private IEnumerator ChatTap(UnityAction onTapped)
        {
            yield return new WaitUntil(CheckTapBounds);
            onTapped?.Invoke();
        }

        private bool CheckTapBounds()
        {
            if (Input.GetMouseButtonDown(0) && transform.localScale == Vector3.one)
            {
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                RaycastHit2D[] hits = Physics2D.RaycastAll(mousePosition, Vector2.zero, Mathf.Infinity);
                
                if (ContentScreen.CurrentData != null)
                {
                    BoxCollider2D content = gallery.ContentScreen.Container.GetComponent<BoxCollider2D>();

                    if (content.OverlapPoint(mousePosition))
                    {
                        Debug.Log("Clicked on Unclickable area!");
                        return false;
                    }
                }

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

        private void MessagesEnded()
        {
            IsMessagesEnded = true;
            _eventManager.ChatMessagesEnded();
        }

        private void MessagesStarted()
        {
            IsMessagesEnded = false;
            _eventManager.ChatMessagesStarted();
        }

        private void EndConversation(bool isLastMessage = false)
        {
            if (isLastMessage)
            {
                config.OnConversationEnd?.Invoke();
                IsMessagesEnded = true;
            }
        }

        private void CheckOptionsParentIsLastSibling()
        {
            Transform optionsTransform = contentMsgs.Find("Options");

            if (optionsTransform != null)
            {
                optionsTransform.SetAsLastSibling();
            }
        }

        private void StartConversation()
        {
            config.OnConversationStart?.Invoke();

            if (_isStartConversation == false) _isStartConversation = true;
        }

        private bool CheckLastMessage(MessageData currentMessage, MessageData[] messages, bool lastMessage)
        {
            if (currentMessage == messages[^1])
            {
                lastMessage = true;
            }
            
            return lastMessage;
        }

        private bool CheckLastMessage(MessageData[] messages, int i, bool lastMessage)
        {
            if (i == messages.Length - 1)
            {
                lastMessage = true;
            }
            
            return lastMessage;
        }

        private bool MsgHasBranches(MessageData messageData)
        {
            if (messageData.optionalData.Branches.Length > 0)
            {
                for (int j = 0; j < messageData.optionalData.Branches.Length; j++)
                {
                    if (messageData.optionalData.Branches[j] != null) break;
                }
                return true;
            }

            return false;
        }

        private void RenderMsgData(MessageData message, IMessage newMsg)
        {
            try
            {
                newMsg.RenderGeneralData(message,
                    CurrentConversationData.ActorLeftSprite,
                    CurrentConversationData.ActorRightSprite,
                    config.StoryTellerSprite,
                    config.NeedIconStoryTeller);
            }
            catch (Exception e)
            {
                Debug.Log(e);
                throw;
            }

            if (newMsg is not MessageDefaultView defaultMsg) { return; }

            InstallColorFontAndBackground();
            InstallBackgroundMsg();

            void InstallColorFontAndBackground()
            {
                defaultMsg.SetFontColor(config.leftActorColorMsg, config.rightActorColorMsg, config.storyTellerColorMsg);
            }
            void InstallBackgroundMsg()
            {
                defaultMsg.SetBlockBackground(config.leftActorBlockMsg, config.rightActorBlockMsg, config.storyTellerBlockMsg);
            }
        }

        private void InstallOptions(MessageData msgData)
        {
            InstallTranslationOptionsByLastMsg(msgData, optionButtons);
            
            for (int i = 0; i < optionButtons.Length; i++)
            {
                float mainHeight = config.MainHeight;

                if (msgData.optionalData.Branches.Length == 1)
                {
                    Debug.Log("Only one option: " + msgData.optionalData.Branches.Length);
                    Vector2 newSize = new Vector2(config.OneOptionWidth, mainHeight);
                        
                    for (int j = 0; j < optionButtons.Length; j++)
                    {
                        RectTransform transformOption = optionButtons[0].GetComponent<RectTransform>();
                        transformOption.sizeDelta = newSize;
                        optionButtons[0].gameObject.Activate();
                    }
                }
                else if (msgData.optionalData.Branches.Length == 2)
                {
                    Vector2 newSize = new Vector2(config.TwoOptionsWidth, mainHeight);
                    
                    for (int j = 0; j < Mathf.Min(2, optionButtons.Length); j++)
                    {
                        RectTransform transformOption = optionButtons[j].GetComponent<RectTransform>();
                        transformOption.sizeDelta = newSize;
                        optionButtons[j].gameObject.Activate();
                    }
                }
                else if (msgData.optionalData.Branches.Length == 3)
                {
                    Vector2 newSize = new Vector2(config.ThreeOptionsWidth, mainHeight);
                    
                    for (int j = 0; j < Mathf.Min(3, optionButtons.Length); j++)
                    {
                        RectTransform transformOption = optionButtons[j].GetComponent<RectTransform>();
                        transformOption.sizeDelta = newSize;
                        optionButtons[j].gameObject.Activate();
                    }
                }
                
                optionButtons[i].transform.parent.gameObject.Activate();
            }

            TranslateOptions(optionButtons, _localizer.GlobalLanguageCodeRuntime);

            _eventManager.InvokeDelayedBoolAction(1.5f, (boolParameter) => 
                _eventManager.ChatMessageReceived(boolParameter), false);
        }
        
        private void TranslateOptions(OptionButton[] options, string currentLanguageCode)
        {
            foreach (var option in options)
            {
                LocalizedUIText localizedComponent = option.GetComponent<LocalizedUIText>();
                TextMeshProUGUI textComponentOption = option.GetComponentInChildren<TextMeshProUGUI>();
                Translator.Languages neededTranslationData = localizedComponent.LocalizedData.Find(x => x.languageCode == currentLanguageCode);
                textComponentOption.text = neededTranslationData.key;   
            }
        }
        
        private void InstallTranslationOptionsByLastMsg(MessageData lastMessage, OptionButton[] options)
        {
            if (lastMessage.optionalData.Branches.Length <= 0)
            {
                Debug.LogWarning("This message is not last in current conversation");
            }
            
            for (int i = 0; i < lastMessage.optionalData.Branches.Length && i < options.Length; i++)
            {
                BranchData branch = lastMessage.optionalData.Branches[i];
                OptionButton option = options[i];
                
                LocalizedUIText localizedComponent = option.GetComponent<LocalizedUIText>();

                foreach (var localizedField in branch.LocalizedFields)
                {
                    foreach (var localizedData in localizedField.localizedDataList)
                    {
                        foreach (var localizedDataOption in localizedComponent.LocalizedData)
                        {
                            if (localizedDataOption.languageCode == localizedData.languageCode)
                            {
                                localizedDataOption.key = localizedData.key;
                            }
                        }
                    }
                }
            }
        }
        
        private void SetNameSender(IMessage newMsg)
        {
	        string leftActor = CurrentConversationData.actorLeftName;
	        string rightActor = CurrentConversationData.actorRightName;
            string storyTeller = config.storyTellerName;
	        newMsg.SetNameActors(leftActor, rightActor, storyTeller);
        }

        private MessageData[] CheckIsBranch(MessageData[] messages)
        {
	        if (messages == null) messages = CurrentConversationData.Messages;
            Debug.Log("messages found");
            return messages;
        }

        private IMessage InstallPrefabMsg(MessageData message, out MessagePictureView pictureMsgProxy)
        {
            //if (messages.Length <= 0)
            //{
                //pictureMsgProxy = null;
                //return null;
            //}

            IMessage chosenPrefab = message.optionalData.GallerySlot != null
                ? msgPicturePrefab
                : msgDefaultPrefab;

            Debug.Log("chosen prefab");

            IMessage newMsg = Instantiate((MonoBehaviour)chosenPrefab, contentMsgs).GetComponent<IMessage>();

            pictureMsgProxy = null;
            if (newMsg is MessagePictureView)
            {
                MessagePictureView pictureMsg = newMsg as MessagePictureView;
                pictureMsgProxy = pictureMsg;
                pictureMsgProxy.SetDurationSendingPicture(config.DurationSendingPicture);
                _currentPictureMsg = pictureMsg;
                PictureMessages.Add(pictureMsg);
                DampedPictureMessages.Add(pictureMsg);
                
                if (_allMessages.Contains(pictureMsg) == false)
                    _allMessages.Add(pictureMsg);
            }
            else if (newMsg is MessageDefaultView)
            {
                MessageDefaultView defaultMsg = newMsg as MessageDefaultView;
                defaultMsg.InitSoundInvoker(_soundHandler);
                defaultMsg.InitContentSpace(contentMsgs);
                defaultMsg.InstallLanguageCode(_localizer.GlobalLanguageCodeRuntime);
                defaultMsg.InstallStatusTranslatedAudioButtons();
                _currentDefaultMsg = defaultMsg;
                _defaultMessages.Add(defaultMsg);
                
                if (_allMessages.Contains(defaultMsg) == false)
                    _allMessages.Add(defaultMsg);
            }
            
            return newMsg;
        }

        public RectTransform CreatePadding()
        {
            return Instantiate(paddingPrefab, contentMsgs);
        }

        private void UnRegisterStatusViews()
        {
            foreach (var statusView in _chatStatusViews)
            {
                statusView.OnClick -= StartDialogue;
                statusView.OnClick -= StorySelected;
            }
        }

        private void RegisterOptions()
        {
            foreach (var optionButton in optionButtons)
            {
                optionButton.OnClick += DeactivateOptions;
                optionButton.InitializeChat(this);
            }
        }

        private void UnRegisterOptions()
        {
            foreach (var optionButton in optionButtons)
            {
                optionButton.OnClick -= DeactivateOptions;
            }
        }

        private void DeactivateOptions()
        {
            foreach (var option in optionButtons)
            {
                option.gameObject.Deactivate();
            }
        }

        public void DeactivateStatusViews()
        {
            if (_chatStatusViews.Count > 0) _chatStatusViews.ForEach(chat => chat.gameObject.Deactivate());
        }

        public void ResetStatusViews()
        {
            if (_chatStatusViews.Count > 0)
                _chatStatusViews.Clear();
        }

        private void DeleteContentObjects()
        {
             for (int i = contentMsgs.childCount - 1; i >= 0; i--)
             {
                 if (contentMsgs.GetChild(i) != contentMsgs.Find("Options"))
                     Destroy(contentMsgs.GetChild(i).gameObject);
             }
        }

        private void ResetMsgPictures()
        {
            foreach (var pictureMsg in PictureMessages)
            {
                if (DampedPictureMessages.Contains(pictureMsg) == false) DampedPictureMessages.Add(pictureMsg);
            }
            
            if (PictureMessages.Count > 0) PictureMessages.Clear();
            
            if (DampedPictureMessages.Count > 0) DampedPictureMessages.Clear();
        }

        private void ResetRenderedContent()
        {
            if (contentMsgs.childCount <= 0) return;
            
            StopAllCoroutines();

            if (_currentRenderedChat is null)
            {
                Debug.LogError("CurrentRenderedChat is null to reset rendered content!");
                return;
            }
            
            if (CompletedMessagesCurrentConversation.Count > 0) CompletedMessagesCurrentConversation.Clear();
            if (Messages.Count > 0) Messages.Clear();
            
            ResetMsgPictures();

            foreach (var messageViewBase in _currentRenderedChat.messagesView)
            {
                if (_currentRenderedChat == null) break;
                if (messageViewBase == null) break;
                
                if (messageViewBase is MessageDefaultView defaultMsg)
                {
                    if (defaultMsg.MainParent != null) defaultMsg.MainParent.gameObject.Deactivate();
                    else defaultMsg.gameObject.Deactivate();
                }
                else if (messageViewBase is MessagePictureView pictureMsg)
                {
                    pictureMsg.gameObject.Deactivate();
                }
                
                messageViewBase.PaddingForwardChat?.gameObject.Deactivate();
            } 
            if (Messages.Count > 0) Messages.Clear();
            if (_allMessages.Count > 0) _allMessages.Clear();
        }

        public void ResetContent()
        {
            if (contentMsgs.childCount <= 0) return;

            //IsMessagesEnded = true;
            StopAllCoroutines();

            //if (CompletedMessagesCurrentConversation.Count > 0) CompletedMessagesCurrentConversation.Clear();
            //if (Messages.Count > 0) Messages.Clear();

            foreach (var pictureMsg in PictureMessages)
            {
                if (DampedPictureMessages.Contains(pictureMsg) == false) DampedPictureMessages.Add(pictureMsg);
            }
            
            //if (PictureMessages.Count > 0) PictureMessages.Clear();
            
            if (DampedPictureMessages.Count > 0) DampedPictureMessages.Clear();

            // for (int i = contentMsgs.childCount - 1; i >= 0; i--)
            // {
            //     if (contentMsgs.GetChild(i) != contentMsgs.Find("Options"))
            //         Destroy(contentMsgs.GetChild(i).gameObject);
            // }
        }
    }

    public class ConversationView
    {
        private ChatStatusView _chatStatusView;
        private Sprite _completedChat, _unCompletedChat;

        public void Initialize(Sprite completedChat, Sprite unCompletedChat)
        {
            _completedChat = completedChat;
            _unCompletedChat = unCompletedChat;
        }

        public ChatStatusView InstallStatusView()
        {
            return _chatStatusView;
        }

        public void SetDataConversation(СonversationData chatData)
        {
            //_chatStatusView.Render(chatData);
            SetStatusChat();
        }

        private void SetStatusChat()
        {
            //_chatStatusView.SetStatus(_completedChat, _unCompletedChat);
        }
    }
}