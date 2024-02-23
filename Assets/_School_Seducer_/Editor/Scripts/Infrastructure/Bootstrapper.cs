using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.Infrastructure
{
    public class Bootstrapper : MonoBehaviour
    {
        private IDevice _device;
        private IUpdateable _inputSystem;
        private IInputProvider _inputProvider;
        
        private void Awake()
        {
            BootstrapperFactory bootstrapperFactory = new BootstrapperFactory();
            
            _device = bootstrapperFactory.CreateDevice();
            _device.InitializeBootstrapper();

            _inputProvider = bootstrapperFactory.CreateInputProvider();
            _inputSystem = new InputSystem(_inputProvider);
        }

        private void Update()
        {
            _inputSystem.Tick();
        }
    }

    public class AndroidDevice : IDevice
    {
        public void InitializeBootstrapper()
        {
            throw new System.NotImplementedException();
        }
    }

    public class WebDevice : IDevice
    {
        public void InitializeBootstrapper()
        {
            throw new System.NotImplementedException();
        }
    }
    
    public class DesktopDevice : IDevice
    {
        public void InitializeBootstrapper()
        {
            throw new System.NotImplementedException();
        }
    }
}