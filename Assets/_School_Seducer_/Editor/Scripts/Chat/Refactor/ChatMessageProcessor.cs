using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace _School_Seducer_.Editor.Scripts.Chat
{
    public class ChatMessageProcessor : ChatMessageProcessorBase
    {[Inject] private EventManager _eventManager;
             
             [SerializeField] private RectTransform paddingPrefab;
             
             public СonversationData CurrentConversation => _chatSystem.Initializator.CurrentCharacter.CurrentConversation;
             public List<MessageData> Messages { get; private set; } = new();
     
             public event Action TapEvent;
             
             private ChatSystem _chatSystem;
     
             private IChatStateHandlerModule _stateHandlerModule;
     
             public void InitializeCore(ChatSystem chatSystem)
             {
                 _chatSystem = chatSystem;
     
                 _stateHandlerModule = _chatSystem.StateHandler;
             }
     
             public void StartProcessMessages()
             {
                 StartCoroutine(ChatTap(InvokeLoadMessages));
             }
     
             private void InvokeLoadMessages()
             {
                 StartCoroutine(LoadMessages(CurrentConversation.Messages.ToList()));
             }
     
             private IEnumerator LoadMessages(List<MessageData> messages = null)
             {
                 if (messages != null)
                     Messages.AddRange(messages);
     
                 for (int i = 0; i < Messages.Count; i++)
                 {
                     if (_stateHandlerModule.CheckEndConversation()) StopCoroutine(ProcessMessage(i, messages[i]));
                         
                     yield return ProcessMessage(i, messages[i]);
                 }
     
                 _stateHandlerModule.TryEndConversation(CheckLastMessage(Messages.ToArray(), Messages.Count - 1, false));
                 _stateHandlerModule.EndConversation();
             }
     
             private IEnumerator ProcessMessage(int index, MessageData messageData)
             {
                 _eventManager.ChatMessageReceived(false);
                 
                 yield return new WaitUntil(CheckTapBounds);
                 yield return new WaitForSeconds(0.5f);
     
                 
             }
     
             private IEnumerator ChatTap(Action onTapped)
             {
                 yield return new WaitUntil(CheckTapBounds);
                 onTapped?.Invoke();
                 TapEvent?.Invoke();
             }
     
             private bool CheckTapBounds()
             {
                 if (Input.GetMouseButtonDown(0))
                 {
                     Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                     
                     BoxCollider2D collider = gameObject.GetComponent<BoxCollider2D>();
                     
                     RaycastHit2D[] hits = Physics2D.RaycastAll(mousePosition, Vector2.zero, Mathf.Infinity);
     
                     foreach (var hit in hits)
                     {
                         GameObject hitObject = hit.collider.gameObject;
                         if (hitObject.gameObject.layer == LayerMask.NameToLayer("Unclickable"))
                         {
                             Debug.Log("Clicked on Unclickable area!");
                             return false;
                         }
     
                         return true;
                     }
                 }
     
                 return false;
             }
             
             private bool CheckLastMessage(MessageData[] messages, int i, bool lastMessage)
             {
                 if (i == messages.Length - 1)
                 {
                     lastMessage = true;
                 }
                 
                 return lastMessage;
             }
             
             private RectTransform CreatePadding()
             {
                 return Instantiate(paddingPrefab, _chatSystem.ContentMsg);
             }
        
    }
}