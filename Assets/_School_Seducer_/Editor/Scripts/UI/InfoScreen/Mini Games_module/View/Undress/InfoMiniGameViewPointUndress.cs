using TMPro;
using System;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;
using UnityEngine.UI;

namespace _School_Seducer_.Editor.Scripts
{
    public class InfoMiniGameViewPointUndress : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private Image point;

        public Image ImageComponent => point;
        public Vector3 InitialPosition { get; private set; }
        
        public InfoMiniGameDataUndressPoint Data { get; private set; }

        public Action<InfoMiniGameViewPointUndress> OnClick;
        public Action<InfoMiniGameViewPointUndress> OnPointerUpAction;
        
        public MiniGameUndressPointType Type { get; private set; }

        private Camera _mainCamera;

        private void Awake()
        {
            _mainCamera = Camera.main;
        }

        private void Start()
        {
            InitialPosition = transform.localPosition;
        }

        public void Render(InfoMiniGameDataUndressPoint data)
        {
            Data = data;
            
            point.sprite = data.icon;
            Type = data.type;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector3 mouseWorldPosition = _mainCamera.ScreenToWorldPoint(eventData.position);
            mouseWorldPosition.z = transform.position.z;
            transform.position = mouseWorldPosition;
        }

        public void OnEndDrag(PointerEventData eventData)
        {

        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Vector3 mouseWorldPosition = _mainCamera.ScreenToWorldPoint(eventData.position);
            mouseWorldPosition.z = 0;
            
            OnClick?.Invoke(this);
            //offset = transform.position - mouseWorldPosition;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            OnPointerUpAction?.Invoke(this);
        }
    }
}