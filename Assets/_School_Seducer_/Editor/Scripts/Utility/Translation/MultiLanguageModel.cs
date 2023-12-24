using System.Collections.Generic;
using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.Utility.Translation
{
    public class MultiLanguageModel
    {
        public string LanguageKey;
    }

    public class MultiLanguageTextModel : MultiLanguageModel
    {
        [TextArea]
        public string Value;
    }

    public class MultiLanguageImageModel : MultiLanguageModel
    {
        public Sprite Value;
    }
    
    public class MultiLanguageTextListModel : MultiLanguageModel
    {
        [TextArea(3,10)] [NonReorderable]
        public List<string> Value;
    }
}