using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine;

public class EngineEvents : MonoBehaviour
{
	[SerializeField] private UnityEvent awakeEvent;
	[SerializeField] private UnityEvent startEvent;
	[SerializeField] private UnityEvent enableEvent;
	[SerializeField] private UnityEvent disableEvent;
	[SerializeField] private UnityEvent destroyEvent;

	void Awake()
    {
        awakeEvent?.Invoke();
    }

    void Start()
    {
        startEvent?.Invoke();
    }

    void OnEnable() 
    {
    	enableEvent?.Invoke();
    }

    void OnDisable() 
    {
    	disableEvent?.Invoke();
    }

    void OnDestroy() 
    {
    	destroyEvent?.Invoke();
    }
}
