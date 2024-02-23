using System;
using System.Collections.Generic;
using _School_Seducer_.Editor.Scripts.Chat;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace _School_Seducer_.Editor.Scripts.UI
{
    [RequireComponent(
        typeof(IContentDisplay),
        typeof(IContentUserInteraction),
        typeof(IContentDataProvider))
    ]
    public class ContentScreenProxy : MonoBehaviour, IContentScreenModules
    {
        [Inject] private IContentDisplay _displayModule;
        [Inject] private IContentDataProvider _dataProviderModule;
        [Inject] private IContentUserInteraction _userInteractionModule;

        [ShowInInspector] public static OpenContentBase CurrentContent;

        private void Awake()
        {
            InitializeModules();
            
            void InitializeModules()
            {
                _displayModule.Initialize(_dataProviderModule);
                _userInteractionModule.Initialize(_displayModule, _displayModule as IContentAnimation);
            }
        }

        public IContentDisplay GetContentDisplay() => _displayModule;
        public IContentDataProvider GetContentDataProvider() => _dataProviderModule;
        public IContentUserInteraction GetContentUserInteraction() => _userInteractionModule;
    }
}