using System;

namespace _School_Seducer_.Editor.Scripts.Infrastructure
{
    public class InputSystem : IUpdateable
    {
        public event Action OnTick; 
        
        private const int LEFT_MOUSE = 0;
        private readonly IInputProvider _inputProvider;
        
        public InputSystem(IInputProvider inputProvider)
        {
            _inputProvider = inputProvider;
        }
        
        public void Tick()
        {
            if (_inputProvider.GetButtonDown(LEFT_MOUSE)) OnTick?.Invoke();
        }
    }

    public interface IUpdateable
    {
        void Tick();
    }
}