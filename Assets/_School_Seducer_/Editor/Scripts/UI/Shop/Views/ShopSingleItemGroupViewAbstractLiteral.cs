using System;
using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using _School_Seducer_.Editor.Scripts.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _School_Seducer_.Editor.Scripts.UI.Shop
{
    public abstract class ShopSingleItemGroupViewAbstractLiteral : ShopSingleItemGroupViewBase
    {
        [SerializeField] private TimerUniqueBehaviour timerComponent;
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private TextMeshProUGUI valueText;
        
        protected override IShopItemDataBase Data {get => LiteralData; set => LiteralData = value as ShopSingleItemAbstractLiteralData; }

        protected abstract ShopSingleItemAbstractLiteralData LiteralData { get; set; }

        protected override void Awake()
        {
            buyButton.AddListener(Buy);
        }

        public override void AdditionalRender()
        {
            if (LiteralData.useTimer)
            {
                TimeSpan timeSpan = TimeSpan.FromMinutes(LiteralData.currentAwaitedTime);
                
                timerComponent.InitializeTimeParameters(LiteralData.currentAwaitedTime, LiteralData.timeToWait);
                timerComponent.InitializeFormat(timeSpan, string.Format($"{timeSpan.Hours}h {timeSpan.Minutes}min"));
                timerComponent.StartedEvent += () => buyButton.interactable = false;
                timerComponent.FinishedEvent += () => buyButton.interactable = true;
                timerComponent.FinishedEvent += () => timerText.text = "Time is out!";
                
                buyButton.AddListener(() => timerComponent.Restart());
                
                timerComponent.StartTimer();
            }
            
            valueText.text = LiteralData.value.ToString();
        }

        protected override void OnDestroy()
        {
            buyButton.RemoveListener(Buy);
            
            if (LiteralData.useTimer)
            {
                buyButton.RemoveListener(() => timerComponent.Restart());
                
                LiteralData.currentAwaitedTime = (int)timerComponent.CurrentTime;
                timerComponent.StartedEvent -= () => buyButton.interactable = false;
                timerComponent.FinishedEvent -= () => buyButton.interactable = true;
                timerComponent.FinishedEvent -= () => timerText.text = "Time is out!";
            }
        }
    }
}