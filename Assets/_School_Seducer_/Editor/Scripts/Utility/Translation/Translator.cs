using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _School_Seducer_.Editor.Scripts.Utility.Translation
{
    [Serializable]
    public class Translator
    {
        [ReadOnly] public List<LanguagesMessage> languages;

        [Serializable]
        public class LanguagesMessage
        {
            public string languageCode;
            public List<SerializedMessage> messages;
        }

        [Serializable]
        public class LanguagesBase
        {
            public string languageCode;
        }

        [Serializable]
        public class Languages : LanguagesBase
        {
            public string key;
        }

        [Serializable]
        public class LanguagesImage : LanguagesBase
        {
            public Sprite key;
        }

        [Serializable]
        public class LanguageAudioClip
        {
            public string languageCode;
            public AudioClip key;
        }

        [Serializable]
        public class SerializedMessage
        {
            public string key;
        }
    }
}