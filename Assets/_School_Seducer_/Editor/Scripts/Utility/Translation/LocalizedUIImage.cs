using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace _School_Seducer_.Editor.Scripts.Utility.Translation
{
    public class LocalizedUIImage : LocalizedUIBase
    {
        [SerializeField] private Image image;
        [SerializeField] private List<Translator.LanguagesImage> _localizedData;

        public Sprite CurrentSprite { get; private set; }

        protected override List<Translator.LanguagesBase> localizedData => _localizedData.Cast<Translator.LanguagesBase>().ToList();

        private void Start()
        {
            UpdateView();
        }

        public override void UpdateView()
        {
            OnChangeLanguage();
            image.sprite = CurrentSprite;
        }

        protected override void OnChangeLanguage()
        {
            if (GetCurrentLanguage() is not Translator.LanguagesImage currentLanguage) return;    

            if (currentLanguage.key is not null)
            {
                image.sprite = currentLanguage.key;
                CurrentSprite = currentLanguage.key;
            }
            else
                Debug.LogError("Localized key not found: " + currentLanguage.key, gameObject);
        }
    }
}