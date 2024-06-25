using PuzzleGame.Gameplay.Merged.Tasks.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PuzzleGame.Gameplay.Merged
{
    public class MiniGamesTaskView : MonoBehaviour
    {
        [SerializeField] private Button completeTaskButton;
        [SerializeField] private Toggle checker;
        [SerializeField] private TextMeshProUGUI description;

        public MiniGamesTaskAbstract Model { get; private set; }

        private int _countCompletion;

        public void Initialize()
        {
            checker.onValueChanged.AddListener(OnToggle);
        }

        public void Render(MiniGamesTaskAbstract model)
        {
            description.fontStyle = FontStyles.Normal;
            checker.isOn = false;

            Model = model;
            description.text = model.Data.description.ToUpper();

            if (Model is IMiniGamesCountableTask countableTask)
            {
                description.text += $"   ({_countCompletion}/{countableTask.MaxCount})";
                countableTask.CountChanged += UpdateCountableTask;
            }

            Model.Completed += ToggleOn;
            completeTaskButton.onClick.AddListener(Model.Complete);
        }

        private void UpdateCountableTask(int count)
        {
            IMiniGamesCountableTask countableTask = Model as IMiniGamesCountableTask;
            string taskDescription = Model.Data.description.ToUpper();
            description.text = taskDescription + $"   ({count}/{countableTask.MaxCount})";
        }

        private void ToggleOn(MiniGamesTaskAbstract model)
        {
            checker.isOn = true;
            Debug.Log("Toggled!", gameObject);
        }

        private void OnToggleOn()
        {
            description.fontStyle = FontStyles.Strikethrough;
            description.color = Color.green;
        }

        private void OnToggleOff()
        {
            description.fontStyle = FontStyles.Normal;
        }

        private void OnToggle(bool value)
        {
            if (value)
            {
                OnToggleOn();
            }
            else
                OnToggleOff();
        }

        private void OnDestroy()
        {
            checker.onValueChanged.RemoveListener(OnToggle);

            if (Model != null)
            {
                Model.Completed -= ToggleOn;
                completeTaskButton.onClick.RemoveListener(Model.Complete);
                
                if (Model is IMiniGamesCountableTask countableTask)
                {
                    countableTask.CountChanged -= UpdateCountableTask;
                }
                
                Model.OnDestroy();
            }
        }
    }
}