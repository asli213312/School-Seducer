using System.Collections.Generic;
using System.Linq;
using PuzzleGame.Gameplay.Merged.Tasks.Models;
using UnityEngine;

namespace PuzzleGame.Gameplay.Merged
{
    public class MiniGamesTaskTracker : MonoBehaviour, IModule<MiniGamesTaskSystem>
    {
        public List<MiniGamesTaskView> CurrentTasks { get; private set; } = new();
        private List<MiniGamesTaskView> _replacedTasks = new();
        private List<MiniGamesTaskView> _newTasks = new();
        
        private MiniGamesTaskSystem _system;
        public void InitializeCore(MiniGamesTaskSystem system)
        {
            _system = system;
        }

        public void CheckRefreshTasks()
        {
            if (CurrentTasks.All(x => x.Model.IsCompleted))
            {
                ResetTasks();
                InstallTasks();
            }
        }

        public void CheckTasks(IMiniGamesTaskUpdater taskUpdater) 
        {
            CurrentTasks.ForEach(x => x.Model.CheckCompleted(taskUpdater));
            
            Debug.Log("Total tasks count after check: " + CurrentTasks.Count);
        }

        public void InstallTasks() 
        {
            for (int i = 0; i < _system.Data.maxTasks; i++)
                CurrentTasks.Add( _system.Renderer.CreateRandomTask());
        }

        public void ResetTasks()
        {
            foreach (var task in CurrentTasks)
            {
                Destroy(task.gameObject);
            }
            
            CurrentTasks.Clear();
        }

        private void AddNewTasks()
        {
            if (_newTasks.Count > 0)
            {
                CurrentTasks.AddRange(_newTasks);
                _newTasks.Clear();
            }
        }

        private void RemoveCompletedTasks()
        {
            if (_replacedTasks.Count > 0)
            {
                for (int i = CurrentTasks.Count - 1; i >= 0; i--)
                {
                    var task = CurrentTasks[i];
                    if (_replacedTasks.Contains(task))
                    {
                        CurrentTasks.Remove(task);
                        task.Model.OnDestroy();
                        Destroy(task);
                    }
                }

                _replacedTasks.Clear();
            }
        }

        public void RefreshCompletedTask(MiniGamesTaskAbstract taskCompleted)
        {
            for (var index = 0; index < CurrentTasks.Count; index++)
            {
                var task = CurrentTasks[index];

                if (task.Model == taskCompleted)
                {
                    task.Model.Completed -= RefreshCompletedTask;
                    _replacedTasks.Add(task);
                    //_newTasks.Add(_system.Renderer.CreateRandomTask());
                    Debug.Log("Was refreshed task: " + task.Model.Data.description);
                    break;
                }

                Debug.Log("Can't find completed task to refresh: " + taskCompleted.Data.description);
            }
        }

        private void OnDestroy() 
        {
            if (CurrentTasks.Count == 0) return;

            foreach (var task in CurrentTasks)
                task.Model.Completed -= RefreshCompletedTask;
        }
    }
}