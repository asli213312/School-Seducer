using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _School_Seducer_.Editor.Scripts
{
    public class Location : MonoBehaviour
    {
        [Inject] private EventManager _eventManager;
        
        [SerializeField] private LocationData data;
        public LocationData Data => data;
        public SpriteRenderer AvailabilitySprite { get; set; }

        //public event Action<Location> LocationSelected;
        public event Action<Location> LocationAvailabilityChanged;

        private void Awake()
        {
            AvailabilitySprite = GetComponentInChildren<SpriteRenderer>();
        }

        private void OnMouseDown()
        {
            //LocationSelected?.Invoke(this);
            LocationAvailabilityChanged?.Invoke(this);
            _eventManager.SelectLocation(this);
        }
    }
}