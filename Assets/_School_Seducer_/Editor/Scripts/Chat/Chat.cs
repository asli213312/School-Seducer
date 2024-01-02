using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _BonGirl_.Editor.Scripts;
using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using _School_Seducer_.Editor.Scripts.UI;
using _School_Seducer_.Editor.Scripts.Utility;
using _School_Seducer_.Editor.Scripts.Utility.Translation;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;

namespace _School_Seducer_.Editor.Scripts.Chat
{
    public class Chat : MonoBehaviour, IObservableCustom<MonoBehaviour>
    {
        [Inject] private LocalizedGlobalMonoBehaviour _localizer;
        [Inject] private SoundHandler _soundHandler;
        [Inject] private EventManager _eventManager;
        
        [SerializeField] private Transform contentMsgs;
        [SerializeField] private Transform contentChats;
        [SerializeField] private GalleryScreen gallery;
        [SerializeField] private Previewer previewer;
        [SerializeField] private OptionButton[] optionButtons;

        [Header("Data")] 
        [SerializeField] private ChatConfig config;
        [SerializeField] private ChatStatusView chatStatusView;
        [SerializeField] private MessageDefaultView msgDefaultPrefab;
        [SerializeField] private MessagePictureView msgPicturePrefab;
        [SerializeField] private RectTransform paddingPrefab;
        [ShowInInspector] public List<IContent> PictureMessages { get; private set; } = new();
        
        public Transform ContentMsgs => contentMsgs;
        public BranchData CurrentBranchData { get; private set; }
        public СonversationData CurrentConversationData { get; private set; }
        public bool IsMessagesEnded { get; private set; }
        public ChatConfig Config => config;
        public List<MessageData> CompletedMessages { get; private set; } = new();

        private List<MessageData> _messages = new();
        private List<ChatStatusView> _chatStatusViews = new();
        private List<MessageDefaultView> _defaultMessages = new();
        private ChatStatusView _currentStatusView;
        private ConversationView _conversationView;
        private bool _isStartConversation;
        private int _iteratedMessages;
        private MessageDefaultView _currentDefaultMsg;
        private MessagePictureView _currentPictureMsg;

        [ContextMenu("Translate Messages")]
        private void TranslateRenderedMessages()
        {
            if (_defaultMessages.Count < 0) return;

            foreach (var msg in _defaultMessages)
            {
                msg.TranslateText(_localizer.GlobalLanguageCodeRuntime);
            }
        }

        private void Awake()
        {
            config.OnConversationEnd.AddListener(previewer.AddDiamondOnConversationEnd);
            config.OnMessageReceived.AddListener(previewer.ReduceMoneyPlayer);

            RegisterOptions();
            previewer.Initialize(this);

            _conversationView = new ConversationView();
            _localizer.AddObserver(this);
            //_conversationView.Initialize(config.ChatCompletedSprite, config.ChatUncompletedSprite);
        }

        private void OnDestroy()
        {
            config.OnConversationEnd.RemoveListener(previewer.AddDiamondOnConversationEnd);
            config.OnMessageReceived.RemoveListener(previewer.ReduceMoneyPlayer);

            UnRegisterOptions();
            
            _localizer.RemoveObserver(this);
        }

        private void OnEnable()
        {
            InitializeChats();
        }
        
        public void OnObservableUpdate()
        {
            TranslateRenderedMessages();

            if (_messages.Count < 0) return;

            foreach (var msg in _messages)
            {
                msg.Msg = msg.TranslateMsg(_localizer.GlobalLanguageCodeRuntime);
            }
            
            var paddingBack = CreatePadding();
            paddingBack.gameObject.Deactivate(0.1f);
        }

        public void LoadBranch(BranchData branchData)
        {
            CurrentBranchData = branchData;
            StartCoroutine(LoadMessages(branchData.Messages));
        }

        public void InvokeStartConversation(MessageData[] messages)
        {
            StartCoroutine(LoadMessages(messages));
        }
        
	    public void InstallCurrentConversation(СonversationData data) 
	    {
	    	CurrentConversationData = data;
            gallery.SetCurrentData(previewer.GetCurrentCharacterData().gallery);
            _currentStatusView = _chatStatusViews[CurrentConversationData.conversationIndex];
        }

        public void StartDialogue(UnityAction onStarted)
        {
            ResetContent();
            StartCoroutine(ChatTap(onStarted));
        }

        private void InitializeChats()
        {
            for (int i = 0; i < config.Chats.Length; i++)
            {
                ChatStatusView chatStatus = Instantiate(chatStatusView, contentChats);
                chatStatus.Render(config.Chats[i]);
                chatStatus.SetStatus(config.ChatCompletedSprite, config.ChatUncompletedSprite);
                _chatStatusViews.Add(chatStatus);
            }
        }

        private IEnumerator LoadMessages(MessageData[] messages = null)
	    {
            messages = CheckIsBranch(messages);

            MessagesStarted();
            StartConversation();
            
            _messages.AddRange(messages);
            
            OnObservableUpdate();

            for (int i = 0; i < _messages.Count; i++)
            {
                if (IsMessagesEnded) StopCoroutine(ProcessMessage(i));
                    
                yield return ProcessMessage(i);
            }

            EndConversation(CheckLastMessage(_messages.ToArray(), _messages.Count - 1, false));
            MessagesEnded();
        }
        
        private IEnumerator ProcessMessage(int index)
        {
            _eventManager.ChatMessageReceived(false);
            
            if (CompletedMessages.Count <= 0) StopCoroutine(ProcessMessage(index));
            
            if (CompletedMessages.Contains(_messages[index]))
            {
                yield break;
            }

            CheckOptionsParentIsLastSibling();

            yield return new WaitUntil(CheckTapBounds);
            yield return new WaitForSeconds(0.5f);

            var paddingBack = CreatePadding();
            var newMsg = InstallPrefabMsg(_messages.ToArray(), index, out var pictureMsgProxy);
            var paddingForward = CreatePadding();
            
            if (newMsg == null) yield break;

            newMsg.Initialize(optionButtons);
            RenderMsgData(_messages.ToArray(), newMsg, index);

            ActivatePaddings(paddingBack, paddingForward);

            Debug.Log("Render completed");

            config.OnMessageReceived?.Invoke();
            _eventManager.ChatMessageReceived(true);

            CheckOptionsParentIsLastSibling();

            yield return new WaitForSeconds(config.DelayBtwMessage);

            if (MsgHasBranches(_messages.ToArray(), index))
            {
                Debug.Log("options installed in MsgHasBranches");

                if (pictureMsgProxy != null)
                {
                    yield return new WaitUntil(() => pictureMsgProxy.PictureInstalled);
                    InstallOptions(_messages[index]);
                }

                InstallOptions(_messages[index]);

                _iteratedMessages++;

                CompletedMessages.Add(_messages[index]);
                _currentStatusView.CompletionChanged(index, _messages.ToArray());

                Debug.Log("iterated messages: " + _iteratedMessages);
                yield break;
            }

            if (_messages[index] == _messages[^1])
            {
                MessagesEnded();
            }

            var lastMessage = CheckLastMessage(_messages.ToArray(), index, false);

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
            _currentStatusView.CompletionChanged(index, _messages.ToArray());
            CompletedMessages.Add(_messages[index]);

            Debug.Log("iterated messages: " + _iteratedMessages);
        }

        private IEnumerator ChatTap(UnityAction onTapped)
        {
            yield return new WaitUntil(CheckTapBounds);
            onTapped?.Invoke();
        }

        private bool CheckTapBounds()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                
                BoxCollider2D collider = gameObject.GetComponent<BoxCollider2D>();
                
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

        private void ActivatePaddings(RectTransform paddingBack, RectTransform paddingForward)
        {
            paddingBack.gameObject.Activate();
            paddingForward.gameObject.Activate();
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

        private bool CheckLastMessage(MessageData[] messages, int i, bool lastMessage)
        {
            if (i == messages.Length - 1)
            {
                lastMessage = true;
            }
            
            return lastMessage;
        }

        private bool MsgHasBranches(MessageData[] messages, int i)
        {
            if (messages[i].optionalData.Branches.Length > 0)
            {
                for (int j = 0; j < messages[i].optionalData.Branches.Length; j++)
                {
                    if (messages[i].optionalData.Branches[j] != null) break;
                }
                return true;
            }

            return false;
        }

        private void RenderMsgData(MessageData[] messages, IMessage newMsg, int i)
        {
            newMsg.RenderGeneralData(messages[i],
	            CurrentConversationData.ActorLeftSprite,
	            CurrentConversationData.ActorRightSprite,
                config.StoryTellerSprite,
                config.NeedIconStoryTeller);
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
                LocalizedUIObject localizedComponent = option.GetComponent<LocalizedUIObject>();
                Text textComponentOption = option.GetComponentInChildren<Text>();
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
                
                LocalizedUIObject localizedComponent = option.GetComponent<LocalizedUIObject>();

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

        private IMessage InstallPrefabMsg(MessageData[] messages, int i, out MessagePictureView pictureMsgProxy)
        {
            if (messages.Length <= 0)
            {
                pictureMsgProxy = null;
                return null;
            }

            IMessage chosenPrefab = messages[i].optionalData.GallerySlot != null
                ? msgPicturePrefab
                : msgDefaultPrefab;

            Debug.Log("chosen prefab");

            IMessage newMsg = Instantiate((MonoBehaviour) chosenPrefab, contentMsgs).GetComponent<IMessage>();

            pictureMsgProxy = null;
            if (newMsg is MessagePictureView)
            {
                MessagePictureView pictureMsg = newMsg as MessagePictureView;
                pictureMsgProxy = pictureMsg;
                pictureMsgProxy.SetDurationSendingPicture(config.DurationSendingPicture);
                _currentPictureMsg = pictureMsg;
                PictureMessages.Add(pictureMsg);
            }
            else if (newMsg is MessageDefaultView)
            {
                MessageDefaultView defaultMsg = newMsg as MessageDefaultView;
                defaultMsg.InitSoundInvoker(_soundHandler);
                defaultMsg.InitContentSpace(contentMsgs);
                _currentDefaultMsg = defaultMsg;
                _defaultMessages.Add(defaultMsg);
            }

            return newMsg;
        }

        public RectTransform CreatePadding()
        {
            return Instantiate(paddingPrefab, contentMsgs);
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

        public void ResetContent()
        {
            if (contentMsgs.childCount <= 0) return;

            if (_messages.Count > 0) _messages.Clear();
            if (CompletedMessages.Count > 0) CompletedMessages.Clear();
            
            for (int i = contentMsgs.childCount - 1; i >= 0; i--)
            {
                if (contentMsgs.GetChild(i) != contentMsgs.Find("Options"))
                    Destroy(contentMsgs.GetChild(i).gameObject);
            }
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
            _chatStatusView.Render(chatData);
            SetStatusChat();
        }

        private void SetStatusChat()
        {
            _chatStatusView.SetStatus(_completedChat, _unCompletedChat);
        }
    }
}