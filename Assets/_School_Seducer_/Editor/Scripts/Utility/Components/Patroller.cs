using System;
using System.Collections;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace _School_Seducer_.Editor.Scripts.Utility.Components
{
	[Serializable]
	public class PatrolPath
	{
		public string id;
		public Transform[] points;
	}
	public class Patroller : MonoBehaviour
	{
		[SerializeField] private Transform target;
	    [SerializeField] private PatrolPath path;
	    [SerializeField] private float speed = 1f;
	    [SerializeField] private float delayToNext = 1f;
	    [SerializeField] private bool useOtherPaths;
	    [SerializeField, ShowIf(nameof(useOtherPaths))] private PatrolPath[] paths;

	    [Header("Events")]
	    [SerializeField] private UnityEvent endEvent;
	    [SerializeField] private UnityEvent stopEvent;
	    [SerializeField] private UnityEvent continueEvent;

	    private bool _isWorking;
	    private int _stopCount;
	    private int _continueCount;
	    private int _endCount;

	    public int CurrentCounter { get; private set; } = 999;

	    public void ChangePathById(string id) => path = paths.FirstOrDefault(x => x.id == id);

	    public void SelectCounter(string counterName)
	    {
		    int counter = 0;
		    
		    switch (counterName)
		    {
			    case "Stop": counter = _stopCount; break;
			    case "End": counter = _endCount; break;
			    case "Continue": counter = _continueCount; break;
		    }

		    CurrentCounter = counter;
	    }

	    public void InvokePatrol()
	    {
	        StartCoroutine(PatrolCoroutine());
	    }

	    public void Continue() 
	    {
	    	_isWorking = true;
	    	continueEvent?.Invoke();

	        _continueCount++;
	    } 

	    public void StopPatrol()
	    {
	        _isWorking = false;
	        stopEvent?.Invoke();

	        _stopCount++;
	    }

	    public void EndPatrol()
	    {
	        StopCoroutine("PatrolCoroutine");
	        endEvent?.Invoke();

	        _endCount++;
	    }

	    private IEnumerator PatrolCoroutine()
	    {
	        _isWorking = true;

	        int index = 0;

	        while (_isWorking)
		    {
		        // Получаем текущую целевую точку
		        Vector3 targetPosition = path.points[index].position;

		        // Перемещаем объект к текущей целевой точке
		        while (Vector3.Distance(target.position, targetPosition) > 0.1f)
		        {
		            target.position = Vector3.MoveTowards(target.position, targetPosition, speed * Time.deltaTime);
		            yield return null;
		        }

		        // Переходим к следующей точке или начинаем сначала, если это последняя точка
		        index = (index + 1) % path.points.Length;

		        // Пауза перед следующим перемещением
		        yield return new WaitForSeconds(delayToNext);

		        // Если достигнута последняя точка, телепортируем объект к первой точке
		        if (index == 0)
		        {
		            target.position = path.points[0].position;
		        }
		    }
		}
	}
}