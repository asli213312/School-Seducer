using System.Collections;
using System.Linq;
using _School_Seducer_.Editor.Scripts.Chat;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Events;

namespace _School_Seducer_.Editor.Scripts.Services.Tutorial
{
    public abstract class TutorialControllerBase : MonoBehaviour, IModule<TutorialSystem>
    {
        [SerializeField, SerializeReference] protected TutorialContractBase[] Contracts;
        [SerializeField] private UnityEvent tutorialStartEvent;
        [SerializeField] private UnityEvent tutorialEndEvent;
        
        protected TutorialSystem System;
        protected TutorialContractBase CurrentContract;

        private bool _isWorking;

        public void InitializeCore(TutorialSystem system)
        {
            System = system;
        }

        public void Initialize() => Contracts.ForEach(x => x.Initialize(System.UtilityModule));

        public void StartTutorial()
        {
            if (System.GlobalSettings.TutorialCompleted) return;

            _isWorking = true;
            tutorialStartEvent?.Invoke();
            StartCoroutine(IterateContracts());
        }

        public void Continue() => _isWorking = true;
        public void Stop() => _isWorking = false;
        public void End() => StopCoroutine(IterateContracts());

        protected virtual bool ConditionToComplete() => CurrentContract == Contracts[^1];

        private IEnumerator IterateContracts()
        {
            foreach (var contract in Contracts)
            {
                CurrentContract = contract;

                yield return new WaitUntil(() => _isWorking);
                
                yield return contract.MainProcess();
            }

            CurrentContract = null;
            tutorialEndEvent?.Invoke();
            Debug.Log("Tutorial completed!");
        }
    }
}