using _School_Seducer_.Editor.Scripts.Utility;
using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.Services.Tutorial
{
    public class TutorialSystem : MonoBehaviour
    {
        [Header("Services")] 
        [SerializeField] private GlobalSettings globalSettings;

        [Header("Modules")]
        [SerializeField, SerializeReference] private TutorialControllerBase controller;
        [SerializeField, SerializeReference] private TutorialUtility utility;
        
        public GlobalSettings GlobalSettings => globalSettings;
        public TutorialUtility UtilityModule => utility;
        public TutorialControllerBase Controller => controller;

        private void Awake()
        {
            InitializeModules();

            void InitializeModules()
            {
                controller.InitializeCore(this);
                utility.InitializeCore(this);
                controller.Initialize();
            }
        }
    }
}