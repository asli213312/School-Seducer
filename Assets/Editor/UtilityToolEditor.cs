using System.Collections.Generic;
using System.Linq;
using _School_Seducer_.Editor.Scripts.Utility.Translation;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor
{
    public class UtilityToolEditor : UnityEditor.Editor
    {
        [MenuItem("Tools/Remove 'ё' from russian")]
        private static void EditLocalizedData()
        {
            LocalizedUIText[] localizedUITexts = FindObjectsOfType<LocalizedUIText>();
            foreach (var locale in localizedUITexts)
            {
                Translator.LanguagesText russianLanguage = locale.LocalizedData.FirstOrDefault(x => x.languageCode == "ru");

                if (russianLanguage == null)
                {
                    Debug.Log("Can't find russian language for: " + locale.gameObject.name, locale.gameObject);
                    return;
                }

                List<char> newKey = new();
                foreach (var t in russianLanguage.key)
                {
                    var i = t;
                    if (i == 'ё')
                    {
                        i = 'е';
                    }
                    newKey.Add(i);
                }
                
                russianLanguage.key = new string(newKey.ToArray());
            }
        }
    }
}