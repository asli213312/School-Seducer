using System.Collections;
using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using _School_Seducer_.Editor.Scripts.Utility;
using _School_Seducer_.Editor.Scripts.Utility.Translation;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Events;

namespace _School_Seducer_.Editor.Scripts.Services.Tutorial.Other
{
    public class TutorialContractUtility : TutorialContractBase
    {
        [SerializeField] private Transform optionsPanel;
        [SerializeField] private Transform languagePanel;
        [SerializeField] private SwitchLocalization[] switchersLocalization;

        private bool _isCompleted;

        private void Awake()
        {
            foreach (var switcher in switchersLocalization)
            {
                switcher.LanguageChangedEvent += DeactivateOptionsPanel;
            }
        }
        
        protected override IEnumerator Process()
        {
            yield return new WaitUntil(() => _isCompleted);
            Debug.Log("Deactivated Options Panel");
            switchersLocalization.ForEach(x => x.LanguageChangedEvent -= DeactivateOptionsPanel);
        }

        private void DeactivateOptionsPanel()
        {
            optionsPanel.gameObject.Deactivate();
            languagePanel.gameObject.Deactivate();
            _isCompleted = true;
        }
    }
}