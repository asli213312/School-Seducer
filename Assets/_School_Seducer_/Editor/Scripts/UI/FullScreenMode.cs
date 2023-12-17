using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using _School_Seducer_.Editor.Scripts.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace _School_Seducer_.Editor.Scripts.UI
{
    public class FullScreenMode : BaseModeContent, IModeContent
    {
        protected override bool Opened => _backGround.gameObject.activeSelf;
        protected override ModeContentEnum ModeContent => ModeContentEnum.FullScreen;

        private readonly Sprite _spriteToSet;
        private readonly Image _backGround;

        public FullScreenMode(Sprite spriteToSet, Image background)
        {
            _spriteToSet = spriteToSet;
            _backGround = background;
            
            Debug.Log("background: " + _backGround.name);
        }

        public void OnClick()
        {
            if (Opened == false)
            {
                _backGround.sprite = _spriteToSet;
            }

            Debug.Log("Content installed");
        }
    }
}