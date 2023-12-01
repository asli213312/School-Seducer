using System;
using _School_Seducer_.Editor.Scripts;
using _School_Seducer_.Editor.Scripts.Chat;
using UnityEngine;
using DialogueEditor;
using Zenject;

public class Character : MonoBehaviour
{
    [Inject] private EventManager _eventManager; 
    
    [SerializeField] private CharacterData characterData;
    [SerializeField] private SpriteRenderer spriteRenderer;

    public СonversationData  Conversation => characterData.conversation;
    public CharacterData Data => characterData;
    public SpriteRenderer SpriteRenderer => spriteRenderer;

    private BoxCollider2D _collider;
    public event Action<Character> CharacterSelected;
    public event Action<Character> CharacterEnter;
    public event Action CharacterExit;

    private Chat _chat;
    
    public void Initialize(Chat chat)
    {
        _chat = chat;
    }
    
    void Start()
    {
        _collider = GetComponent<BoxCollider2D>();
    }

    public void OnMouseDown()
    {
       Debug.Log("This character: " + gameObject.name);
       CharacterSelected?.Invoke(this);
    }

    public void OnMouseEnter()
    {
        CharacterEnter?.Invoke(this);
    }

    public void OnMouseExit()
    {
        CharacterExit?.Invoke();
    }

    public void StartConversation()
    {
        _chat.InvokeStartConversation(characterData.conversation.Messages);
    }

    public void EndConversation()
    {
        _eventManager.ConversationEnded();
    }
}
