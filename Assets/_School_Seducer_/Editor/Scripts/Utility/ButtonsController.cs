using System;
using System.Linq;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _School_Seducer_.Editor.Scripts.Utility
{
    public class ButtonsController : MonoBehaviour
    {
        [Serializable]
        private class ButtonContainer
        {
            public Button button;
            public string id;
            public CollapsableEvents events;

            [Serializable]
            public class CollapsableEvents 
            {
                [Header("Events")]
                public UnityEvent enabledEvent;
                public UnityEvent disabledEvent;
            }
        }
        
        [Header("Global Events")]
        [SerializeField] private UnityEvent allEnabledEvent;
        [SerializeField] private UnityEvent allDisabledEvent;

        [SerializeField] private ButtonContainer[] buttons;

        public void EnableButtonById(string buttonId)
        {
            var foundButton = GetButtonContainerById(buttonId);
            
            foundButton.button.interactable = true;
            foundButton.events.enabledEvent?.Invoke();
        }

        public void DisableButtonById(string buttonId)
        {
            var foundButton = GetButtonContainerById(buttonId);

            foundButton.button.interactable = false;
            foundButton.events.disabledEvent?.Invoke();
        }
        public void EnableButtons() 
        {
            buttons.ForEach(x => x.button.interactable = true);
            allEnabledEvent?.Invoke(); 
        } 
        public void DisableButtons() 
        {
            buttons.ForEach(x => x.button.interactable = false);
            allDisabledEvent?.Invoke();
        } 

        private ButtonContainer GetButtonContainerById(string buttonId)
        {
            var foundContainerButton = buttons.FirstOrDefault(x => x.id == buttonId);

            if (foundContainerButton == null)
            {
                Debug.LogError("Button with id " + buttonId + " not found");
                return null;
            }

            return foundContainerButton;
        }
    }
}