using _School_Seducer_.Editor.Scripts.UI;
using UnityEngine;
using Zenject;

namespace _School_Seducer_.Editor.Scripts.Installers
{
    [RequireComponent(typeof(ContentScreenProxy))]
    public class ContentScreenInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IContentDisplay>().FromComponentInHierarchy().AsSingle();
            Container.Bind<IContentDataProvider>().FromComponentInHierarchy().AsSingle();
            Container.Bind<IContentUserInteraction>().FromComponentInHierarchy().AsSingle();
        }
    }
}