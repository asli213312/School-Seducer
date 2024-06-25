using System;

namespace _School_Seducer_.Editor.Scripts.Utility
{
    [Serializable]
    public class Condition
    {
        private readonly bool _isTrue;
        
        public Condition(bool condition)
        {
            _isTrue = condition;
        }

        public bool IsTrue() => _isTrue;
    }
}