using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using _School_Seducer_.Editor.Scripts.Extensions;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _School_Seducer_.Editor.Scripts.Chat
{
    public class Chat : MonoBehaviour
    {
        [Inject] private EventManager _eventManager;
        
        [SerializeField] private Transform contentMsgs;
        [SerializeField] private Previewer previewer;
        [SerializeField] private OptionButton[] optionButtons;

        [Header("Data")] 
        [SerializeField] private ChatConfig config;
        [SerializeField] private MessageDefaultView msgDefaultPrefab;
        [SerializeField] private MessagePictureView msgPicturePrefab;
        [SerializeField] private RectTransform paddingPrefab;

        public bool IsBigMessage { get; private set; }
        public bool IsVeryBigMessage { get; private set; }
        public Transform ContentMsgs => contentMsgs;
	    public СonversationData CurrentConversation { get; private set; }
        public bool IsMessagesEnded { get; private set; }
        public ChatConfig Config => config;

        public OptionButton[] OptionButtons => optionButtons;
        private bool _isStartConversation;

        private void Awake()
        {
            config.OnConversationEnd.AddListener(previewer.AddDiamondOnConversationEnd);
            config.OnMessageReceived.AddListener(previewer.ReduceMoneyPlayer);

            RegisterOptions();
            previewer.Initialize(this);
        }

        private void OnDestroy()
        {
            config.OnConversationEnd.RemoveListener(previewer.AddDiamondOnConversationEnd);
            config.OnMessageReceived.RemoveListener(previewer.ReduceMoneyPlayer);

            UnRegisterOptions();
        }

        public void LoadBranch(BranchData branchData)
        {
            //ResetContent();
            StartCoroutine(LoadMessages(branchData.Messages));
        }

        public void InvokeStartConversation(MessageData[] messages)
        {
            StartCoroutine(LoadMessages(messages));
        }
        
	    public void InstallCurrentConversation(СonversationData data) 
	    {
	    	CurrentConversation = data;
	    }

        private IEnumerator LoadMessages(MessageData[] messages = null)
	    {
            messages = CheckIsBranch(messages);
            bool lastMessage = false;

            MessagesStarted();
            StartConversation();

            for (int i = 0; i < messages.Length; i++)
            {
                CheckOptionsIsLastSibling();

                yield return new WaitUntil(InputExtensions.CheckTap);
                yield return new WaitForSeconds(0.5f);

                IsVeryBigMessage = false;
                IsBigMessage = false;
                
                var paddingBack = CreatePadding();
                var newMsg = InstallPrefabMsg(messages, i, out var pictureMsgProxy);
                var paddingForward = CreatePadding();

                newMsg.Initialize(optionButtons);

                RenderMsgData(messages, newMsg, i);
                SetNameSender(newMsg);
                
                if (newMsg is MessageDefaultView)
                {
                    MessageDefaultView defaultMsg = newMsg as MessageDefaultView;
                    bool needDestroy = false;
                    if (defaultMsg.CheckIsVeryBigMessage())
                    {
                        IsVeryBigMessage = true;
                        Debug.Log("IsVeryBigMessage in CHAT: " + IsVeryBigMessage);
                        paddingBack.gameObject.Activate();
                        paddingForward.gameObject.Activate();
                    }
                    else if (defaultMsg.CheckIsVeryBigMessage() == false)
                    {
                        IsBigMessage = true;
                        Debug.Log("IsBigMessage in CHAT: " + IsBigMessage);
                        paddingBack.gameObject.Activate();
                        paddingForward.gameObject.Activate();
                    }
                    
                    if (defaultMsg.CheckIsVeryBigMessage() == false && defaultMsg.IsBigMessageFalse())
                    {
                        needDestroy = true;
                    }

                    if (needDestroy)
                    {
                        paddingBack.gameObject.Destroy();
                        paddingForward.gameObject.Destroy();
                    }
                }

                Debug.Log("Render completed");

	            config.OnMessageReceived?.Invoke();

                CheckOptionsIsLastSibling();

                yield return new WaitForSeconds(config.DelayBtwMessage);

                if (MsgHasBranches(messages, i))
                {
                    Debug.Log("options installed in MsgHasBranches");

                    if (pictureMsgProxy != null)
                    {
                        yield return new WaitUntil(() => pictureMsgProxy.PictureInstalled);
                        InstallOptions(messages[i]);
                    }

                    InstallOptions(messages[i]);
                    break;
                }
                
                if (messages[i] == messages[^1])
                {
                    MessagesEnded();
                }

                lastMessage = CheckLastMessage(messages, i, lastMessage);

                while (pictureMsgProxy != null && !pictureMsgProxy.PictureInstalled)
                {
                    yield return new WaitForSeconds(1f);   
                }
            }

            EndConversation(lastMessage);
            MessagesEnded();
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

        private void CheckOptionsIsLastSibling()
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
	            CurrentConversation.ActorLeftSprite,
	            CurrentConversation.ActorRightSprite,
                config.StoryTellerSprite,
                config.NeedIconStoryTeller);
        }

        private void InstallOptions(MessageData msgData)
        {
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
            }
        }
        
        private void SetNameSender(IMessage newMsg)
        {
	        string leftActor = CurrentConversation.ActorLeftName;
	        string rightActor = CurrentConversation.ActorRightName;
            string storyTeller = config.StoryTellerName;
	        newMsg.SetNameActors(leftActor, rightActor, storyTeller);
        }

        private MessageData[] CheckIsBranch(MessageData[] messages)
        {
	        if (messages == null) messages = CurrentConversation.Messages;
            Debug.Log("messages found");
            return messages;
        }

        private IMessage InstallPrefabMsg(MessageData[] messages, int i, out MessagePictureView pictureMsgProxy)
        {
            IMessage chosenPrefab = messages[i].optionalData.PictureInsteadMsg != null
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
            }

            return newMsg;
        }

        public RectTransform CreatePadding()
        {
            return Instantiate(paddingPrefab, contentMsgs);
        }
        
	    private bool IsOptionsActivated() 
	    {
	    	foreach (var option in optionButtons) 
	    	{
	    		return option.gameObject.activeSelf;
	    	}
	    	
	    	return false;
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

        private void ResetContent()
        {
            for (int i = contentMsgs.childCount - 1; i >= 0; i--)
            {
                if (contentMsgs.GetChild(i) != contentMsgs.Find("Options"))
                    Destroy(contentMsgs.GetChild(i).gameObject);
            }
        }
    }
}