using Spine.Unity;
using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.UI
{
    public class OpenContentAnimation : OpenContentBase
    {
        [SerializeField] private SkeletonAnimation skeletonAnimation;

        public SkeletonAnimation Animation => skeletonAnimation;
        public string AnimationName => skeletonAnimation.SkeletonDataAsset.GetSkeletonData(false).Animations.Items[0].Name;

        protected override IModeContent ModeContent => new AnimationMode(skeletonAnimation, _animationData);

        private SkeletonDataAsset _animationData;

        protected override void InstallComponents()
        {
            _animationData = skeletonAnimation.SkeletonDataAsset;
        }
    }
}