using _BonGirl_.Editor.Scripts;
using UnityEngine;
using Zenject;

namespace _School_Seducer_.Editor.Scripts.Installers
{
    public class SoundInvokerInstaller : MonoInstaller
    {
        [SerializeField] private SoundInvoker soundInvoker;
        public override void InstallBindings()
        {
            Container.BindInstance(soundInvoker);
        }
    }
}