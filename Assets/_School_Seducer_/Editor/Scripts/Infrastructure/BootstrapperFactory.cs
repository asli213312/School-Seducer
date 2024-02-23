using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.Infrastructure
{
    public class BootstrapperFactory
    {
        public IDevice CreateDevice()
        {
            return CreateObjectForPlatform<IDevice>(
                new AndroidDevice(),
                new WebDevice(),
                new DesktopDevice()
            );
        }

        public IInputProvider CreateInputProvider()
        {
            return CreateObjectForPlatform<IInputProvider>(
                new UnityInputProvider(), 
                new UnityInputProvider(), 
                new UnityInputProvider()
            );
        }
        
        private T CreateObjectForPlatform<T>(T android, T web, T desktop)
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android: return android;
                case RuntimePlatform.WebGLPlayer: return web;
                case RuntimePlatform.WindowsPlayer: return desktop;
            }

            Debug.LogError("Unsupported platform: " + Application.platform);
            return desktop;
        }
    }
}