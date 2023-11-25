using System;
using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using UnityEngine.UI;
using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.Chat
{
    [RequireComponent(typeof(Button))]
    public class OptionButton : MonoBehaviour
    {
        public BranchData BranchData { get; set; }
        
        private Button _button;
        private Chat _chat;

        public event Action OnClick;
        
        private RectTransform _parent;

        private void Awake()
        {
            _parent = transform.parent.GetComponent<RectTransform>();

            _button = GetComponent<Button>();
            _button.AddListener(LoadBranch);
            _button.AddListener(gameObject.Deactivate);
            OnClick += CreateCopyInChat;
        }

        private void OnDestroy()
        {
            _button.RemoveListener(LoadBranch);
            _button.RemoveListener(gameObject.Deactivate);
            OnClick -= CreateCopyInChat;
        }

        public void InitializeChat(Chat chat)
        {
            _chat = chat;
        }

        public void CreateCopyInChat()
        {
            GameObject newOptionParent = CreateOptionParent();
            SetupOption(newOptionParent);
            SetupPadding(newOptionParent);
    
            Debug.Log("lastMessage:", _chat.ContentMsgs.GetChild(_chat.ContentMsgs.childCount - 2));
            Debug.Log("new option:", newOptionParent.gameObject);
        }

        private GameObject CreateOptionParent()
        {
            GameObject newOptionParent = Instantiate(_parent.gameObject, _chat.ContentMsgs);
            newOptionParent.transform.SetAsLastSibling();
            return newOptionParent;
        }

        private void SetupOption(GameObject newOptionParent)
        {
            GameObject newOption = newOptionParent.transform.GetChild(0).gameObject;
            RectTransform newOptionRect = newOption.GetComponent<RectTransform>();
            Button newOptionButton = newOption.GetComponent<Button>();
            HorizontalLayoutGroup parentHorizontalLayout = newOptionParent.GetComponent<HorizontalLayoutGroup>();

            newOption.Activate();
            newOptionRect.sizeDelta = new Vector2(660, 100);
            newOptionButton.interactable = false;

            parentHorizontalLayout.childAlignment = TextAnchor.MiddleLeft;
            parentHorizontalLayout.padding.left = 200;

            KeepOnlyOneSibling(newOptionParent, newOption.GetComponent<OptionButton>());
        }

        private void SetupPadding(GameObject newOptionParent)
        {
            RectTransform backPadding = _chat.CreatePadding();
            RectTransform forwardPadding = _chat.CreatePadding();

            backPadding.SetSiblingIndex(newOptionParent.transform.GetSiblingIndex() - 1);
            backPadding.sizeDelta = new Vector2(backPadding.sizeDelta.x, backPadding.sizeDelta.y + 20);
            forwardPadding.SetSiblingIndex(newOptionParent.transform.GetSiblingIndex() + 1);
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