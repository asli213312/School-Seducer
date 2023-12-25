using System;
using System.Collections.Generic;
using System.Reflection;
using _School_Seducer_.Editor.Scripts.Chat;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _School_Seducer_.Editor.Scripts.Utility.Translation
{
    public class LocalizedGlobalMonoBehaviour : MonoBehaviour, IObserverCustom<MonoBehaviour>
    {
        [SerializeField] private List<Text> localizedUIObjects = new();
        [SerializeField] private string globalLanguageCodeRuntime;
        public string GlobalLanguageCodeRuntime { get => globalLanguageCodeRuntime; set => globalLanguageCodeRuntime = value; }
        private List<IObservableCustom<MonoBehaviour>> _localizedObjects = new();

        private void Awake()
        {
            globalLanguageCodeRuntime = GlobalSettings.GlobalCurrentLanguage;
        }

        private void OnDestroy()
        {
            _localizedObjects.Clear();
        }

        public void AddObserver(IObservableCustom<MonoBehaviour> observable)
        {
            if (IsOriginalObservable(observable)) _localizedObjects.Add(observable);
        }

        public void RemoveObserver(IObservableCustom<MonoBehaviour> observable)
        {
            if (observable is null) return;
            
            _localizedObjects.Remove(observable);
        }

        public void Notify()
        {
            foreach (var localizedObject in _localizedObjects)
            {
                localizedObject.OnObservableUpdate();
            }
        }

        private bool IsOriginalObservable(IObservableCustom<MonoBehaviour> observable)
        {
            return _localizedObjects.Contains(observable) == false;
        }
    }
}