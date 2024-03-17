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
        protected PushesModule Pushes;

        public void InitializeCore(SpinHandlerModule system)
        {
            SpinHandler = system;
        }

        public void Initialize(PushesModule pushesModule)
        {
            Pushes = pushesModule;
        }

        protected virtual void TryBuySpin()
        {
            if (SpinHandler.Data.CanSpin(Bank.Data.Money) == false)
            {
                Debug.LogWarning("Not enough money to spin!");
                return;
            }
            
            if (SpinHandler.scrollCharactersContent.childCount > 0)
                Bank.ChangeValueGold(-SpinHandler.Data.moneyForSpin);
        }

        protected abstract void Spin();

        protected abstract void Stop();

        private void FixedUpdate()
        {
            Spin();
        }
    }
}