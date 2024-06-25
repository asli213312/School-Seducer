using Spine.Unity;
using UnityEngine;
using Zenject;

namespace _School_Seducer_.Editor.Scripts.Utility
{
    public class SpineUtility : MonoInstaller
    {
        private SkeletonGraphic _currentAnimation;

        public override void InstallBindings()
        {
            Container.Bind<SpineUtility>().FromComponentInHierarchy().AsSingle();
        }

        public void InvokeStartAnimation(SkeletonGraphic newAnimation) 
        {
            InstallAnimation(newAnimation);
            StartupAnimation();
            SetAnimationState(0);
        }

        public void InstallAnimation(SkeletonGraphic newAnimation) => _currentAnimation = newAnimation;

        public void StartupAnimation()
        {
            if (_currentAnimation == null) return;
            
            _currentAnimation.startingAnimation = _currentAnimation.skeletonDataAsset.GetSkeletonData(false).Animations.Items[0].Name;
            _currentAnimation.Initialize(true);
        } 
        
        public void SetAnimationState(int index)
        {
            if (_currentAnimation == null) return;

            string newAnimation = _currentAnimation.skeletonDataAsset.GetSkeletonData(false).Animations.Items[index].Name;
            _currentAnimation.AnimationState.SetAnimation(0, newAnimation, false);
        }
    }
}