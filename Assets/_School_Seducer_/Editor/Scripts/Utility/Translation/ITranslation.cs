using System;
using System.Collections.Generic;

namespace _School_Seducer_.Editor.Scripts.Utility.Translation
{
    public interface ITranslation
    {
        public List<Translator.Languages> LanguagesProperty { get; set; }
    }
}