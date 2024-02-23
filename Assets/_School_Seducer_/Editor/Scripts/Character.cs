using System;
using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using _School_Seducer_.Editor.Scripts;
using _School_Seducer_.Editor.Scripts.Chat;
using UnityEngine;
using Zenject;

public class Character : MonoBehaviour
{
    [Inject] private EventManager _eventManager;
    [Inject] private ChatSystem _chatSystem;
    
    [SerializeField] private CharacterData characterData;
    [SerializeField] private SpriteRenderer spriteRenderer;

    public СonversationData  currentConversation => characterData.currentConversation;
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
       
       _chat.gameObject.SafeActivate();
       _chatSystem.gameObject.SafeActivate();
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
        _chat.ResetContent();
        _chat.InvokeStartConversation(characterData.currentConversation.Messages);
        
        Debug.Log("Conversation is started: " + characterData.currentConversation.name);
    }

    public void EndConversation()
    {
        _eventManager.ConversationEnded();
    }
}
