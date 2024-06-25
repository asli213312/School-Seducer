using System;
using PuzzleGame.Gameplay.Boosters;
using UnityEngine;

namespace PuzzleGame.Gameplay.Merged.Tasks.Models
{
	public interface IMiniGamesCountableTask
	{
		event Action<int> CountChanged; 
		int MaxCount { get; }
		int Count { get; }
	}
	
	public abstract class MiniGamesTaskAbstract
    {
    	public event Action<MiniGamesTaskAbstract> Completed;
    	public abstract MiniGamesAbstractTaskData Data { get; set; }

        public bool IsCompleted { get; protected set; }

        public void Complete() => IsCompleted = true;

    	public void CheckCompleted(IMiniGamesTaskUpdater updaterStruct)
        {
	        if (IsCompleted) return;
	        
    		Update(updaterStruct);

    		if (TryComplete()) 
    		{
    			Completed?.Invoke(this);
                IsCompleted = true;
    			Debug.Log("Was completed task: " + Data.description);
    		}
    	}

    	public virtual void Initialize(MiniGamesAbstractTaskData data) 
    	{
    		Data = data;
        }
        
        public virtual void OnDestroy() { }
        
    	protected abstract bool TryComplete();
    	protected abstract void Update(IMiniGamesTaskUpdater updaterStruct);
    }
	
    public class MiniGamesTaskGatherSets : MiniGamesTaskAbstract, IMiniGamesCountableTask
    {
	    public event Action<int> CountChanged;
    	public event Action<MiniGamesTaskAbstract> Completed;
        public override MiniGamesAbstractTaskData Data { get => _data; set => _data = value as MiniGamesTaskDataGatherSets; }
        public int MaxCount => _data.setsCount;
        public int Count => _countSets;
        
        private MiniGamesTaskDataGatherSets _data;

    	private int _countSets;
    	private int _numberOfSet;

        protected override void Update(IMiniGamesTaskUpdater updaterStruct) 
    	{
    		if (updaterStruct is MiniGamesTaskGatherSetsUpdater gatherUpdater)  
    		{
	            if (gatherUpdater.numberOfSet == _data.numberForSet)
	            {
		            _numberOfSet = gatherUpdater.numberOfSet;
		            Debug.Log("Was updated setNumber for gather sets at: " + _numberOfSet);
	            }
	            
	            if (_numberOfSet == _data.numberForSet)
                {
	                _countSets++;
	                CountChanged?.Invoke(_countSets);
					Debug.Log("Was updated countSets for gather sets at: " + _countSets);
                }
    		}
    	}

        protected override bool TryComplete() 
    	{
	        if (_data == null)
	        {
		        Debug.LogError("Data task is null!");
		        return false;
	        }
    	
    		bool IsCompleted = false;

    		if (_numberOfSet != _data.numberForSet) return false;

    		if (_countSets == _data.setsCount) IsCompleted = true;
    		return IsCompleted;
    	}
    }

    public class MiniGamesTaskUseBooster : MiniGamesTaskAbstract
    {
	    public override MiniGamesAbstractTaskData Data { get; set; }

	    private bool _isCompleted;

	    public override void Initialize(MiniGamesAbstractTaskData data)
	    {
		    base.Initialize(data);

		    BoostersController.Instance.BoosterProceeded += Complete;
	    }
	    protected override bool TryComplete()
	    {
		    return _isCompleted;
	    }

	    protected override void Update(IMiniGamesTaskUpdater updaterStruct)
	    {
		    
	    }

	    private void Complete(BoosterPreset presetBooster, bool isPurchased)
	    {
		    _isCompleted = true;
		    CheckCompleted(new MiniGamesTaskGatherSetsUpdater(0));
		    BoostersController.Instance.BoosterProceeded -= Complete;
	    }
    }
}