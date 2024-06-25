using UnityEngine;

namespace PuzzleGame.Gameplay.Merged
{
    public class MiniGamesTaskSystem : MonoBehaviour
    {
        [SerializeField] private MiniGamesTaskContainerData data;
        [SerializeField] private MiniGamesTaskRenderer taskRenderer;
        [SerializeField] private MiniGamesTaskTracker tracker;

        public MiniGamesTaskContainerData Data => data;
        public MiniGamesTaskRenderer Renderer => taskRenderer;
        public MiniGamesTaskTracker Tracker => tracker;

        private void Awake()
        {
            taskRenderer.InitializeCore(this);
            tracker.InitializeCore(this);
        }
    }
}