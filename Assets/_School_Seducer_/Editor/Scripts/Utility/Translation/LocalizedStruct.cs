using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace _School_Seducer_.Editor.Scripts.Utility.Translation
{
    [Serializable]
    public struct LocalizedStruct
    {
        public List<LocalizedData> localizedField;

        [Serializable]
        public class LocalizedData
        {
            public string languageCode;
            public string key;
        }  
    }
}