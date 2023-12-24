using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

namespace _School_Seducer_.Editor.Scripts.Utility.Translation
{
    public class LocalizedUIObject : MonoBehaviour, IObservableCustom<MonoBehaviour>
    {
        [Inject] private LocalizedGlobalMonoBehaviour _localizer;

        [SerializeField] private TextMeshProUGUI textPro;
        [SerializeField] private List<Translator.Languages> localizedData;

        private Text _text;

        private void Awake()
        {
            _text = textPro == null ? GetComponent<Text>() : null;

            _localizer.AddObserver(this);
        }

        private void OnDestroy()
        {
            _localizer.RemoveObserver(this);
        }

        public void OnObservableUpdate()
        {
            Translator.Languages currentLanguage = localizedData.Find(x => x.languageCode == _localizer.GlobalLanguageCodeRuntime);
            
            if (_text != null) _text.text = currentLanguage.key;
             else textPro.text = currentLanguage.key;
        }
    }
}