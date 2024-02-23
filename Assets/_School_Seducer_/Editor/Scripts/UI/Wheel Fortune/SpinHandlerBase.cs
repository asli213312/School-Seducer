using System;
using _School_Seducer_.Editor.Scripts.Chat;
using UnityEngine;
using Zenject;

namespace _School_Seducer_.Editor.Scripts.UI.Wheel_Fortune
{
    public abstract class SpinHandlerBase : MonoBehaviour, IModule<SpinHandlerModule>
    {
        [Inject] protected Bank Bank;
        [Inject] protected EventManager EventManager;
        [Inject] protected IChatStoryResolverModule ChatStoryResolver;

        protected SpinHandlerModule SpinHandler;

        public void InitializeCore(SpinHandlerModule system)
        {
            SpinHandler = system;
        }

        protected abstract void Spin();

        protected abstract void Stop();

        private void FixedUpdate()
        {
            Spin();
        }
    }
}