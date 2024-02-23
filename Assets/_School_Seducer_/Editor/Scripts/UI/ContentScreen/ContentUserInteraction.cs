using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using _School_Seducer_.Editor.Scripts.Extensions;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace _School_Seducer_.Editor.Scripts.UI
{
    public class ContentUserInteraction : MonoBehaviour, IContentUserInteraction
    {
        [SerializeField] private Button previousContentButton; 
        [SerializeField] private Button nextContentButton;
        [SerializeField] private Button animateButton;

        private IContentDisplay _displayModule;
        private IContentAnimation _contentAnimation;

        public void Initialize(IContentDisplay displayModule, [CanBeNull] IContentAnimation contentAnimation)
        {
            _displayModule = displayModule;
            _contentAnimation = contentAnimation;
            
            RegisterContentButtons();

            _contentAnimation.IsNullReturn();
            RegisterAnimateButton();
        }

        public void HandleUserInteraction()
        {
            
        }

        private void OnDestroy()
        {
            UnregisterContentButtons();
            
            _contentAnimation.IsNullReturn();
            UnregisterAnimateButton();
        }

        private void RegisterAnimateButton() => animateButton.AddListener(_contentAnimation.EnableAnimation);

        private void UnregisterAnimateButton() => animateButton.RemoveListener(_contentAnimation.EnableAnimation);

        private void RegisterContentButtons()
        {
            nextContentButton.AddListener(_displayModule.SwitchToNextContent);
            previousContentButton.AddListener(_displayModule.SwitchToPreviousContent);
        }

        private void UnregisterContentButtons()
        {
            nextContentButton.RemoveListener(_displayModule.SwitchToNextContent);
            previousContentButton.RemoveListener(_displayModule.SwitchToPreviousContent);
        }
    }
}