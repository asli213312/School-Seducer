using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.Infrastructure
{
    public class UnityInputProvider : IInputProvider
    {
        public bool GetButtonDown(int buttonIndex)
        {
            return Input.GetMouseButtonDown(buttonIndex);
        }
    }

    public interface IInputProvider
    {
        bool GetButtonDown(int buttonIndex);
    }
}