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

        public Image Content { get; private set; }
        private Button _button;
        private Sprite _spriteToSet;

        private IModeContent _modeContent;

        public void OnPointerDown(PointerEventData eventData)
        {
            ContentInstall(this);
            Debug.Log("Was attempt to get content..." +  ContentScreen.CurrentData.name);
        }

        private void OnValidate()
        {
            _button = GetComponent<Button>();
            _spriteToSet = GetComponent<Image>().sprite;
        }

        private void Start()
        {
            InstallComponents();
            InstallMode();
        }

        private void OnDestroy()
        {
            if (_button == null) return;
            
            _button.RemoveListener(_modeContent.OnClick);
        }

        private void InstallComponents()
        {
            _button = GetComponent<Button>();
            Content = GetComponent<Image>();
            _spriteToSet = GetComponent<Image>().sprite;
        }

        private void InstallMode()
        {
            switch (modeContent)
            {
                case ModeContentEnum.FullScreen:
                    _modeContent = new FullScreenMode(_spriteToSet, Content);
                    break;
            }
            
            _button.AddListener(_modeContent.OnClick);
        }

        private void ContentInstall(OpenContent content)
        {
            ContentScreen.CurrentData = content;
        }
    }
}