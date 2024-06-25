using System;
using System.Collections.Generic;
using System.Linq;
using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using _School_Seducer_.Editor.Scripts.Chat;
using _School_Seducer_.Editor.Scripts.Extensions;
using TMPro;
using GameAnalyticsSDK;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _School_Seducer_.Editor.Scripts.UI.Map
{
    public class LocationContainer : MonoBehaviour, IPointerDownHandler, IModule<MapSelectorBase>
    {
        [SerializeField] private LocationData data;
        [SerializeField] private Transform contentPoint;
        [SerializeField] private TextMeshProUGUI locationName;
        [SerializeField] private TextMeshProUGUI charactersCount;
        [SerializeField] private Transform highlight;
        [SerializeField] private RectTransform notifyPoint;
        [SerializeField] private RectTransform notifyCanUnlockStoryPoint;
        [SerializeField] private RectTransform panelHeaderContent;
        [SerializeField] private RectTransform panelHeaderSlotPrefab;
        [SerializeField] private RectTransform panelHeaderMoreSlotsPrefab;

        public event Action<LocationContainer> ShowContentEvent;
        public event Action<Character> CharacterSelected;

        public Transform HighLight => highlight;

        private RectTransform _charactersContainer;
        private CharacterOnLocationView _prefabView;
        private List<Character> _characters = new();

        private MapSelectorBase _mapSelectorModule;

        public void InitializeCore(MapSelectorBase system)
        {
            _mapSelectorModule = system;
        }

        public void Initialize(RectTransform content, CharacterOnLocationView prefabView)
        {
            _charactersContainer = content;
            _prefabView = prefabView;
        }

        private void Start()
        {
            locationName.text = data.name;
            charactersCount.text = data.characters.Length.ToString();
            RenderHeaderPanel();
        }

        public void CheckNotify() 
        {
            CheckNotifyCharacter();
            CheckNotifyCanUnlockStory();
        }

        public void RenderHeaderPanel()
        {
            if (panelHeaderContent.childCount > 0)
            {
                ResetHeaderPanel();
            }
            
            int countCharacters = 0;
                
            foreach (var character in data.characters)
            {
                if (character.isLocked) continue;
                
                if (countCharacters == 3)
                {
                    RectTransform headerMoreSlots = Instantiate(panelHeaderMoreSlotsPrefab, panelHeaderContent);
                    int leftCharacters = data.characters.Length - countCharacters;
                    headerMoreSlots.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "+" + leftCharacters;
                    headerMoreSlots.transform.GetChild(1).gameObject.SetActive(true);

                    break;   
                }
                
                Image headerSlot = Instantiate(panelHeaderSlotPrefab, panelHeaderContent).GetComponent<Image>();
                headerSlot.sprite = character.info.onLocationSprite;

                if (character.HasUnseenConversations() && data.characters.Length <= 3) 
                {
                	headerSlot.transform.GetChild(0).gameObject.SetActive(true);
            	}
            	else headerSlot.transform.GetChild(0).gameObject.SetActive(false);

                countCharacters++;
            }

            CheckNotify();
        }

        public void SetPositionContent(Transform contentParent)
        {
            contentParent.position = contentPoint.position;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            ShowContentEvent?.Invoke(this);
            GameAnalytics.NewDesignEvent("main_" + name + "_open");
            Debug.Log("main_" + name + "_open");
            Debug.Log("Location selected: " + name);
            
            SpawnContent();
        }

        public void UnregisterCharacters()
        {
            if (_characters.Count > 0)
            {
                foreach (var character in _characters)
                {
                    _mapSelectorModule.UnregisterCharactersOnLocation(character);
                }
                
                _characters.Clear();
            }
        }

        private void CheckNotifyCharacter()
        {
            CharacterData[] unseenCharactersConversations = data.characters.Where(character =>
            {
                СonversationData[] unseenConversations = character.allConversations.Where(conversation =>
                    conversation.isUnlocked && !conversation.isSeen
                ).ToArray();
                
                return unseenConversations.Length > 0;
            }).ToArray();

            if (unseenCharactersConversations.Length == 0) return;
        }

        private void CheckNotifyCanUnlockStory() 
        {
            this.Deactivate(notifyCanUnlockStoryPoint);

            foreach (var character in data.characters)
                if (character.CanUnlockStory()) 
                {
                    this.Activate(notifyCanUnlockStoryPoint);
                    break;
                }
        }

        private void ResetHeaderPanel()
        {
            for (int i = 0; i < panelHeaderContent.childCount; i++)
            {
                Destroy(panelHeaderContent.GetChild(i).gameObject);
            }
        }

        private void SpawnContent()
        {
            if (_charactersContainer.childCount > 0)
            {
                for (int i = 0; i < _charactersContainer.childCount; i++)
                {
                    Destroy(_charactersContainer.GetChild(i).gameObject);
                }
            }
        
            foreach (var characterData in data.characters)
            {
                if (characterData.isLocked) continue;
                
                CharacterOnLocationView view = Instantiate(_prefabView, _charactersContainer);
                view.Render(characterData);
                view.Character.CharacterSelected += CharacterSelected;
                _mapSelectorModule.RegisterCharacterOnLocation(view.Character);
                _characters.Add(view.Character);
            }
        }
    }
}