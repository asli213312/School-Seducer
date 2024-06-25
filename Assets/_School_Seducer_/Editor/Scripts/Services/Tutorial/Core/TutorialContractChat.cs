using System;
using System.Collections;
using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using _School_Seducer_.Editor.Scripts.Chat;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Events;

namespace _School_Seducer_.Editor.Scripts.Services.Tutorial.Core
{
    public class TutorialContractChat : TutorialContractBase
    {
        [SerializeField] private Chat.Chat chat;
        [SerializeField] private SkeletonGraphic animation;
        [SerializeField] private UnityEvent completedEvent;

        private void Awake()
        {
            chat.TapStoryEvent += CloseAnimation;
        }
        
        protected override IEnumerator Process()
        {
            Utility.Spine.InvokeStartAnimation(animation);
            yield return new WaitUntil(() => chat.IsMessagesEnded);
            completedEvent?.Invoke();
        }
        
        private void CloseAnimation(bool chatTapped)
        {
            if (chatTapped)
            {
                animation.gameObject.Deactivate(.3f);
                chat.TapStoryEvent -= CloseAnimation;
            }
        }
    }
}