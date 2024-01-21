using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PuzzleGame.UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class CoinsCounter : MonoBehaviour
    {
        TextMeshProUGUI label;

        void OnProgressUpdate()
        {
            label.text = "+ " + UserProgress.Current.Coins;
        }

        void Start()
        {
            label = GetComponent<TextMeshProUGUI>();

            OnProgressUpdate();
            UserProgress.Current.ProgressUpdate += OnProgressUpdate;
        }

        void OnDestroy()
        {
            UserProgress.Current.ProgressUpdate -= OnProgressUpdate;
        }
    }
}
