using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace _School_Seducer_.Editor.Scripts.Utility.Translation
{
    [Serializable]
    public abstract class BaseLocalizator
    {
        public TextAsset translationJson;

        protected abstract void InitializeTranslation();
    }
}