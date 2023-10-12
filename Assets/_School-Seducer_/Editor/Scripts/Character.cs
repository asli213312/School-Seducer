using System;
using _School_Seducer_.Editor.Scripts;
using UnityEngine;
using DialogueEditor;

public class Character : MonoBehaviour
{
    [SerializeField] private CharacterData characterData;
    [SerializeField] private NPCConversation dialogue;
    [SerializeField] private SpriteRenderer spriteRenderer;

    public CharacterData Data => characterData;
    public SpriteRenderer SpriteRenderer => spriteRenderer;
    public NPCConversation Dialogue => dialogue;
    
    private BoxCollider2D _collider;
    public event Action<Character> CharacterSelected;
    public event Action<Character> CharacterEnter;
    public event Action CharacterExit;
    
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
        ConversationManager.Instance.StartConversation(dialogue);
    }
}
