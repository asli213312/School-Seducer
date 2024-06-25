using System;
using System.Collections;
using PuzzleGame.Gameplay;
using PuzzleGame.Gameplay.Merged;
using UnityEngine;
using UnityEngine.Events;

namespace _School_Seducer_.Editor.Scripts.Services.Tutorial
{
    public class TutorialContractMiniGame : TutorialContractBase
    {
        [SerializeField] private UnityEvent rotatedDicesEvent;
        [SerializeField] private UnityEvent brickPlacedEvent;

        private void Awake()
        {
            FigureController.FiguresRotatedAction += InvokeDicesEvent();
            GameControllerMerged.OnBrickPlaced += InvokeBrickPlacedEvent();
            rotatedDicesEvent.AddListener(() => OnRotatedDices());
            brickPlacedEvent.AddListener(() => BrickPlaced());
        }

        private void OnDestroy()
        {
            FigureController.FiguresRotatedAction -= InvokeDicesEvent();
            GameControllerMerged.OnBrickPlaced -= InvokeBrickPlacedEvent();
            rotatedDicesEvent.RemoveListener(() => OnRotatedDices());
            brickPlacedEvent.RemoveListener(() => BrickPlaced());
        }

        public void InvokeWaitUntilBrickPlaced()
        {
            IEnumerator ProcessUntilBrickPlaced()
            {
                yield return new WaitUntil(BrickPlaced);
            }
        }

        public void InvokeWaitUntilDicesRotated()
        {
            IEnumerator ProcessUntilDicesRotated()
            {
                yield return new WaitUntil(OnRotatedDices);
            }
        }

        protected override IEnumerator Process()
        {
            yield return new WaitUntil(OnRotatedDices);
        }

        private Action InvokeBrickPlacedEvent() => () => brickPlacedEvent?.Invoke();
        private Action InvokeDicesEvent() => () => rotatedDicesEvent?.Invoke();

        private bool BrickPlaced() => true;

        private bool OnRotatedDices()
        {
            return true;
        }
    }
}