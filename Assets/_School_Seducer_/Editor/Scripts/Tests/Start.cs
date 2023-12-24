using System.Collections.Generic;
using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.Tests
{
    public class Start : MonoBehaviour
    {
        public int _a;
        private int _e = 5;
        private float _b = 4.5f;
        private string _c = "Привет";
        private bool _d = true;
        private List<float> floatList = new();
        private List<int> intList = new();

        [ContextMenu("Execute")]
        private float Method()
        {
            int localValue = 56 + _a;

            intList.Add(localValue);

            Debug.Log(localValue);

            return localValue;
        }

        public int Def()
        {
            int t = _a;
            return t;
        }

        private void Execute()
        {
            int a = Def();
            Debug.Log(a);
        }

        private void Last()
        {
            
        }
    }

    public class TestObject
    {
        public int intValue;
    }
}