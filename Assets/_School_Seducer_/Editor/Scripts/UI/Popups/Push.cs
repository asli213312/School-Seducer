using System.Collections.Generic;
using _School_Seducer_.Editor.Scripts.UI.Popups;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _School_Seducer_.Editor.Scripts.UI
{
    public class Push : MonoBehaviour
    {
        [SerializeField] private List<Transition> transitions = new();

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