using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
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
        public class LanguagesTextOptions : LanguagesBase
        {
            public TextOptions options;
        }

        [Serializable]
        public class LanguagesText : LanguagesBase
        {
            public string key;
            public TextOptions options;
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
    
    [Serializable]
    public class TextOptions
    {
        public TMP_FontAsset font;
        private TextMeshProUGUI _text;

        public void Install(TextMeshProUGUI text)
        {
            _text = text;

            if (font != null) _text.font = font;
        }
    }
}