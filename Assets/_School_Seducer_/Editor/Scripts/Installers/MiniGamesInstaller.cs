using UnityEngine;
using Zenject;

namespace _School_Seducer_.Editor.Scripts.Installers
{
    public class MiniGamesInstaller : MonoInstaller
    {
        [SerializeField] private MiniGameInitializer miniGameInitializer;
        
        public override void InstallBindings()
        {
            Container.BindInstance(miniGameInitializer);
        }
    }
}