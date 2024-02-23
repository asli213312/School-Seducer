using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _School_Seducer_.Editor.Scripts.UI
{
    public class OpenContentSprite : OpenContentBase
    {
        [SerializeField] private Image contentImage;

        public Image Content => contentImage;
        private Sprite _spriteToSet;

        protected override IModeContent ModeContent => new FullScreenMode(_spriteToSet, Content);

        protected override void InstallComponents()
        {
            _spriteToSet = contentImage.sprite;
        }
    }
}