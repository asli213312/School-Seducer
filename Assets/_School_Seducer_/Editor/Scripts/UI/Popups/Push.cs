using System;
using System.Collections.Generic;
using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using _School_Seducer_.Editor.Scripts.UI.Popups;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _School_Seducer_.Editor.Scripts.UI
{
    public class Push : MonoBehaviour
    {
        [SerializeField] private List<Transition> transitions = new();
        [SerializeField, Space(20)] private bool needOtherElements;
        [SerializeField, ShowIf(nameof(needOtherElements))] private Button[] buttons = { };

        public Button[] Buttons => buttons;

        public void InitializeButton(int indexButton, UnityAction action)
        {
            if (buttons.Length <= 0) return;

            for (int i = 0; i < buttons.Length; i++)
            {
                if (indexButton == i) 
                    buttons[i].AddListener(action); break; 
            }
        }

        public void SetDataParent(IDataParent dataParent)
        {
            foreach (var transition in transitions)
            {
                transition.SetDataParent(dataParent);
            }
        }

        public void MakeTransitions()
        { 
            foreach (var transition in transitions)
            {
                transition.Transit();
            }
        }

        public void InitializeTransitionParent(int indexTransition, IDataParent dataParent)
        {
            for (int i = 0; i < transitions.Count; i++)
            {
                if (i == indexTransition)
                {
                    var selectedTransition = transitions[i];
                    selectedTransition.SetDataParent(dataParent);
                    selectedTransition.Transit();
                    break;
                }
                
                Debug.LogError("Transition not found with name: ");
            }
        }

        public void InitializeData(ITransitionData data)
        {
            foreach (var transition in transitions)
            {
                transition.SetData(data);
            }
        }

        private void OnEnable()
        {
            foreach (var transition in transitions)
            {
                transition.Transit();
            }
        }
    }
}