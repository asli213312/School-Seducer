using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine;
using UnityEngine.UI;
using _School_Seducer_.Editor.Scripts.UI.Wheel_Fortune;

public class GlobalSelectors : MonoBehaviour
{
	[SerializeField] public UnityEvent selectedEvent;
	[SerializeField] private GiftPopup giftPopup;

	public GiftPopup GiftPopup => giftPopup;

	public Transform SelectedGift { get; private set; }

	public event Action<Transform> SelectedObjectEvent;
	
	public void SelectGift(Transform gift) 
    {
    	SelectedGift = gift;
    	SelectedObjectEvent?.Invoke(gift);
    	selectedEvent?.Invoke();
        
        GiftPopup.transform.position = gift.position + GiftPopup.Offset;
    }
}
