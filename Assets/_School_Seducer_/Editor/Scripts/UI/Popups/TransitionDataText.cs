using System;
using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.UI.Popups
{
    [Serializable]
    public struct TransitionDataText : ITransitionData
    {
        [SerializeField] public string dataText;
        public string fieldName;
        
        public TransitionDataText(string data, string fieldName)
        {
            dataText = data;
            this.fieldName = fieldName;
        }
    }
}