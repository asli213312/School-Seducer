using UnityEngine;
using Zenject;

namespace _School_Seducer_.Editor.Scripts.Installers
{
    public class BankInstaller : MonoInstaller
    {
        [SerializeField] private Bank _bank;
        
        public override void InstallBindings()
        {
            Container.BindInstance(_bank);
        }
    }
}