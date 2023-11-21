using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using _School_Seducer_.Editor.Scripts.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace _School_Seducer_.Editor.Scripts.Chat
{
    public class Chat : MonoBehaviour
    {
        [SerializeField] private Transform contentMsgs;
        [SerializeField] private Previewer previewer;
        [SerializeField] private OptionButton[] optionButtons;

        [Header("Data")] 
        [SerializeField] private ChatConfig config;
        [SerializeField] private MessageDefaultView msgDefaultPrefab;
        [SerializeField] private MessagePictureView msgPicturePrefab;
	    private СonversationData _currentConversation;

        public OptionButton[] OptionButtons => optionButtons;
        private bool _isStartConversation;

        private void Awake()
        {
            config.OnConversationEnd.AddListener(previewer.AddDiamondOnConversationEnd);
            
            RegisterOptions();
            previewer.Initialize(this);
        }

        private void OnDestroy()
        {
            config.OnConversationEnd.RemoveListener(previewer.AddDiamondOnConversationEnd);
            
            UnRegisterOptions();
        }

        private void Start()
        {
            //StartCoroutine(LoadMessages());
        }

        public void LoadBranch(BranchData branchData)
        {
            ResetContent();
            StartCoroutine(LoadMessages(branchData.Messages));
        }

        public void InvokeStartConversation(MessageData[] messages)
        {
            StartCoroutine(LoadMessages(messages));
        }
        
	    public void InstallCurrentConversation(СonversationData data) 
	    {
	    	_currentConversation = data;
	    }

        private IEnumerator LoadMessages(MessageData[] messages = null)
	    {
            messages = CheckIsBranch(messages);
            bool lastMessage = false;

            StartConversation();

            for (int i = 0; i < messages.Length; i++)
            {
                CheckOptionsIsLastSibling();
                
                var newMsg = InstallPrefabMsg(messages, i, out var pictureMsgProxy);

                newMsg.Initialize(optionButtons);

                RenderMsgData(messages, newMsg, i);
                SetNameSender(newMsg);

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

                lastMessage = CheckLastMessage(messages, i, lastMessage);

                while (pictureMsgProxy != null && !pictureMsgProxy.PictureInstalled)
                {
                    yield return new WaitForSeconds(1f);   
                }
            }

            IsEndConversation(lastMessage);
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
                lastMessage = true;
            return lastMessage;
        }

        private void IsEndConversation(bool lastMessage)
        {
            if (lastMessage)
                config.OnConversationEnd?.Invoke();
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
	            _currentConversation.ActorLeftSprite,
	            _currentConversation.ActorRightSprite,
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
	        string leftActor = _currentConversation.ActorLeftName;
	        string rightActor = _currentConversation.ActorRightName;
            string storyTeller = config.StoryTellerName;
	        newMsg.SetNameActors(leftActor, rightActor, storyTeller);
        }

        private MessageData[] CheckIsBranch(MessageData[] messages)
        {
	        if (messages == null) messages = _currentConversation.Messages;
            Debug.Log("messages found");
            return messages;
        }

        private IMessage InstallPrefabMsg(MessageData[] messages, int i, out MessagePictureView pictureMsgProxy)
        {
            IMessage chosenPrefab = messages[i].optionalData.PictureInsteadMsg != null
                ? msgPicturePrefab
                : msgDefaultPrefab;

            Debug.Log("choosed prefab");

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