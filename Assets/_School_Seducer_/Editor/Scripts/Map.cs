using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using UnityEngine;
using Zenject;

namespace _School_Seducer_.Editor.Scripts
{
    public class Map : MonoBehaviour
    {
        [Inject] private EventManager _eventManager;
        
        [SerializeField] private LocationsConfig locationsConfig;
        [SerializeField] private PlayerConfig playerConfig;
        [SerializeField] private Location[] locations;
        
        private SpriteRenderer _renderer;

        private const int MIN_ORDER = 0;

        private Location _currentLocation;
        private LocationData _currentLocationData;

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();

            RegisterLocations();
        }

        private void OnDestroy()
        {
            UnRegisterLocations();
        }

        private void Start()
        {
            foreach (var location in locations)
            {
                if (location.Data.CanUnlock(playerConfig.Level))
                {
                    location.Data.UnlockLocation();
                    location.AvailabilitySprite.sprite = locationsConfig.LocationIsUnLocked;
                }
                else
                {
                    location.AvailabilitySprite.sprite = locationsConfig.LocationIsLocked;
                    location.Data.LockLocation();
                }
                
                StartCoroutine(location.AvailabilitySprite.FadeOut(1f));
            }
        }

        private void LocationSelected(Location location)
        {
            _currentLocation = location;
            _currentLocationData = location.Data;
        }

        public void CloseMap()
        {
            _renderer.SetSortingOrderForAllChildrens(MIN_ORDER);
        }
        
        private void RegisterLocations()
        {
            foreach (var location in locations)
            {
                _eventManager.LocationSelectedEvent += LocationSelected;
                location.LocationAvailabilityChanged += OnChangeAvailability;
            }
        }
        
        private void UnRegisterLocations()
        {
            foreach (var location in locations)
            {
                _eventManager.LocationSelectedEvent -= LocationSelected;
                location.LocationAvailabilityChanged -= OnChangeAvailability;
            }
        }

        private void OnChangeAvailability(Location location)
        {
            if (location.Data.CanUnlock(playerConfig.Level))
            {
                location.AvailabilitySprite.sprite = locationsConfig.LocationIsUnLocked;
            }
            else
            {
                location.AvailabilitySprite.sprite = locationsConfig.LocationIsLocked;
            }
        }
    }
}