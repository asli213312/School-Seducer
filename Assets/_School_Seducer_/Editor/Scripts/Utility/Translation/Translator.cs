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
        public List<LanguagesMessage> languages;

        [Serializable]
        public class LanguagesMessage
        {
            public string languageCode;
            public List<SerializedMessage> messages;
        }

        [Serializable]
        public class LanguagesUIObject
        {
            public string languageCode;
            //public List<SerializedUIObject> objects;
        }

        [Serializable]
        public class Languages
        {
            public string languageCode;
            public string key;
        }

        [Serializable]
        public class SerializedMessage
        {
            public string key;
        }
    }
}