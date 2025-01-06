using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using TMPro;
using GameAnalyticsSDK;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Linq;

namespace _School_Seducer_.Editor.Scripts.UI.Map
{
    public class MapSelectorBase : MonoBehaviour, IMapSelectorModule
    {
        [Header("Data")]
        [SerializeField] private LocationContainer[] locations;
        [SerializeField] private CharacterOnLocationView locationPrefab;
        
        [Header("UI")]
        [SerializeField] private RectTransform content;
        [SerializeField] private Transform contentParent;
        [SerializeField] private Transform comingSoonParent;
        [SerializeField] private RectTransform bgLocker;
        [SerializeField] private Button closeContentButton;
        [SerializeField] private TextMeshProUGUI locationName;

        [Header("Events")]
        [SerializeField] private UnityEvent characterSelectedEvent;

        protected MapSystem System;

        public LocationContainer CurrentLocation { get; private set; }

        private Button _comingSoonCloseButton;
        private Transform _currentContentParent;

        public void InitializeCore(MapSystem system)
        {
            System = system;
        }

        public void Initialize()
        {
            _comingSoonCloseButton = comingSoonParent.transform.GetChild(0).GetComponent<Button>();

            foreach (var location in locations)
            {
                location.InitializeCore(this);
                location.Initialize(content, locationPrefab);
                location.ShowContentEvent += OnLocationSelected;
                location.CharacterSelected += OnCloseContent;
                location.CharacterSelected += (character) => characterSelectedEvent?.Invoke();
            }
                        
            closeContentButton.AddListener(OnCloseContent);
            _comingSoonCloseButton.AddListener(OnCloseContent);
        }

        public void UpdateLocationsHeaderPanel() => locations.ForEach(x => x.RenderHeaderPanel());
        public void UpdateForNotify() => Invoke(nameof(UpdateLocationsForNotify), 0.3f);
        public void RegisterCharacterOnLocation(Character character) => System.Previewer.RegisterCharacter(character);
        public void UnregisterCharactersOnLocation(Character character) => System.Previewer.UnregisterCharacter(character);
        
        private void UpdateLocationsForNotify() => locations.ForEach(x => x.CheckNotify());

        private void OnCloseContent(Character character)
        {
            CloseContent();

            CurrentLocation = null;
        }

        private void OnCloseContent()
        {
            CloseContent();
        }

        private void CloseContent()
        {
            DeactivateHighlight();
            DeactivateContentParent();
            CurrentLocation.UnregisterCharacters();

            GameAnalytics.NewDesignEvent("main_" + CurrentLocation.name + "_close");
            Debug.Log("main_" + CurrentLocation.name + "_close");
        }
        
        private void DeactivateContentParent() => _currentContentParent.gameObject.Deactivate();

        private void DeactivateHighlight()
        {
            CurrentLocation.HighLight.gameObject.Deactivate();
            bgLocker.gameObject.Deactivate();
        }

        private void OnLocationSelected(LocationContainer location)
        {
            if (CurrentLocation != null)
            {
                CloseContent();
            }
            
            CurrentLocation = location;
            locationName.text = CurrentLocation.name.ToUpper();
            
            foreach (var characterInPreviewer in System.Previewer.Characters) 
            {
                System.Previewer.SetLockedConversation(characterInPreviewer);
            }

            if (location.Data.characters.Length > 0)
            {
                _currentContentParent = contentParent;
            }
            else
            {
                _currentContentParent = comingSoonParent;
            }

            ActivateContentParent();
            SetContentAtLocation();
            ActivateLockerScreen();
            ActivateHighlight();
        }

        private void SetContentAtLocation() => CurrentLocation.SetPositionContent(_currentContentParent);
        private void ActivateContentParent() => _currentContentParent.gameObject.Activate();
        private void ActivateHighlight() => CurrentLocation.HighLight.gameObject.Activate();
        private void ActivateLockerScreen() => bgLocker.gameObject.Activate();

        private void OnDestroy()
        {
            foreach (var location in locations)
            {
                location.ShowContentEvent -= OnLocationSelected;
                location.CharacterSelected -= OnCloseContent;
                location.CharacterSelected -= (character) => characterSelectedEvent?.Invoke();
            }
            
            closeContentButton.RemoveListener(OnCloseContent);
            _comingSoonCloseButton.RemoveListener(OnCloseContent);
        }
    }
}