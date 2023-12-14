using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using _School_Seducer_.Editor.Scripts.Utility;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _School_Seducer_.Editor.Scripts.UI
{
    public class OpenContent : MonoBehaviour
    {
        [SerializeField] private ModeContentEnum modeContent;
        private Button _button;
        private Image _backGround;
        private Sprite _spriteToSet;

        private IModeContent _modeContent;

        public void Initialize(Image backGround)
        {
            _backGround = backGround;
        }

        private void Start()
        {
            InstallComponents();
            InstallMode();
        }

        private void InstallComponents()
        {
            _button = GetComponent<Button>();
            _spriteToSet = GetComponent<Image>().sprite;
        }

        private void InstallMode()
        {
            switch (modeContent)
            {
                case ModeContentEnum.FullScreen:
                    _modeContent = new FullScreenMode(_spriteToSet, _backGround);
                    break;
            }
            
            _button.AddListener(_modeContent.OnClick);
        }
    }
}