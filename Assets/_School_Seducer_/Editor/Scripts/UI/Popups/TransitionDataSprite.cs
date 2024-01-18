using System;
using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.UI.Popups
{
    [Serializable]
    public struct TransitionDataSprite : ITransitionData
    {
        [SerializeField] public Sprite dataSprite;
    }
}