using System;
using System.Collections.Generic;
using _School_Seducer_.Editor.Scripts.Chat;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _School_Seducer_.Editor.Scripts.UI.Map
{
    public class LocationContainer : MonoBehaviour, IPointerDownHandler, IModule<MapSelectorBase>
    {
        [SerializeField] private LocationData data;
        [SerializeField] private Transform contentPoint;

        public event Action<LocationContainer> ShowContentEvent;
        public event Action<Character> CharacterSelected;

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

        public void SetPositionContent(Transform contentParent)
        {
            contentParent.position = contentPoint.position;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            ShowContentEvent?.Invoke(this);
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
                CharacterOnLocationView view = Instantiate(_prefabView, _charactersContainer);
                view.Render(characterData);
                view.Character.CharacterSelected += CharacterSelected;
                _mapSelectorModule.RegisterCharacterOnLocation(view.Character);
                _characters.Add(view.Character);
            }
        }
    }
}