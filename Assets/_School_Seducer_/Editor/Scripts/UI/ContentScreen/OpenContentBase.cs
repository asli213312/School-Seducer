using System;
using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using _School_Seducer_.Editor.Scripts.Utility;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _School_Seducer_.Editor.Scripts.UI
{
    public abstract class OpenContentBase : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] protected Button button;

        public event Action<OpenContentBase> ContentInstalledEvent;
        public event Action ContentDeletedEvent;

        protected abstract IModeContent ModeContent { get; }
        protected virtual bool NeedContentScreen => true;
        protected Condition Condition;

        private void Start()
        {
            ContentDeletedEvent += OnDestroyContent;
            
            InstallComponents();
            InstallMode();
        }

        private void OnDestroy()
        {
            ContentDeletedEvent?.Invoke();
            ContentDeletedEvent -= OnDestroyContent;
            
            if (button == null) return;
            
            button.RemoveListener(ModeContent.OnClick);
        }

        public void SetCondition(Condition condition) => Condition = condition;
        
        public void OnPointerDown(PointerEventData eventData)
        {
            if (NeedContentScreen == false) return;
            
            if (Condition == null)
                ContentInstall(this);
            else if (Condition.IsTrue())
                ContentInstall(this);
            
            //Debug.Log("Was attempt to get content..." +  ContentScreen.CurrentData.name);
        }

        protected abstract void InstallComponents();
        protected virtual void OnDestroyContent() { }

        private void ContentInstall(OpenContentBase content)
        {
            ContentScreen.CurrentData = content;
            ContentScreenProxy.CurrentContent = content;
            
            ContentInstalledEvent?.Invoke(content);
        }

        private void InstallMode()
        {
            button.AddListener(ModeContent.OnClick);
        }
    }
}