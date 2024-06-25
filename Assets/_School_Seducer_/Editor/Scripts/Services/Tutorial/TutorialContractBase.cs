using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace _School_Seducer_.Editor.Scripts.Services.Tutorial
{
    public abstract class TutorialContractBase : MonoBehaviour
    {
        [SerializeField] private UnityEvent startContractEvent;
        [SerializeField] private UnityEvent endContractEvent;
        protected TutorialUtility Utility;
        
        public void Initialize(TutorialUtility utility) => Utility = utility; 
        
        public void Begin()
        {
            StartCoroutine(MainProcess());
        }

        public IEnumerator MainProcess()
        {
            startContractEvent?.Invoke();
            yield return Process();
            endContractEvent?.Invoke();
        }
        protected abstract IEnumerator Process();
    }
}