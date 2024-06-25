using System;
using PuzzleGame.Gameplay.Merged.Tasks.Models;
using UnityEngine;

namespace PuzzleGame.Gameplay.Merged
{
    [Serializable]
    public abstract class MiniGamesAbstractTaskData
    {
        [SerializeField] public string description;
        public abstract MiniGamesTaskAbstract CreateTask();
    }
    
    [Serializable]
    public class MiniGamesAbstractTaskDataUseBooster : MiniGamesAbstractTaskData
    {
        public override MiniGamesTaskAbstract CreateTask()
        {
            MiniGamesTaskUseBooster task = new();
            task.Initialize(this);
            return task;
        }
    }
}