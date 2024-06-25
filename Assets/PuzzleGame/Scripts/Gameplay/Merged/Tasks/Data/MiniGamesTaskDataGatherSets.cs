using System;
using PuzzleGame.Gameplay.Merged.Tasks.Models;
using UnityEngine;

namespace PuzzleGame.Gameplay.Merged.Tasks
{
    [Serializable]
    public class MiniGamesTaskDataGatherSets : MiniGamesAbstractTaskData
    {
        [SerializeField] public int setsCount;
        [SerializeField] public int numberForSet;

        public override MiniGamesTaskAbstract CreateTask()
        {
            MiniGamesTaskGatherSets task = new MiniGamesTaskGatherSets();
            task.Initialize(this);
            return task;
        } 
    }
}