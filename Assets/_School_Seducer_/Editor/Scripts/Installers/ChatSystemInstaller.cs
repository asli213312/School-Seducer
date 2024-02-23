using System;
using _School_Seducer_.Editor.Scripts.Chat;
using UnityEngine;
using Zenject;

namespace _School_Seducer_.Editor.Scripts.Installers
{
    [RequireComponent(typeof(ChatSystem))]
    public class ChatSystemInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<ChatSystem>().FromComponentsInHierarchy().AsSingle();
            Container.Bind<IChatInitialization>().FromComponentInHierarchy().AsSingle();
            Container.Bind<IChatMessageRendererModule>().FromComponentInHierarchy().AsSingle();
            Container.Bind<IChatMessageProcessorModule>().FromComponentInHierarchy().AsSingle();
            Container.Bind<IChatStateHandlerModule>().FromComponentInHierarchy().AsSingle();
            Container.Bind<IChatStoryResolverModule>().FromComponentInHierarchy().AsSingle();
        }

        private void OnValidate()
        {
            if (TryGetComponent(out IChatInitialization initializationModule) && initializationModule == null)
                Debug.LogError("ChatSystemInstaller: Missing Initialization Module!");
        }
    }
}