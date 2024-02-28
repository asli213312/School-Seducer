using _School_Seducer_.Editor.Scripts.Utility;
using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.Installers
{
    [RequireComponent(typeof(MonoController))]
    public class MonoControllerInstaller : Zenject.MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<MonoController>().FromComponentInHierarchy().AsSingle();
        }
    }
}