using System.Collections.Generic;
using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using _School_Seducer_.Editor.Scripts.Chat;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _School_Seducer_.Editor.Scripts.UI.Wheel_Fortune
{
    public class WheelFortuneSystem : MonoBehaviour
    {
        [Inject] private Bank _bank;
        [Inject] private EventManager _eventManager;

        [Header("Data")] 
        [SerializeField] private Previewer previewer;
        [SerializeField] private WheelFortuneData data;

        [Header("UI elements")]
        [SerializeField] private TextMeshProUGUI goldText;

        [Header("Modules")] 
        [SerializeField] private SpinHandlerModule spinHandlerModule;

        public WheelFortuneData Data => data;
        public Previewer Previewer => previewer;

        private void Awake()
        {
            spinHandlerModule.InitializeCore(this);
            spinHandlerModule.Initialize();

            _eventManager.ChangeValueExperienceEvent += spinHandlerModule.OnChangeValueExperience;
            _eventManager.UpdateTextMoneyEvent += UpdateGoldText;
        }
        
        private void OnDestroy()
        {
            _eventManager.ChangeValueExperienceEvent -= spinHandlerModule.OnChangeValueExperience;
            
            _eventManager.UpdateTextMoneyEvent -= UpdateGoldText;
        }

        private void UpdateGoldText()
        {
            goldText.text = _bank.Money.ToString();
        }
    }
}