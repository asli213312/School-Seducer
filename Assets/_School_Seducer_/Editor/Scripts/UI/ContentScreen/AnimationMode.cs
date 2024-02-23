using _School_Seducer_.Editor.Scripts.Utility;
using Spine.Unity;
using UnityEngine.UI;

namespace _School_Seducer_.Editor.Scripts.UI
{
    public class AnimationMode : BaseModeContent, IModeContent
    {
        protected override bool Opened => _skeletonAnimation.enabled;
        protected override ModeContentEnum ModeContent => ModeContentEnum.Animation;

        private readonly SkeletonAnimation _skeletonAnimation;
        private readonly SkeletonDataAsset _data;

        public AnimationMode(SkeletonAnimation skeletonAnimation, SkeletonDataAsset data)
        {
            _skeletonAnimation = skeletonAnimation;
            _data = data;
        }

        public void OnClick()
        {
            if (Opened == false)
            {
                
            }
        }
    }
}