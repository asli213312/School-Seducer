using System.Collections;
using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using UnityEngine;
using UnityEngine.Events;

namespace _School_Seducer_.Editor.Scripts.Services.Tutorial.Core
{
    public class TutorialContractStory : TutorialContractBase
    {
        [SerializeField] private Chat.Chat mainChat;
        [SerializeField] private UnityEvent conversationCompletedEvent;
        [SerializeField] private UnityEvent conversationFailedEvent;
        
        private bool _completed;
        private bool _isStarted;

        private void Update()
        {
            if (_isStarted == false) return;

            if (mainChat.IsMessagesEnded && _completed == false)
            {
                conversationCompletedEvent?.Invoke();
                conversationFailedEvent.RemoveAllListeners();
                
                _isStarted = true;
                _completed = true;
                
                Debug.Log("Tutorial contract story completed! " + name);
                
                gameObject.Deactivate();
            }
        }

        public void ResetProcess() => _isStarted = false;
        public void Start() => _isStarted = true;
        public void End() => gameObject.Deactivate();
        public void EndConversation()
        {
            if (_completed) return;
            
            conversationFailedEvent?.Invoke();
            
            _isStarted = false;
            
            Debug.Log("Tutorial contract story failed! " + name);
        }

        protected override IEnumerator Process()
        {
            yield break;
        }
    }
}