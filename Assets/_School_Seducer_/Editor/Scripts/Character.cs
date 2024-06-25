using System;
using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using _School_Seducer_.Editor.Scripts;
using _School_Seducer_.Editor.Scripts.Chat;
using GameAnalyticsSDK;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class Character : MonoBehaviour, IPointerDownHandler
{
    [Inject] private EventManager _eventManager;
    [Inject] private ChatSystem _chatSystem;
    [Inject] private DiContainer _diContainer;
    
    [SerializeField] private CharacterData characterData;
    [SerializeField] private SpriteRenderer spriteRenderer;

    public СonversationData CurrentConversation => characterData.currentConversation;
    public CharacterData Data => characterData;
    public SpriteRenderer SpriteRenderer => spriteRenderer;

    private BoxCollider2D _collider;
    public event Action<Character> CharacterSelected;
    public event Action<Character> CharacterEnter;
    public event Action CharacterExit;
    
    public void Initialize(CharacterData data) => characterData = data;

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("This character: " + gameObject.name);
        CharacterSelected?.Invoke(this);

        GameAnalytics.NewDesignEvent("main_girl_" + characterData.name);
        Debug.Log("main_girl_" + characterData.name);
        
        //_chatSystem.gameObject.SafeActivate();
    }

    public void OnMouseEnter()
    {
        CharacterEnter?.Invoke(this);
    }

    public void OnMouseExit()
    {
        CharacterExit?.Invoke();
    }
}
