using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace _School_Seducer_.Editor.Scripts.Utility.Translation
{
    public abstract class LocalizedUIBase : MonoBehaviour, IObservableCustom<MonoBehaviour>
    {
        [Inject] protected LocalizedGlobalMonoBehaviour Localizer;
        protected abstract List<Translator.LanguagesBase> localizedData { get; }

        public void OnObservableUpdate()
        {
            OnChangeLanguage();
            UpdateView();
        }

        public abstract void UpdateView();
        protected abstract void OnChangeLanguage();
        protected Translator.LanguagesBase GetCurrentLanguage() => localizedData.Find(x => x.languageCode == Localizer.GlobalLanguageCodeRuntime);

        private void Awake()
        {
            if (Localizer != null) Localizer.AddObserver(this);
        }

        private void OnDestroy()
        {
            Localizer?.RemoveObserver(this);
        }
    }
}