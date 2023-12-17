using _BonGirl_.Editor.Scripts;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace _School_Seducer_.Editor.Scripts.Installers
{
    public class SoundHandlerInstaller : MonoInstaller
    {
        [FormerlySerializedAs("soundInvoker")] [SerializeField] private SoundHandler soundHandler;
        public override void InstallBindings()
        {
            Container.BindInstance(soundHandler);
        }
    }
}