using System;
using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.UI.Popups
{
    [Serializable]
    public struct TransitionDataText : ITransitionData
    {
        [SerializeField] public string dataText;
    }
}