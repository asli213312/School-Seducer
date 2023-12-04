using UnityEngine;
using Zenject;

namespace _School_Seducer_.Editor.Scripts.Installers
{
    public class EventManagerInstaller : MonoInstaller
    {
        [SerializeField] private EventManager eventManager;
        
        public override void InstallBindings()
        {
            Container.BindInstance(eventManager).AsSingle();
        }
    }
}