using _School_Seducer_.Editor.Scripts.Utility.Translation;
using UnityEngine;
using Zenject;

namespace _School_Seducer_.Editor.Scripts.Installers
{
    public class LocalizerInstaller : MonoInstaller
    {
        [SerializeField] private LocalizedGlobalMonoBehaviour globalLocalizer;
        
        public override void InstallBindings()
        {
            Container.BindInstance(globalLocalizer).AsSingle();
        }
    }
}