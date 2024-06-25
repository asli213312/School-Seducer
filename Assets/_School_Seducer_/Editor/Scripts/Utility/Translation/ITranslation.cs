using System;
using System.Collections.Generic;

namespace _School_Seducer_.Editor.Scripts.Utility.Translation
{
    public interface ITranslation
    {
        public List<Translator.LanguagesText> LanguagesProperty { get; set; }
    }
}