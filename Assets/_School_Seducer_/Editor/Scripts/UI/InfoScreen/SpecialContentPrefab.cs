using System;
using _School_Seducer_.Editor.Scripts.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _School_Seducer_.Editor.Scripts
{
    public class SpecialContentPrefab : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private TextMeshProUGUI label;
        [SerializeField] private Image checkerBought;
        [SerializeField] private Image frame;
        [SerializeField] private Image preview;
        
        public Action<SpecialContentPrefab> OnClick;
        public GallerySlotDataBase Data { get; private set; }

        public void OnPointerDown(PointerEventData eventData) 
        {
        	OnClick?.Invoke(this);
        }	

        public void Render(GallerySlotDataBase data) 
        {
	        GallerySlotDataSpecial specialData = data as GallerySlotDataSpecial;
	        Data = data;

	        frame.sprite = specialData.frame;
	        preview.sprite = specialData.preview;
	        label.text = specialData.label;

        	if (data.AddedInGallery)
        		checkerBought.gameObject.SetActive(true);
        	else
        		checkerBought.gameObject.SetActive(false);
        }
    }
}