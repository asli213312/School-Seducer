using System;
using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _School_Seducer_.Editor.Scripts.Chat
{
    [RequireComponent(typeof(Button))]
    public class OptionButton : MonoBehaviour
    {
        public BranchData BranchData { get; set; }
        
        private Button _button;
        private Chat _chat;

        public event Action OnClick;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.AddListener(LoadBranch);
            _button.AddListener(gameObject.Deactivate);
        }

        private void OnDestroy()
        {
            _button.RemoveListener(LoadBranch);
            _button.RemoveListener(gameObject.Deactivate);
        }

        public void InitializeChat(Chat chat)
        {
            _chat = chat;
        }
        
        public void InteractableOn()
        {
            _button.interactable = true;            
        }

        public void InteractableOff()
        {
            _button.interactable = false;            
        }

        private void LoadBranch()
        {
            OnClick?.Invoke();
            _chat.LoadBranch(BranchData);

            Debug.Log("option", gameObject);
        }
    }
}