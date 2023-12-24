using System;
using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityFigmaBridge.Runtime.UI;
using Zenject;

namespace _School_Seducer_.Editor.Scripts.Chat
{
    [RequireComponent(typeof(Button))]
    public class OptionButton : MonoBehaviour
    {
        [SerializeField] private ChatConfig _chatConfig;
        [Inject] private SoundHandler _soundHandler;
        [Inject] private EventManager _eventManager;
        
        public BranchData BranchData { get; set; }
        
        private Button _button;
        private Chat _chat;

        public event Action OnClick;
        
        private RectTransform _parent;
        private GameObject _backSibling;

        public void InitializeChat(Chat chat)
        {
            _chat = chat;
        }

        private void Awake()
        {
            _parent = transform.parent.GetComponent<RectTransform>();

            _button = GetComponent<Button>();
            _button.AddListener(LoadBranch);
            _button.AddListener(gameObject.Deactivate);
            OnClick += CreateCopyInChat;
        }

        private void Start()
        {
            GetIndexSiblingAlongParent();
        }

        private void OnDestroy()
        {
            _button.RemoveListener(LoadBranch);
            _button.RemoveListener(gameObject.Deactivate);
            OnClick -= CreateCopyInChat;
        }

        private void CreateCopyInChat()
        {
            GameObject newOptionParent = CreateOptionParent();

            _backSibling = _chat.ContentMsgs.GetChild(1).gameObject;

            GameObject option = SetupOption(newOptionParent);
            SetupPadding(newOptionParent);
            
            SetupBackGroundImage(option);
            SetupAudioMessage();
            
            _eventManager.UpdateScrollChat();

            Debug.Log("lastMessage:", _chat.ContentMsgs.GetChild(_chat.ContentMsgs.childCount - 2));
            Debug.Log("new option:", newOptionParent.gameObject);
        }

        private void SetupAudioMessage()
        {
            _soundHandler.InvokeClipAfterPlayback(InstallAudioMessage);
        }

        private void InstallAudioMessage()
        {
            _soundHandler.InvokeOneClip(BranchData.audioMsg, null, _chatConfig.delayAudioMsg);
        }

        private void SetupBackGroundImage(GameObject option)
        {
            FigmaImage optionImage = option.GetComponent<FigmaImage>();
            optionImage.StrokeColor = _backSibling.transform.GetComponent<FigmaImage>().StrokeColor;
            optionImage.FillColor = _backSibling.transform.GetComponent<FigmaImage>().FillColor;
            optionImage.FillGradient = _backSibling.GetComponent<FigmaImage>().FillGradient;
            optionImage.GradientHandlePositions =
                _backSibling.transform.GetComponent<FigmaImage>().GradientHandlePositions;
            optionImage.Fill = FigmaImage.FillStyle.LinearGradient;
        }

        private GameObject CreateOptionParent()
        {
            GameObject newOptionParent = Instantiate(_parent.gameObject, _chat.ContentMsgs);
            newOptionParent.transform.SetAsLastSibling();
            _parent.gameObject.Deactivate();
            return newOptionParent;
        }

        private GameObject SetupOption(GameObject newOptionParent)
        {
            GameObject newOption = newOptionParent.transform.GetChild(GetIndexSiblingAlongParent()).gameObject;
            RectTransform newOptionRect = newOption.GetComponent<RectTransform>();
            Button newOptionButton = newOption.GetComponent<Button>();
            HorizontalLayoutGroup parentHorizontalLayout = newOptionParent.GetComponent<HorizontalLayoutGroup>();

            newOption.Activate();
            newOptionRect.sizeDelta = new Vector2(660, 100);
            newOptionButton.interactable = false;

            parentHorizontalLayout.childAlignment = TextAnchor.MiddleLeft;
            parentHorizontalLayout.padding.left = 73;
            
            //optionImage.StrokeColor = _backSibling.transform.Find("Background").GetComponent<FigmaImage>().StrokeColor;

            KeepOnlyOneSibling(newOptionParent, newOption.GetComponent<OptionButton>());

            return newOption;
        }

        private void SetupPadding(GameObject newOptionParent)
        {
            RectTransform backPadding = _chat.CreatePadding();
            RectTransform forwardPadding = _chat.CreatePadding();

            backPadding.SetSiblingIndex(newOptionParent.transform.GetSiblingIndex() - 1);
            backPadding.sizeDelta = new Vector2(backPadding.sizeDelta.x, backPadding.sizeDelta.y + 20);
            forwardPadding.SetSiblingIndex(newOptionParent.transform.GetSiblingIndex() + 1);
        }

        private int GetIndexSiblingAlongParent()
        {
            for (int i = 0; i < _parent.childCount; i++)
            {
                if (_parent.GetChild(i) == gameObject.transform)
                    return i;
            }

            Debug.LogError("Couldn't find sibling along parent for option: " + name);
            return 0;
        }

        private void KeepOnlyOneSibling(GameObject parent, OptionButton desiredSibling)
        {
            for (int i = 0; i < parent.transform.childCount; i++)
            {
                OptionButton optionSibling = parent.transform.GetChild(i).GetComponent<OptionButton>();
                if (optionSibling != desiredSibling)
                {
                    Destroy(parent.transform.GetChild(i).gameObject);
                }
            }
        }

        private void LoadBranch()
        {
            OnClick?.Invoke();
            _chat.LoadBranch(BranchData);

            Debug.Log("option", gameObject);
        }
    }
}