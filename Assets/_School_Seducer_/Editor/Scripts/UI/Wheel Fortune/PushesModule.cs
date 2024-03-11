using System.Collections;
using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using _School_Seducer_.Editor.Scripts.Chat;
using Spine.Unity;
using UnityEngine;
using AnimationState = Spine.AnimationState;

namespace _School_Seducer_.Editor.Scripts.UI.Wheel_Fortune
{
    public class PushesModule : MonoBehaviour, IModule<WheelFortuneSystem>
    {
        [SerializeField] public SkeletonGraphic giftPush;

        private WheelFortuneSystem _system;

        private SkeletonGraphic _currentAnimation;
        private AnimationState _currentState;
        
        public void InitializeCore(WheelFortuneSystem system)
        {
            _system = system;
        }

        public void ShowGiftPush()
        {
            InitializeAnimation(giftPush.SkeletonDataAsset, giftPush);

            StartCoroutine(ProcessGiftAnimation());
        }

        public void HideGiftPush()
        {
            _currentAnimation.AnimationState.SetAnimation(0, _currentAnimation.skeletonDataAsset.GetSkeletonData(false).Animations.Items[1].Name, false);

            _currentAnimation.gameObject.Deactivate(0.75f);
        }

        private void InitializeAnimation(SkeletonDataAsset animationData, SkeletonGraphic animation)
        {
            _currentAnimation = animation;
            _currentState = animation.AnimationState;

            _currentAnimation.gameObject.Activate();
            
            animation.skeletonDataAsset = animationData;

            Debug.Log("Installed SPINE animation name: " + animation.startingAnimation + " for animation: " + animation.name);

            //StartCoroutine(WaitForAnimationStateEnd(2));
        }

        private IEnumerator ProcessGiftAnimation()
        {
            _currentAnimation.startingAnimation = _currentAnimation.skeletonDataAsset.GetSkeletonData(false).Animations.Items[0].Name;
            _currentAnimation.startingLoop = false;
            _currentAnimation.Initialize(true);

            yield return WaitCurrentAnimationComplete();
            
            _currentAnimation.AnimationState.SetAnimation(0, _currentAnimation.skeletonDataAsset.GetSkeletonData(false).Animations.Items[2].Name, true);
            
            //_currentAnimation.startingAnimation = _currentAnimation.skeletonDataAsset.GetSkeletonData(false).Animations.Items[2].Name;
            //_currentAnimation.startingLoop = true;
            //_currentAnimation.Initialize(true);
        }

        private IEnumerator WaitCurrentAnimationComplete()
        {
            yield return new WaitForSecondsRealtime(GetCurrentAnimationDuration());
        }

        private float GetCurrentAnimationDuration()
        {
            return _currentState.GetCurrent(0).Animation.Duration;
        }
    }
}