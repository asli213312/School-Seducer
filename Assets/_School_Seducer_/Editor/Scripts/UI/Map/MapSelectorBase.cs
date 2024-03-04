using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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
        [SerializeField] private RectTransform bgLocker;
        [SerializeField] private Button closeContentButton;
        [SerializeField] private TextMeshProUGUI locationName;

        protected MapSystem System;

        private LocationContainer _currentLocation;

        public void InitializeCore(MapSystem system)
        {
            System = system;
        }

        public void Initialize()
        {
            foreach (var location in locations)
            {
                location.InitializeCore(this);
                location.Initialize(content, locationPrefab);
                location.ShowContentEvent += OnLocationSelected;
                location.CharacterSelected += OnCloseContent;
            }
            
            closeContentButton.AddListener(OnCloseContent);
        }

        public void RegisterCharacterOnLocation(Character character) => System.Previewer.RegisterCharacter(character);
        public void UnregisterCharactersOnLocation(Character character) => System.Previewer.UnregisterCharacter(character);

        private void OnCloseContent(Character character)
        {
            CloseContent();
        }

        private void OnCloseContent()
        {
            CloseContent();
        }

        private void CloseContent()
        {
            DeactivateHighlight();
            DeactivateContentParent();
            _currentLocation.UnregisterCharacters();
        }
        
        private void DeactivateContentParent() => contentParent.gameObject.Deactivate();

        private void DeactivateHighlight()
        {
            _currentLocation.transform.GetChild(0).gameObject.Deactivate();
            bgLocker.gameObject.Deactivate();
        }

        private void OnLocationSelected(LocationContainer location)
        {
            _currentLocation = location;
            locationName.text = _currentLocation.name.ToUpper();
            
            ActivateContentParent();
            SetContentAtLocation();
            ActivateLockerScreen();
            ActivateHighlight();
        }

        private void SetContentAtLocation() => _currentLocation.SetPositionContent(contentParent);
        private void ActivateContentParent() => contentParent.gameObject.Activate();
        private void ActivateHighlight() => _currentLocation.transform.GetChild(0).gameObject.Activate();
        private void ActivateLockerScreen() => bgLocker.gameObject.Activate();

        private void ResetContent()
        {
            if (content.childCount <= 0) return;
            
            for (int i = 0; i < content.childCount; i++)
            {
                content.GetChild(i).gameObject.Destroy(3f);
            }
        }

        private void OnDestroy()
        {
            foreach (var location in locations)
            {
                location.ShowContentEvent -= OnLocationSelected;
                location.CharacterSelected -= OnCloseContent;
            }
            
            closeContentButton.RemoveListener(OnCloseContent);
        }
    }
}