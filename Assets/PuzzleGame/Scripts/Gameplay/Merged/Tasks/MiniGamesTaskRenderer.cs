using System.Collections.Generic;
using System.Linq;
using PuzzleGame.Gameplay.Merged.Tasks.Models;
using UnityEngine;
using UnityEngine.UI;

namespace PuzzleGame.Gameplay.Merged
{
    public class MiniGamesTaskRenderer : MonoBehaviour, IModule<MiniGamesTaskSystem>
    {
        [SerializeField] private MiniGamesTaskView taskView;
        [SerializeField] private Transform content;

        private MiniGamesTaskSystem _system;

        public void InitializeCore(MiniGamesTaskSystem system)
        {
            _system = system;
        }

        public MiniGamesTaskView CreateRandomTask() 
        {
            MiniGamesTaskAbstract model = CreateTaskModel();
            MiniGamesTaskView view = Instantiate(taskView, content);
            view.Initialize();
            view.Render(model);

            Debug.Log("MINI_GAMES was created task: " + model.Data.description);
            return view;
        }

        private MiniGamesTaskAbstract CreateTaskModel()
        {
            if (_system.Tracker.CurrentTasks.Count == 0)
            {
                CreateTaskFromScope(_system.Data.tasks.ToList());
            }
            
            var currentTaskData = _system.Tracker.CurrentTasks.Select(task => task.Model.Data);
            var availableTasks = _system.Data.tasks.Except(currentTaskData).ToList();

            if (availableTasks.Count == 0)
            {
                Debug.LogError("No tasks found to create!");
                return null;
            }
            
            return CreateTaskFromScope(availableTasks); 
        }

        private MiniGamesTaskAbstract CreateTaskFromScope(List<MiniGamesAbstractTaskData> availableTasks)
        {
            int randomIndex = Random.Range(0, availableTasks.Count);
            MiniGamesTaskAbstract newTask = availableTasks[randomIndex].CreateTask();
            newTask.Completed += _system.Tracker.RefreshCompletedTask;
            return newTask;
        }
    }
}