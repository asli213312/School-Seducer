using System;
using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using _School_Seducer_.Editor.Scripts.Utility;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

namespace _School_Seducer_.Editor.Scripts.UI
{
    public class OpenContent : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private ModeContentEnum modeContent;
        [SerializeField] private Image contentImage;
        [SerializeField] private Button button;
        
        public Image Content => contentImage;
        private Sprite _spriteToSet;

        private IModeContent _modeContent;
        private Condition _condition;
        
        public void SetCondition(Condition condition) => _condition = condition;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_condition == null)
                ContentInstall(this);
            else if (_condition.IsTrue())
                ContentInstall(this);
            
            //Debug.Log("Was attempt to get content..." +  ContentScreen.CurrentData.name);
        }

        private void Start()
        {
            InstallComponents();
            InstallMode();
        }

        private void OnDestroy()
        {
            if (button == null) return;
            
            button.RemoveListener(_modeContent.OnClick);
        }

        private void InstallComponents()
        {
            //_button = GetComponent<Button>();
            _spriteToSet = contentImage.sprite;
        }

        private void InstallMode()
        {
            switch (modeContent)
            {
                case ModeContentEnum.FullScreen:
                    _modeContent = new FullScreenMode(_spriteToSet, Content);
                    break;
            }
            
            button.AddListener(_modeContent.OnClick);
        }

        private void ContentInstall(OpenContent content)
        {
            ContentScreen.CurrentData = content;
        }
    }
}