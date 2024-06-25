using System.Collections;
using _School_Seducer_.Editor.Scripts.Chat;
using _School_Seducer_.Editor.Scripts.Utility;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace _School_Seducer_.Editor.Scripts.Services.Tutorial
{
    public class TutorialUtility : MonoBehaviour, IModule<TutorialSystem>
    {
        [SerializeField] public GlobalSettings GlobalSettings;
        [SerializeField] public SpineUtility Spine; 
        private TutorialSystem _system;

        public void InitializeCore(TutorialSystem system)
        {
            _system = system;
        }
    }
}